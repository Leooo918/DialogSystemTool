using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(menuName = "SO/NormalNode")]
    public class NormalNodeSO : NodeSO
    {
        public string reader;
        protected string tagExceptedContents;
        [SerializeField] protected string contents;

        public bool useForInGame;

        #region ForVisualNovel


        public bool useImage;
        public bool useBackground;

        public List<ImageStruct> images;
        public Sprite background;

        #endregion

        public List<TagAnimation> tagAnimations = new();
        public NodeSO nextNode;

        public string GetContents() => tagExceptedContents;

        private void OnEnable()
        {
            tagExceptedContents = contents;
            tagAnimations = TagParser.ParseAnimation(ref tagExceptedContents);
            Debug.Log(contents);

            foreach (var anim in tagAnimations)
            {
                anim.SetParameter();
                Debug.Log(anim.tagType);
            }
        }
    }

    [Serializable]
    public class ImageStruct
    {
        public Sprite image;

        public Vector2 position;
        public Vector2 size;
    }
}

