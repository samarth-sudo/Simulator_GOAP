using Cheese.GOAP.Aircraft;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class GOAPDriver_DemoVtol : GOAPDriver
    {
        public DemoVtolPilot pilot;
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
            pilot.SetTargetPosition(tf.position, velocity);
            if (pilot.rb.velocity.magnitude > 10)
            {
                pilot.SetAimDir(pilot.rb.velocity);
            }
        }

        protected override void TerminalNavigate()
        {
            pilot.SetTargetPosition(target, Vector3.zero);
        }

        protected override void Panic()
        {
            //we don't have a path to help us naviagate right now! panic!
            if (tf.up.y < 0f)
            {
                // we are inverted, we need to recover from this before we can do anything else
                PanicRecover();
            }
            if (pilot.rb.velocity.magnitude > crawlSpeed)
            {
                // Try to come to a stop to avoid colliding with anything
                PanicStop();
            }
            else
            {
                // We have come to a stop, we might be unable to find a new path
                // Due to being to close to a wall, we should search for nearby obstacles and crawl away
                PanicCrawl();
            }
        }

        private void PanicRecover()
        {
            pilot.SetTargetPosition(tf.position, Vector3.zero);
        }


        private void PanicStop()
        {
            // I planned to implement obstacle avoidance here also, but it seems that this is unecessary
            // Most of the time it finds itself without a path, coming to a stop tends to be safe enough
            pilot.SetTargetPosition(tf.position, Vector3.zero);
        }

        private float raycastTimer;
        private Vector3 avoidPos;

        private void PanicCrawl()
        {
            raycastTimer -= Time.deltaTime;

            Vector3 offset = tf.position - avoidPos;
            if (raycastTimer < 0f)
            {
                raycastTimer = 0.1f;

                RaycastHit hit;
                if (Physics.Raycast(tf.position, Random.onUnitSphere, out hit, vehicleSpecs.radius * 2f, vehicleSpecs.layerMask))
                {
                    Vector3 newOffset = tf.position - hit.point;
                    if (newOffset.magnitude < offset.magnitude)
                    {
                        offset = newOffset;
                        avoidPos = hit.point;
                    }
                }
            }

            if (offset.magnitude < vehicleSpecs.radius * 2f)
            {
                pilot.SetTargetPosition(tf.position, offset.normalized * crawlSpeed);
            }
        }
        public override bool IsAlive()
        {
            return true;
        }

        public override GOAPWorldState GetCurrentState()
        {
            return new GOAPWorldState_SimpleAircraft(tf, pilot.rb);
        }
    }
}