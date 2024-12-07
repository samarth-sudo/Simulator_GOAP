using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Helicopter_StopEngine", menuName = "Demo/Action/Helicopter/StopEngine")]
    public class DemoAction_Helicopter_StopEngine : DemoAction_Helicopter
    {
        public override void StartAction()
        {
            helicopter.physics.ShutdownEngine();
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (helicopter.physics.rpm < helicopter.physics.maxRpm * 0.1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}