using UnityEngine;

namespace Cheese.GOAP
{
    [CreateAssetMenu(fileName = "GOAPGoal_FollowTerrain", menuName = "Cheese/GOAP/Goal/FollowTerrain")]
    public class GOAPGoal_FollowTerrain : GOAPGoalSO
    {
        public float minAlt = 50f;
        public float maxAlt = 100f;

        public float maxDecentSpeed = 10f;
        public float maxClimbSpeed = 10f;

        public float altitudeFactor = 1f;
        public float verticalSpeedPenalty = 0.1f;

        public LayerMask terrainLayer;

        public override float GetValue(GOAPWorldState state)
        {
            GOAPWorldState_Vehicle vehicleState = state as GOAPWorldState_Vehicle;

            //use alt above water as a fallback
            float alt = vehicleState.position.y;
            RaycastHit hit;
            if (Physics.Raycast(vehicleState.position, Vector3.down, out hit, terrainLayer))
            {
                alt = hit.distance;
            }

            float targetVeticalSpeed;
            if (alt < minAlt)
            {
                targetVeticalSpeed = Mathf.Clamp((minAlt - alt) * altitudeFactor, 0, maxClimbSpeed);
            }
            else if (alt > maxAlt)
            {
                targetVeticalSpeed = -Mathf.Clamp((alt - maxAlt) * altitudeFactor, 0, maxDecentSpeed);
            }
            else
            {
                return base.GetValue(state);
            }

            return base.GetValue(state) - Mathf.Abs(targetVeticalSpeed - vehicleState.velocity.y) * verticalSpeedPenalty;
        }
    }
}