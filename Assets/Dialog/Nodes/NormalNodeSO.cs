using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(menuName = "SO/NormalNode")]
    public class NormalNodeSO : NodeSO
    {
        protected string tagExceptedReader;
        [SerializeField] protected string reader;

        protected string tagExceptedContents;
        [SerializeField] protected string contents;

        public bool useForVisualNovel;
        #region ForVisualNovel

        public bool useImage;
        public bool useBackground;

        public List<ImageStruct> images;
        public Sprite background;

        #endregion

        public List<TagAnimation> readerTagAnimations = new();
        public List<TagAnimation> contentTagAnimations = new();
        public NodeSO nextNode;

        public string GetContents() => tagExceptedContents;
        public string GetReaderName() => tagExceptedReader;

        public override List<TagAnimation> GetAllAnimations()
        {
            List<TagAnimation> tagAnimations = new List<TagAnimation>();

            readerTagAnimations.ForEach(anim => tagAnimations.Add(anim));
            contentTagAnimations.ForEach(anim => tagAnimations.Add(anim));

            return tagAnimations;
        }

        private void OnEnable()
        {
            tagExceptedContents = contents;
            contentTagAnimations = TagParser.ParseAnimation(ref tagExceptedContents);
            tagExceptedReader = reader;
            readerTagAnimations = TagParser.ParseAnimation(ref tagExceptedReader);

            contentTagAnimations.ForEach(anim =>
            {
                if (!anim.SetParameter())
                    Debug.LogError(tagExceptedContents);
            });
            readerTagAnimations.ForEach(anim =>
            {
                if (!anim.SetParameter())
                    Debug.LogError(tagExceptedReader);
            });
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

