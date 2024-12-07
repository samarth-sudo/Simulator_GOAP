using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoUnit_FixedWing : DemoUnit
    {
        public DemoFixedWingPilot pilot;
        public GOAPDriver_DemoFixedWing goapDriver;

        private Vector3 spawnPoint;
        private Quaternion spawnRotation;

        public float spawnSpeed = 100f;

        public GOAPGoal_MoveTo defaultMoveGoal;

        protected override void Start()
        {
            defaultMoveGoal = Instantiate(defaultMoveGoal);

            base.Start();
            spawnPoint = transform.position;
            spawnRotation = transform.rotation;

            Respawn();
        }

        public override void Respawn()
        {
            transform.position = spawnPoint;
            transform.rotation = spawnRotation;

            pilot.rb.velocity = transform.forward * spawnSpeed;
            pilot.rb.angularVelocity = Vector3.zero;

            defaultMoveGoal.goalPos = spawnPoint;
            goapDriver.GoToGoal(defaultMoveGoal);
        }

        public Vector3 GetSpawnPos()
        {
            return spawnPoint;
        }

        public override void GetPYRT(out float pitch, out float yaw, out float roll, out float throttle)
        {
            pitch = -pilot.flightModel.pitchInput;
            yaw = pilot.flightModel.yawInput;
            roll = pilot.flightModel.rollInput;
            throttle = pilot.engine.throttle;
        }
    }
}