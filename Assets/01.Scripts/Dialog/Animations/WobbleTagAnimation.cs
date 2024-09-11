using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialog
{
    public class WobbleTagAnimation : TagAnimation
    {
        private float _power;

        public WobbleTagAnimation()
        {
            _timing = AnimTiming.Update;
            tagType = TagEnum.Wobble;
            _checkEndPos = true;
        }

        public override void Play()
        {

        }

        public override void SetParameter()
        {
            if (float.TryParse(Param, out _power) == false)
            {
                Debug.LogError($"{tagType.ToString()} ({Param}) : Parameter is wrong");
            }
        }
    }
}
