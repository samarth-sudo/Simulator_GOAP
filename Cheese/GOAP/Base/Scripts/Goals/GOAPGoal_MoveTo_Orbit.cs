using UnityEngine;

namespace Cheese.GOAP
{

    [CreateAssetMenu(fileName = "GOAPGoal_MoveTo_Orbit", menuName = "Cheese/GOAP/Goal/MoveTo_Orbit")]
    public class GOAPGoal_MoveTo_Orbit : GOAPGoal_MoveTo
    {
        public enum OrbitDirection
        {
            Both,
            Clockwise,
            Anticlockwise
        }

        public float timeWeight = 1f;

        public OrbitDirection direction;

        public float targetSpeed = 50f;
        public float onSpeedReward = 0.1f;

        public float minRadius = 250f;
        public float maxRadius = 750f;
        public float radiusPenalty = 1f;

        public float minAlt = 50f;
        public float maxAlt = 100f;
        public float altitudePenalty = 1f;

        public override float GetValue(GOAPWorldState state)
        {
            GOAPWorldState_Vehicle vehicleState = state as GOAPWorldState_Vehicle;

            Vector3 goalVec = goalPos - vehicleState.position;
            goalVec.y = 0;
            Vector3 orbitDir = Vector3.Cross(goalVec.normalized, Vector3.up).normalized;

            switch (direction)
            {
                case OrbitDirection.Clockwise:
                    break;
                case OrbitDirection.Anticlockwise:
                    orbitDir = -orbitDir;
                    break;
                default:
                case OrbitDirection.Both:
                    if (Vector3.Dot(orbitDir, vehicleState.velocity) < 0)
                    {
                        orbitDir = -orbitDir;
                    }
                    break;
            }

            float value = base.GetValue(vehicleState);

            value += vehicleState.timeSinceBegining * timeWeight;
            value -= Mathf.Abs(Vector3.Dot(orbitDir, vehicleState.velocity) - targetSpeed) * onSpeedReward;

            float currentRadius = goalVec.magnitude;
            if (currentRadius < minRadius)
            {
                value -= (minRadius - currentRadius) * radiusPenalty;
            }
            if (currentRadius > maxRadius)
            {
                value -= (currentRadius - maxRadius) * radiusPenalty;
            }

            float currentAltitude = vehicleState.position.y - goalPos.y;
            if (currentAltitude < minRadius)
            {
                value -= (minAlt - currentAltitude) * altitudePenalty;
            }
            if (currentAltitude > maxRadius)
            {
                value -= (currentAltitude - maxAlt) * altitudePenalty;
            }

            return value;
        }
    }
}