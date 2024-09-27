using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(menuName = "SO/Condition/CoinCondition")]
    public class CoinCondition : ConditionSO
    {
        public List<int> coinLess = new List<int>();

        //일단 디버그용 나중에는 다른 곳에서 가져올 수 있도록 해야함
        private int coin = 10;

        public override int Decision()
        {
            for(int i = 0; i < coinLess.Count; i++)
            {
                if (coinLess[i] > coin)
                {
                    return i;
                }
            }

            return coinLess.Count;
        }

        public override int GetBranchCount() => coinLess.Count + 1;
    }
}
