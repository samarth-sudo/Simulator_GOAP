using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoUnit_Helicopter : DemoUnit
    {
        public DemoHelicopterPhysics physics;
        public DemoHelicopterPilot pilot;
        public GOAPDriver_DemoHelicopter goapDriver;

        public Transform groundCheckTf;

        public bool IsFlying()
        {
            return physics.rb.velocity.magnitude > 5f || Physics.Raycast(groundCheckTf.position, Vector3.down, 5f) == false;
        }

        protected override void Start()
        {
            base.Start();
            spawnPoint = transform.position;
            spawnRotation = transform.rotation;
        }

        public void Respawn()
        {
            transform.position = spawnPoint;
            transform.rotation = spawnRotation;

            physics.rb.velocity = Vector3.zero;
            physics.rb.angularVelocity = Vector3.zero;

            physics.ShutdownEngineImmediate();
        }

        public Vector3 GetSpawnPos()
        {
            return spawnPoint;
        }

        public override void GetPYRT(out float pitch, out float yaw, out float roll, out float throttle)
        {
            pitch = -physics.pitch;
            yaw = physics.yaw;
            roll = physics.roll;
            throttle = physics.collective;
        }
    }
}