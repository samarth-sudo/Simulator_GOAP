using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Vtol_LandAtWaypoint", menuName = "Demo/Action/Vtol/LandAtWaypoint")]
    public class DemoAction_Vtol_LandAtWaypoint : DemoAction_Vtol
    {
        public GOAPGoal_MoveTo overrideGoal;
        public bool startGoalImmediate = true;

        public float landingApproachHeight = 5;
        private Vector3 targetPos;

        public override void StartAction()
        {
            targetPos = vtol.waypointPos + Vector3.up * landingApproachHeight;
            if (overrideGoal != null)
            {
                GOAPGoal_MoveTo instantiatedGoal = Instantiate(overrideGoal);
                instantiatedGoal.startingPos = vtol.goapDriver.tf.position;
                instantiatedGoal.goalPos = targetPos;
                vtol.goapDriver.GoToGoal(instantiatedGoal, startGoalImmediate);
            }
            else
            {
                vtol.goapDriver.GoToPos(targetPos, startGoalImmediate);
            }
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if ((vtol.pilot.tf.position - targetPos).magnitude < 1f)
                vtol.goapDriver.GoToPosTerminal(vtol.waypointPos);

            if ((vtol.pilot.tf.position - vtol.waypointPos).magnitude < 3f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}