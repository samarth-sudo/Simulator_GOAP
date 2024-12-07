namespace Cheese.GOAP.Demo
{
    public class DemoAction_Vtol : DemoAction
    {
        protected DemoUnit_Vtol vtol;

        public override void SetUp(DemoUnit unit)
        {
            base.SetUp(unit);
            vtol = unit as DemoUnit_Vtol;
        }
    }
}