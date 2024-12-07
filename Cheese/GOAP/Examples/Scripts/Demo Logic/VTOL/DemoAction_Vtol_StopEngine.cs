using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Vtol_StopEngine", menuName = "Demo/Action/Vtol/StopEngine")]
    public class DemoAction_Vtol_StopEngine : DemoAction_Vtol
    {
        public override void StartAction()
        {
            vtol.pilot.engines.ShutdownEngine();
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (vtol.pilot.engines.rpm < 0.1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}