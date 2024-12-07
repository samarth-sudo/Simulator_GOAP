using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_FixedWing_FlyToWaypoint", menuName = "Demo/Action/FixedWing/FlyToWaypoint")]
    public class DemoAction_FixedWing_FlyToWaypoint : DemoAction_FixedWing
    {
        public GOAPGoal_MoveTo overrideGoal;
        public bool startGoalImmediate;

        public float normalHeight = 30;
        private Vector3 targetPos;

        public override void StartAction()
        {
            targetPos = fixedWingUnit.waypointPos + fixedWingUnit.waypointNormal * normalHeight;
            if (overrideGoal != null)
            {
                GOAPGoal_MoveTo instantiatedGoal = Instantiate(overrideGoal);
                instantiatedGoal.startingPos = fixedWingUnit.goapDriver.tf.position;
                instantiatedGoal.goalPos = targetPos;
                fixedWingUnit.goapDriver.GoToGoal(instantiatedGoal, startGoalImmediate);
            }
            else
            {
                fixedWingUnit.goapDriver.GoToPos(targetPos, startGoalImmediate);
            }
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            return DemoSequence.SequenceStatus.Running;
        }
    }
}
