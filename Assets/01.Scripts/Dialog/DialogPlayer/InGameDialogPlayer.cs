using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialog
{
    public class InGameDialogPlayer : DialogPlayer
    {
        [SerializeField] private List<CharacterStruct> characters;
        [SerializeField] private float _textOutDelay;
        [SerializeField] private float _nextNodeDealy;
        private bool _playingEndAnimation = false;
        private CharacterStruct _curCharacter;

        private TMP_TextInfo _txtInfo;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartDialog();
            }
        }

        private void LateUpdate()
        {
            if (_curReadingNode is NormalNodeSO node && _isReadingDialog)
            {
                NormalNodeAnimatingOnUpdate(node);
            }
        }

        #region DialogRead

        public override void StartDialog()
        {
            if (_isReadingDialog)
                Debug.Log("이미 실행중인데~\n허~접 ♥");

            _isReadingDialog = true;
            _curReadingNode = dialog.nodes[0];
            ReadSingleLine();
        }

        public override void EndDialog()
        {
            characters.ForEach((c) => c.talkBubbleObj.SetActive(false));
        }

        public override void ReadSingleLine()
        {
            if (_curReadingNode is NormalNodeSO node)
            {
                characters.ForEach(c =>
                {
                    if(c.name == node.reader)
                    {
                        _curCharacter = c;
                        _curCharacter.talkBubbleObj.SetActive(true);
                        Debug.Log(c.name);
                    }
                });
                _readingNodeRoutine = StartCoroutine(ReadingNormalNodeRoutine(node));
            }
        }

        public override void SkipSingleLine()
        {

        }

        #endregion

        #region Animations

        private void NormalNodeAnimatingOnUpdate(NormalNodeSO node)
        {
            _curCharacter.tmp.ForceMeshUpdate();
            _txtInfo = _curCharacter.tmp.textInfo;

            bool playedEndAnim = false;

            node.tagAnimations.ForEach(anim =>
            {
                if (anim.Timing == AnimTiming.End)
                {
                    if(_playingEndAnimation && !anim.EndAnimating)
                    {
                        anim.SetTextInfo(_txtInfo);
                        anim.Play();
                        playedEndAnim = true;
                    }
                    return;
                }

                anim.SetTextInfo(_txtInfo);
                anim.Play();
            });

            if (!playedEndAnim) _playingEndAnimation = false;

            for (int i = 0; i < _txtInfo.meshInfo.Length; ++i)
            {
                var meshInfo = _txtInfo.meshInfo[i];

                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;

                _curCharacter.tmp.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        #endregion

        #region ReadingRoutines

        private IEnumerator ReadingNormalNodeRoutine(NormalNodeSO node)
        {
            TextMeshProUGUI tmp  = _curCharacter.tmp;

            //인게임 용이 아니면 넘겨!
            if (!node.useForInGame)
            {
                _curReadingNode = node.nextNode;
                ReadSingleLine();
                yield break;
            }

            tmp.SetText(node.GetContents());
            tmp.maxVisibleCharacters = 0;
            InitNode(node);
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

            _playingEndAnimation = true;
            yield return new WaitUntil(() => !_playingEndAnimation);

            //if (_curReadingNode is NormalNodeSO node)
            //{
            //    Debug.Log("밍?");
            //    yield return NormalNodeAnimatingOnEnd(node);
            //    Debug.Log("밍!");
            //}

            _isReadingDialog = false;
            _curReadingNode = nextNode;
            _curCharacter.talkBubbleObj.SetActive(false);

            yield return new WaitForSeconds(_nextNodeDealy);

            ReadSingleLine();
        }

        #endregion

        private void InitNode(NodeSO node)
        {
            if (node is NormalNodeSO normalNode)
            {
                normalNode.tagAnimations.ForEach((anim) =>
                {
                    anim.Init();

                    if (anim is SpriteAnimation srAnim)
                        srAnim.Init(_curCharacter.spriteRenderer);
                    if (anim is StopReadingAnimation stopAnim)
                        stopAnim.Init(this);
                });
            }
        }
    }

    [Serializable]
    public struct CharacterStruct
    {
        public string name;
        public GameObject talkBubbleObj;
        public TextMeshProUGUI tmp;
        public SpriteRenderer spriteRenderer;
    }
}
