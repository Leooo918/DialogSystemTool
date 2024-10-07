using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialog
{
    [RequireComponent(typeof(AnimationPlayer))]
    public class InGameDialogPlayer : DialogPlayer
    {
        private AnimationPlayer _animPlayer;

        [SerializeField] private RectTransform _optionParent;
        [SerializeField] private List<IngameCharacterStruct> characters;
        private IngameCharacterStruct _curCharacter;

        private TMP_TextInfo _txtInfo;
        private bool _optionSelected = false;
        private NodeSO _optionSelectNode;
        private List<OptionButton> _optionBtns;

        private void Awake()
        {
            _animPlayer = GetComponent<AnimationPlayer>();
        }


        private void Update()
        {
            //디버그용
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartDialog();
            }

            if (GetInput())
            {
                SkipSingleLine();
            }
        }

        private void LateUpdate()
        {
            //애니메이션 실행부분
            if (_curReadingNode is NormalNodeSO node && _isReadingDialog)
            {
                _animPlayer.NormalDialogAnimation(node);
            }
        }

        #region DialogRead

        /// <summary>
        /// 애니메이션 실행
        /// </summary>
        public override void StartDialog()
        {
            if (_isReadingDialog)
                Debug.Log("이미 실행중인데~\n허~접 ♥");

            _isReadingDialog = true;
            _curReadingNode = dialog.nodes[0];
            ReadSingleLine();
        }

        /// <summary>
        /// 애니메이션 종료
        /// 말풍선 다 꺼주고, _isReadingDialog false로 바꿔줘
        /// </summary>
        public override void EndDialog()
        {
            characters.ForEach((c) => c.talkBubbleObj.SetActive(false));
            _isReadingDialog = false;
        }


        /// <summary>
        /// 다이어로그 노드 딱 하나만 읽어줌
        /// </summary>
        public override void ReadSingleLine()
        {
            if (_curReadingNode == null)
            {
                EndDialog();
                return;
            }

            DialogConditionManager.Instance.AddValue(_curReadingNode.guid);

            if (_curReadingNode is NormalNodeSO node)
            {
                characters.ForEach(c =>
                {
                    if (c.name == node.GetReaderName())
                    {
                        _curCharacter = c;
                        _curCharacter.talkBubbleObj.SetActive(true);
                        _animPlayer.Init(_curCharacter.contentTxt);
                    }
                });
                _readingNodeRoutine = StartCoroutine(ReadingNormalNodeRoutine(node));
            }
            else if (_curReadingNode is OptionNodeSO option)
            {
                ReadingOptionNodeRoutine(option);
            }
            else if (_curReadingNode is BranchNodeSO branch)
            {
                JudgementCondition(branch);
            }
        }


        /// <summary>
        /// 읽는 도중에 스킵
        /// </summary>
        public override void SkipSingleLine()
        {

        }

        private IEnumerator SkipRoutine()
        {
            _curCharacter.contentTxt.maxVisibleCharacters = _curCharacter.contentTxt.text.Length;
            StopCoroutine(_readingNodeRoutine);

            yield return null;

            if (_curReadingNode is NormalNodeSO node)
            {
                StartCoroutine(ReadingNormalNodeRoutine(node));
            }
        }

        #endregion

        #region ReadingRoutines

        /// <summary>
        /// 일반 노드 읽어주는 코루틴
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ReadingNormalNodeRoutine(NormalNodeSO node)
        {
            TextMeshProUGUI tmp = _curCharacter.contentTxt;

            //인게임 용이 아니면 넘겨!
            if (node.useForVisualNovel)
            {
                _curReadingNode = node.nextNode;
                ReadSingleLine();
                yield break;
            }

            tmp.SetText(node.GetContents());
            tmp.maxVisibleCharacters = 0;
            InitNodeAnim(node);
            _isReadingDialog = true;

            while (tmp.maxVisibleCharacters < tmp.text.Length)
            {
                //공백은 바로 넘겨
                if (tmp.text[tmp.maxVisibleCharacters++] == ' ') continue;

                yield return new WaitForSeconds(_textOutDelay);
                yield return new WaitUntil(() => stopReading == false);
            }

            StartCoroutine(WaitNormalNodeRoutine(node.nextNode));
        }

        private IEnumerator WaitNormalNodeRoutine(NodeSO nextNode)
        {
            yield return null;
            yield return new WaitUntil(() => GetInput());
            yield return null;

            //끝 애니메이션은 이렇게 실행해줘
            _playingEndAnimation = true;
            yield return new WaitUntil(() => !_playingEndAnimation);

            _isReadingDialog = false;
            _curReadingNode = nextNode;
            _curCharacter.talkBubbleObj.SetActive(false);

            yield return new WaitForSeconds(_nextNodeDealy);

            ReadSingleLine();
        }

        private void ReadingOptionNodeRoutine(OptionNodeSO node)
        {
            _optionSelected = false;
            _optionBtns = new List<OptionButton>();
            _optionParent.gameObject.SetActive(true);
            InitNodeAnim(node);

            for (int i = 0; i < node.options.Count; i++)
            {
                OptionButton optionButton = Instantiate(node.optionPf, _optionParent);
                optionButton.SetOption(node.options[i], _animPlayer);
                optionButton.OnClcickEvent += OnSelectOption;

                _optionBtns.Add(optionButton);
            }

            StartCoroutine(WaitOptionNodeRoutine(node));
        }

        private void OnSelectOption(NodeSO node)
        {
            _optionSelected = true;
            _optionSelectNode = node;
        }

        private IEnumerator WaitOptionNodeRoutine(OptionNodeSO node)
        {
            yield return new WaitUntil(() => _optionSelected);

            _optionParent.gameObject.SetActive(false);
            _optionBtns.ForEach(option => Destroy(option.gameObject));
            _curReadingNode = _optionSelectNode;
            _isReadingDialog = false;

            yield return new WaitForSeconds(_nextNodeDealy);

            ReadSingleLine();
        }

        private void JudgementCondition(BranchNodeSO branch)
        {
            bool decision = branch.condition.Decision();
            _curReadingNode = branch.nextNodes[decision ? 0 : 1];
            ReadSingleLine();
        }

        #endregion

        private void InitNodeAnim(NodeSO node)
        {
            List<TagAnimation> anims = node.GetAllAnimations();

            anims.ForEach((anim) =>
            {
                anim.Init();

                if (anim is SpriteAnimation srAnim)
                    srAnim.Init(_curCharacter.spriteRenderer);

                if (anim is StopReadingAnimation stopAnim)
                    stopAnim.Init(this);
            });
        }
    }

    [Serializable]
    public struct IngameCharacterStruct
    {
        public string name;
        public GameObject talkBubbleObj;
        public TextMeshProUGUI contentTxt;
        public SpriteRenderer spriteRenderer;
    }
}
