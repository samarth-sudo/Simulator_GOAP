using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Helicopter_FlyToWaypoint", menuName = "Demo/Action/Helicopter/FlyToWaypoint")]
    public class DemoAction_Helicopter_FlyToWaypoint : DemoAction_Helicopter
    {
        public GOAPGoal_MoveTo overrideGoal;
        public bool startGoalImmediate = true;

        public float normalHeight = 30;
        private Vector3 targetPos;

        public override void StartAction()
        {
            targetPos = helicopter.waypointPos + helicopter.waypointNormal * normalHeight;
            if (overrideGoal != null)
            {
                GOAPGoal_MoveTo instantiatedGoal = Instantiate(overrideGoal);
                instantiatedGoal.startingPos = helicopter.goapDriver.tf.position;
                instantiatedGoal.goalPos = targetPos;
                helicopter.goapDriver.GoToGoal(instantiatedGoal, startGoalImmediate);
            }
            else
            {
                helicopter.goapDriver.GoToPos(targetPos, startGoalImmediate);
            }
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            if ((helicopter.physics.tf.position - helicopter.waypointPos).magnitude < 1f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}