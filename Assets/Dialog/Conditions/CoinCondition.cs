using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(menuName = "SO/Condition/CoinCondition")]
    public class CoinCondition : ConditionSO
    {
        public List<int> coinLess = new List<int>();

        //�ϴ� ����׿� ���߿��� �ٸ� ������ ������ �� �ֵ��� �ؾ���
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
