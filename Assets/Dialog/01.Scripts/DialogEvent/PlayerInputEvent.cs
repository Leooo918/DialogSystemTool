namespace Dialog
{
    public class PlayerInputEvent : DialogEvent
    {
        //public CafeInput cafeInput;
        public bool isEnable;

        public override void PlayEvent(DialogPlayer dialogPlayer, Actor actor)
        {
            isCompleteEvent = true;
            //if (isEnable) cafeInput.EnableInput();
            //else cafeInput.DisableInput();
        }
    }
}
