using Cheese.GOAP.Aircraft;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class GOAPDriver_DemoFixedWing : GOAPDriver
    {
        public DemoFixedWingPilot pilot;
        public SimpleAircraftSpecs specs;

        protected override GOAPPathRequest_Vehicle GetRequest()
        {
            return new GOAPPathRequest_SimpleAircraft(this,
                goal,
                stepTime,
                vehicleSpecs,
                specs
                );
        }

        protected override Vector3 GetVelocity()
        {
            return pilot.rb.velocity;
        }

        protected override void MoveVelocity(Vector3 velocity)
        {
            pilot.targetVelocity = velocity;
        }

        protected override void TerminalNavigate()
        {
            // Airplanes can't come to a stop, so this should never get called
        }

        private bool avoidTerrain;
        private Vector3 avoidDir;
        private float raycastTimer;

        protected override void Panic()
        {
            // We don't have a path to help us naviagate right now! panic!

            if (pilot.rb.velocity.magnitude < vehicleSpecs.minSpeed)
            {
                RecoverStall();
            }
            else
            {
                RecoverAvoidAndClimb();
            }
        }

        private void RecoverStall()
        {
            pilot.targetVelocity = pilot.rb.velocity.normalized * vehicleSpecs.maxSpeed;
        }

        private void RecoverAvoidAndClimb()
        {
            // I'm gonna be honest, I don't trust this to save the aircraft
            // if it needs to do anything more to climb to avoid an obstacle...
            // this should be fine as it should be relying on the GOAP anyways

            // logic to spread out those expensive raycasts (also prevents jittering)
            raycastTimer -= Time.deltaTime;

            if (raycastTimer < 0f)
            {
                raycastTimer = 1f;

                RaycastHit hit;
                if (Physics.Raycast(tf.position, pilot.rb.velocity.normalized, out hit, vehicleSpecs.layerMask))
                {
                    avoidDir = Vector3.Reflect(pilot.rb.velocity.normalized, hit.normal);
                    avoidTerrain = true;
                }
                else
                {
                    avoidTerrain = false;
                }
            }

            if (avoidTerrain)
            {
                pilot.targetVelocity = avoidDir.normalized * vehicleSpecs.maxSpeed;
            }
            else
            {
                // Try and full power climb to get us out of this situation
                Vector3 tgtVelocity = pilot.rb.velocity;
                tgtVelocity.y = 0;
                tgtVelocity = tgtVelocity.normalized * vehicleSpecs.maxSpeed;

                if (tf.position.y > specs.maxAltitude)
                {
                    // If we are too high, we actually need to decend to a reasonable altitude
                    tgtVelocity += Vector3.down * vehicleSpecs.maxSpeed * 0.5f;
                }
                else
                {
                    tgtVelocity += Vector3.up * vehicleSpecs.maxSpeed * 0.5f;
                }
                pilot.targetVelocity = tgtVelocity;
            }
        }

        public override GOAPWorldState GetCurrentState()
        {
            return new GOAPWorldState_SimpleAircraft(tf, pilot.rb);
        }

        public override bool IsAlive()
        {
            return true;
        }
    }
}