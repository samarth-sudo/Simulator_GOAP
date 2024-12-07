using UnityEngine;

namespace Cheese.GOAP
{
    [System.Serializable]
    public class VehicleSpecs
    {
        public LayerMask layerMask;

        public float radius = 5f;

        public float maxSpeed = 10f;
        public float minSpeed = 0f;
    }

    public class GOAPPathRequest_Vehicle : GOAPPathRequest
    {
        public GOAPPathRequest_Vehicle(IGOAPAgent agent,
            IGOAPGoal goal,
            float simStepTime) : base(agent, goal, simStepTime)
        {

        }
    }
}
