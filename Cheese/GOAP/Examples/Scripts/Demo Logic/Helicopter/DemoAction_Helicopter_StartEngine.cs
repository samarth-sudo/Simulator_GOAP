using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Helicopter_StartEngine", menuName = "Demo/Action/Helicopter/StartEngine")]
    public class DemoAction_Helicopter_StartEngine : DemoAction_Helicopter
    {
        public override void StartAction()
        {
            helicopter.physics.StartEngine();
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (helicopter.physics.rpm > helicopter.physics.maxRpm * 0.9f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}