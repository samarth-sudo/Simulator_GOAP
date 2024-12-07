using UnityEngine;

namespace Cheese.GOAP.Demo
{
    /// <summary>
    /// The autopilot script for controlling the helicopter
    /// </summary>
    public class DemoHelicopterPilot : MonoBehaviour
    {
        public DemoHelicopterPhysics helicopter;
        public Vector3 hoverTarget;
        public Vector3 velocityTarget;
        public Vector3 aimDir;

        [Tooltip("top speed of the helicopter")]
        public float maxSpeed;
        [Tooltip("max acceleration of the helicopter, limits how far it can tilt")]
        public float maxAcceleration;

        [Tooltip("how quickly the helicopter aproaches the target position")]
        public float speedFactor;
        [Tooltip("pid controller for matching the target speed")]
        public PID3Controller accelerationPID;

        //pid controllers
        public PIDController throttlePID;
        public PIDController pitchPID;
        public PIDController yawPID;
        public PIDController rollPID;

        [Range(0, 1)]
        [Tooltip("0 is pitch matches thrust direction, 1 is pitch matches aim direction")]
        public float aimLerp;

        private void Start()
        {
            hoverTarget = helicopter.tf.position;//hover where we spawn
        }

        private void FixedUpdate()
        {
            FlyPosition(hoverTarget, velocityTarget);
        }

        /// <summary>
        /// Makes the aircraft hover around a point, needs to be called every fixed update
        /// </summary>
        /// <param name="orbitCenter">the center of the orbit</param>
        /// <param name="altitude">the altitude relative to the orbit center</param>
        /// <param name="radius">the radius to orbit at</param>
        /// <param name="speed">the speed to orbit at</param>
        /// <returns>the direction from the center of the orbit to our current position</returns>
        public Vector3 OrbitPosition(Vector3 orbitCenter, float altitude, float radius, float speed)
        {
            Vector3 dir = helicopter.tf.position - orbitCenter;
            dir.y = 0;

            Vector3 orbitPos = orbitCenter + dir.normalized * radius + altitude * Vector3.up;
            Vector3 orbitVelocity = Vector3.Cross(Vector3.up, dir.normalized) * speed;

            SetTargetPosition(orbitPos, orbitVelocity);
            return dir.normalized;
        }

        /// <summary>
        /// Sets the target position and velocity to hover at
        /// </summary>
        /// <param name="position">position to hover at</param>
        /// <param name="velocity">veclocity at that position</param>
        public void SetTargetPosition(Vector3 position, Vector3 velocity)
        {
            hoverTarget = position;
            velocityTarget = velocity;
        }

        /// <summary>
        /// Sets the direction the helicopter should point its nose
        /// </summary>
        /// <param name="dir">the direction to aim in</param>
        public void SetAimDir(Vector3 dir)
        {
            aimDir = dir;
        }

        /// <summary>
        /// Internal function to make the AI fly at a position and velocity
        /// </summary>
        /// <param name="position">position to fly at</param>
        /// <param name="velocity">velocity to fly at</param>
        private void FlyPosition(Vector3 position, Vector3 velocity)
        {
            //fly towards the target positon
            FlyVelocity((position - helicopter.tf.position) * speedFactor + velocity);
        }

        /// <summary>
        /// Internal function to make the AI fly at a certain airspeed
        /// </summary>
        /// <param name="velocity">velocity to fly at</param>
        private void FlyVelocity(Vector3 velocity)
        {
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);//clamp to max speed
                                                                  //Debug.DrawRay(helicopter.tf.position, velocity, Color.red);
                                                                  //accelerate to reach the target speed
            FlyAcceleration(accelerationPID.Evaluate(velocity, helicopter.rb.velocity, Vector3.zero, Time.fixedDeltaTime));
        }

        /// <summary>
        /// Internal function to make the AI accelerate in a direction 
        /// </summary>
        /// <param name="acceleration">the direction to accelerate in</param>
        private void FlyAcceleration(Vector3 acceleration)
        {
            acceleration = Vector3.ClampMagnitude(acceleration, maxAcceleration);
            acceleration -= Physics.gravity;
            //Debug.DrawRay(helicopter.tf.position, acceleration, Color.yellow);
            //Debug.DrawRay(helicopter.tf.position, helicopter.tf.up * acceleration.magnitude, Color.cyan);
            Vector3 normalisedAcceleration = acceleration.normalized;

            //set the collective to produce the right thrust to accelerate in that direction
            float collective = throttlePID.Evaluate(acceleration.magnitude - Physics.gravity.magnitude, 0, 0, Time.fixedDeltaTime);
            helicopter.collective = helicopter.idleCollective + collective;

            //rotate the helicopter to accelerate in the right direction
            FlyAttitude(normalisedAcceleration, aimDir, aimLerp);
        }

        /// <summary>
        /// Rotates the helicopter to face in the right direction
        /// </summary>
        /// <param name="thrustVector">the direction we want to apply thrust</param>
        /// <param name="aimVector">the direction we want to aim the nose</param>
        /// <param name="aimLerp">0 to pitch to match the thrust direction, 1 to pitch to match the aim direction</param>
        private void FlyAttitude(Vector3 thrustVector, Vector3 aimVector, float aimLerp)
        {
            Vector3 localThrustVector = helicopter.tf.InverseTransformDirection(thrustVector.normalized);
            Vector3 localAimVector = helicopter.tf.InverseTransformDirection(aimVector.normalized);

            Vector3 localAngularVel = helicopter.tf.InverseTransformDirection(helicopter.rb.angularVelocity);

            //Debug.DrawRay(helicopter.tf.position, helicopter.tf.forward * localAngularVel.x * 10, Color.blue);

            //turn fast if the target is behind us
            if (localAimVector.z < 0)
            {
                if (localAimVector.x > 0)
                {
                    localAimVector = Vector3.right;
                }
                else
                {
                    localAimVector = Vector3.left;
                }
            }

            //update pids and calculate PYR
            float pitch = pitchPID.Evaluate(Mathf.Lerp(localThrustVector.z, -localAimVector.y, aimLerp), 0, localAngularVel.x, Time.fixedDeltaTime);
            float roll = rollPID.Evaluate(localThrustVector.x, 0, localAngularVel.z, Time.fixedDeltaTime);
            float yaw = yawPID.Evaluate(localAimVector.x, 0, localAngularVel.y, Time.fixedDeltaTime);

            helicopter.pitch = pitch;
            helicopter.roll = roll;
            helicopter.yaw = yaw;
        }
    }
}