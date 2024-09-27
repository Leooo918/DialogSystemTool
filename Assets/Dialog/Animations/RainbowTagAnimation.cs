using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialog
{
    public class RainbowTagAnimation : TagAnimation
    {

        public RainbowTagAnimation()
        {
            _timing = AnimTiming.Update;
            tagType = TagEnum.Rainbow;
            _checkEndPos = true;
        }

        public override void Play()
        {

        }

        public override bool SetParameter() { return true; }
    }
}
