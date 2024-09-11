using UnityEngine;

namespace Dialog
{
    public abstract class DialogPlayer : MonoBehaviour
    {
        public DialogSO dialog;
        [HideInInspector]public bool stopReading = false;
        protected NodeSO _curReadingNode;
        protected Coroutine _readingNodeRoutine;
        protected bool _isReadingDialog = false;

        public abstract void StartDialog();
        public abstract void EndDialog();
        public abstract void ReadSingleLine();
        public abstract void SkipSingleLine();

        protected virtual bool GetInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
}
