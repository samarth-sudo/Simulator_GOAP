namespace Cheese.GOAP.Aircraft
{
    [System.Serializable]
    public class SimpleAircraftSpecs
    {
        public float acceleration = 1f;

        public float minAltitude = 0f;
        public float maxAltitude = 1000f;

        public float maxDescentRate = 20f;
        public float maxClimbRate = 20f;
    }

    public class GOAPPathRequest_SimpleAircraft : GOAPPathRequest_Vehicle
    {
        public GOAPPathRequest_SimpleAircraft(
            IGOAPAgent agent,
            IGOAPGoal goal,
            float simStepTime,
            VehicleSpecs vehicleSpecs,
            SimpleAircraftSpecs specs) : base(agent, goal, simStepTime)
        {
            this.vehicleSpecs = vehicleSpecs;
            this.specs = specs;
        }

        public VehicleSpecs vehicleSpecs;
        public SimpleAircraftSpecs specs;
    }
}