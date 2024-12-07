using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoUnit_Vtol : DemoUnit
    {
        public DemoVtolPilot pilot;
        public GOAPDriver_DemoVtol goapDriver;

        public Transform groundCheckTf;

        public bool IsFlying()
        {
            return pilot.rb.velocity.magnitude > 5f || Physics.Raycast(groundCheckTf.position, Vector3.down, 5f) == false;
        }

        protected override void Start()
        {
            base.Start();
            spawnPoint = transform.position;
            spawnRotation = transform.rotation;

            Respawn();
        }

        public override void Respawn()
        {
            transform.position = spawnPoint;
            transform.rotation = spawnRotation;

            pilot.rb.velocity = Vector3.zero;
            pilot.rb.angularVelocity = Vector3.zero;
        }

        public Vector3 GetSpawnPos()
        {
            return spawnPoint;
        }

        public override void GetPYRT(out float pitch, out float yaw, out float roll, out float throttle)
        {
            pitch = -pilot.fixedWing.pitchInput;
            yaw = pilot.fixedWing.yawInput;
            roll = pilot.fixedWing.rollInput;
            throttle = pilot.engines.GetFinalThrottle();
        }
    }
}