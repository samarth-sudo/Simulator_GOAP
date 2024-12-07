using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Vtol_TakeOff", menuName = "Demo/Action/Vtol/TakeOff")]
    public class DemoAction_Vtol_TakeOff : DemoAction_Vtol
    {
        public float initialClimbHeight = 5;

        private Vector3 targetPos;
        private bool alreadyFlying;

        public override void StartAction()
        {
            alreadyFlying = vtol.IsFlying();
            if (alreadyFlying == false)
            {
                targetPos = vtol.pilot.tf.position + Vector3.up * initialClimbHeight;
                vtol.goapDriver.GoToPosTerminal(targetPos);
            }
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if (alreadyFlying)
                return DemoSequence.SequenceStatus.Completed;

            if ((vtol.pilot.tf.position - targetPos).magnitude < 1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}