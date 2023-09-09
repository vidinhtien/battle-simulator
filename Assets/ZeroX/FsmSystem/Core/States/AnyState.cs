namespace ZeroX.FsmSystem
{
    [System.Serializable]
    public class AnyState : State
    {
        public bool canTransitionToSelf = true;

        public const string fn_canTransitionToSelf = "canTransitionToSelf";
    }
}