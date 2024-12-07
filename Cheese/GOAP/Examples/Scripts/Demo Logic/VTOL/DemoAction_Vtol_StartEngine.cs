using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Vtol_StartEngine", menuName = "Demo/Action/Vtol/StartEngine")]
    public class DemoAction_Vtol_StartEngine : DemoAction_Vtol
    {
        public override void StartAction()
        {
            vtol.pilot.engines.StartEngine();
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (vtol.pilot.engines.rpm > 0.9f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}