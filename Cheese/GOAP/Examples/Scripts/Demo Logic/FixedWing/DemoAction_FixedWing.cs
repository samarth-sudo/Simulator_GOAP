namespace Cheese.GOAP.Demo
{
    public class DemoAction_FixedWing : DemoAction
    {
        protected DemoUnit_FixedWing fixedWingUnit;

        public override void SetUp(DemoUnit unit)
        {
            base.SetUp(unit);
            fixedWingUnit = unit as DemoUnit_FixedWing;
        }
    }
}
