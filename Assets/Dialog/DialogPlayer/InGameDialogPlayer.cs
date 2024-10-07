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
            //����׿�
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
            //�ִϸ��̼� ����κ�
            if (_curReadingNode is NormalNodeSO node && _isReadingDialog)
            {
                _animPlayer.NormalDialogAnimation(node);
            }
        }

        #region DialogRead

        /// <summary>
        /// �ִϸ��̼� ����
        /// </summary>
        public override void StartDialog()
        {
            if (_isReadingDialog)
                Debug.Log("�̹� �������ε�~\n��~�� ��");

            _isReadingDialog = true;
            _curReadingNode = dialog.nodes[0];
            ReadSingleLine();
        }

        /// <summary>
        /// �ִϸ��̼� ����
        /// ��ǳ�� �� ���ְ�, _isReadingDialog false�� �ٲ���
        /// </summary>
        public override void EndDialog()
        {
            characters.ForEach((c) => c.talkBubbleObj.SetActive(false));
            _isReadingDialog = false;
        }


        /// <summary>
        /// ���̾�α� ��� �� �ϳ��� �о���
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
        /// �д� ���߿� ��ŵ
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
        /// �Ϲ� ��� �о��ִ� �ڷ�ƾ
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ReadingNormalNodeRoutine(NormalNodeSO node)
        {
            TextMeshProUGUI tmp = _curCharacter.contentTxt;

            //�ΰ��� ���� �ƴϸ� �Ѱ�!
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
                //������ �ٷ� �Ѱ�
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

            //�� �ִϸ��̼��� �̷��� ��������
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
