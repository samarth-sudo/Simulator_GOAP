using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Helicopter_LandAtWaypoint", menuName = "Demo/Action/Helicopter/LandAtWaypoint")]
    public class DemoAction_Helicopter_LandAtWaypoint : DemoAction_Helicopter
    {
        public GOAPGoal_MoveTo overrideGoal;
        public bool startGoalImmediate = true;

        public float landingApproachHeight = 5;
        private Vector3 targetPos;

        public override void StartAction()
        {
            targetPos = helicopter.waypointPos + Vector3.up * landingApproachHeight;
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
            if ((helicopter.physics.tf.position - targetPos).magnitude < 1f)
                helicopter.goapDriver.GoToPosTerminal(helicopter.waypointPos);

            if ((helicopter.physics.tf.position - helicopter.waypointPos).magnitude < 3f)
                return DemoSequence.SequenceStatus.Completed;

            return DemoSequence.SequenceStatus.Running;
        }
    }
}