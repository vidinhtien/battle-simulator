
namespace BattleSimulatorV2.FlexibleV2
{
    public class NormalEffect : AbstractEffect
    {

        public override bool IsUsing()
        {
            return isUsing;
        }

        public override void Active()
        {
            gameObject.SetActive(true);
        }
    }
}