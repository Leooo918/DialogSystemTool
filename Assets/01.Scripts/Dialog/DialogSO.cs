using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(menuName = "SO/DialogSO")]
    public class DialogSO : ScriptableObject
    {
        public List<NodeSO> nodes;
    }
}
