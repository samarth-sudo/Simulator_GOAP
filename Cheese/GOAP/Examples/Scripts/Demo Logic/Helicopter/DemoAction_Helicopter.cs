namespace Cheese.GOAP.Demo
{
    public class DemoAction_Helicopter : DemoAction
    {
        protected DemoUnit_Helicopter helicopter;

        public override void SetUp(DemoUnit unit)
        {
            base.SetUp(unit);
            helicopter = unit as DemoUnit_Helicopter;
        }
    }
}