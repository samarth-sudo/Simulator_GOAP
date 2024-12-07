using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Helicopter_TakeOff", menuName = "Demo/Action/Helicopter/TakeOff")]
    public class DemoAction_Helicopter_TakeOff : DemoAction_Helicopter
    {
        public float initialClimbHeight = 5;

        private Vector3 targetPos;
        private bool alreadyFlying;

        public override void StartAction()
        {
            alreadyFlying = helicopter.IsFlying();
            if (alreadyFlying == false)
            {
                targetPos = helicopter.physics.tf.position + Vector3.up * initialClimbHeight;
                helicopter.goapDriver.GoToPosTerminal(targetPos);
            }
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (alreadyFlying)
                return DemoSequence.SequenceStatus.Completed;

            if ((helicopter.physics.tf.position - targetPos).magnitude < 1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}