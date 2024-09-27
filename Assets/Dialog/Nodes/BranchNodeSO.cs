using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    public class BranchNodeSO : NodeSO
    {
        public ConditionSO condition;
        public List<NodeSO> nextNodes = new List<NodeSO>();

        public Action onChangeCondition;

        public override List<TagAnimation> GetAllAnimations()
        {
            List<TagAnimation> anims = new List<TagAnimation>();
            return anims;
        }

        private void OnValidate()
        {
            for (int i = nextNodes.Count; i < condition.GetBranchCount(); i++)
                nextNodes.Add(null);

            for (int i = 0; i < nextNodes.Count - condition.GetBranchCount(); i++)
                nextNodes.RemoveAt(nextNodes.Count - 1);

            //nextNodes = new List<NodeSO>(condition.GetBranchCount());
            onChangeCondition?.Invoke();
        }
    }
}
