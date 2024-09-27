using UnityEngine;

namespace Dialog
{
    public abstract class ConditionSO : ScriptableObject
    {
        public abstract int Decision();

        public abstract int GetBranchCount();
    }
}
