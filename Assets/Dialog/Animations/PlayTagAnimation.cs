using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    public class PlayTagAnimation : SpriteAnimation
    {
        private int _animBoolHash;

        public PlayTagAnimation()
        {
            _timing = AnimTiming.Start;
            tagType = TagEnum.Play;
            _checkEndPos = false;
        }

        public override void Play()
        {

        }

        public override bool SetParameter()
        {
            _animBoolHash = Animator.StringToHash(Param);

            return true;
        }
    }
}
