using UnityEngine;

namespace Cheese.GOAP
{

    [CreateAssetMenu(fileName = "GOAPGoal_MoveTo_Position", menuName = "Cheese/GOAP/Goal/MoveTo_Position")]
    public class GOAPGoal_MoveTo_Position : GOAPGoal_MoveTo
    {
        //settings
        [Tooltip("The how close does the agent have to be to the goal to count as completed.")]
        public float completionRadius;
        [Tooltip("The how slow does the agent have to be to count as completed.")]
        public float completionVelocity;

        //weights
        [Tooltip("The reward weight of moving towards the goal (positive values enourage moving towards goal)")]
        public float goalWeight;
        [Tooltip("The reward weight of moving away from the start point (positive values enourage moving away from the start)")]
        public float startWeight;
        [Tooltip("The reward weight of moving at high speeds(positive values enourage moving faster)")]
        public float velocityWeight;
        [Tooltip("The reward weight of finding a long path (positive values encourage generating longer paths)")]
        public float timeWeight;

        public override float GetValue(GOAPWorldState state)
        {
            GOAPWorldState_Vehicle vehicleState = state as GOAPWorldState_Vehicle;

            Vector3 goalVec = goalPos - vehicleState.position;

            return base.GetValue(state)
                + goalVec.magnitude * -goalWeight
                + (startingPos - vehicleState.position).magnitude * startWeight
                + vehicleState.velocity.magnitude * velocityWeight
                + vehicleState.timeSinceBegining * timeWeight;
        }

        public override bool GetIsGoal(GOAPWorldState state)
        {
            GOAPWorldState_Vehicle vehicleState = state as GOAPWorldState_Vehicle;
            return (vehicleState.position - goalPos).magnitude < completionRadius
                && vehicleState.velocity.magnitude < completionVelocity;
        }
    }
}