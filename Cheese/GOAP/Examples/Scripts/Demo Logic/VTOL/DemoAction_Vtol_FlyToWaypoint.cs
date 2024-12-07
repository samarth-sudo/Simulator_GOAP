using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Vtol_FlyToWaypoint", menuName = "Demo/Action/Vtol/FlyToWaypoint")]
    public class DemoAction_Vtol_FlyToWaypoint : DemoAction_Vtol
    {
        public GOAPGoal_MoveTo overrideGoal;
        public bool startGoalImmediate = true;

        public float normalHeight = 30;
        private Vector3 targetPos;

        public override void StartAction()
        {
            targetPos = vtol.waypointPos + vtol.waypointNormal * normalHeight;
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
            if ((vtol.pilot.tf.position - vtol.waypointPos).magnitude < 1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}