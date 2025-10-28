namespace Assets.Scripts.Jumping.Controllers
{
    public interface IFlyKeyboardController
    {
        bool isJump();
        (bool, bool) isLanding();
        bool isStart();

        void resetState();
    }
}