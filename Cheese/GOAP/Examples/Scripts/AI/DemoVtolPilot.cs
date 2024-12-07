using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoVtolPilot : MonoBehaviour
    {
        public DemoVtolEngines engines;
        public DemoFixedWingFlightModel fixedWing;
        public DemoAirbrake airbrake;

        public Rigidbody rb;
        public Transform tf;

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

        //vtol pid controllers
        public PIDController pitchPID;
        public PIDController yawPID;
        public PIDController rollPID;

        //fwd flight pid controllers
        public float rollResponse = 0.1f;
        public float maxAoA = 30f;

        public PIDController aoaPID;
        public PIDController throttlePID;

        public AnimationCurve vtolTransition;
        public AnimationCurve vtolEngineTransition;

        private void Start()
        {
            hoverTarget = transform.position;//hover where we spawn
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
            Vector3 dir = tf.position - orbitCenter;
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
            FlyVelocity((position - tf.position) * speedFactor + velocity, out Vector3 targetAcceleration, out Vector3 targetVelocity);

            Debug.DrawRay(tf.position, targetAcceleration, Color.red);
            Debug.DrawRay(tf.position, targetVelocity, Color.green);

            engines.targetAcceleration = targetAcceleration - Physics.gravity;
            FlyAttitude(targetAcceleration, aimDir, targetVelocity, 0f);
        }

        /// <summary>
        /// Internal function to make the AI fly at a certain airspeed
        /// </summary>
        /// <param name="velocity">velocity to fly at</param>
        private void FlyVelocity(Vector3 velocity, out Vector3 targetAcceleration, out Vector3 targetVelocity)
        {
            //clamp to max speed
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            targetVelocity = velocity;

            //accelerate to reach the target speed
            FlyAcceleration(accelerationPID.Evaluate(velocity, rb.velocity, Vector3.zero, Time.fixedDeltaTime), out targetAcceleration);
        }

        /// <summary>
        /// Internal function to make the AI accelerate in a direction 
        /// </summary>
        /// <param name="acceleration">the direction to accelerate in</param>
        private void FlyAcceleration(Vector3 acceleration, out Vector3 targetAcceleration)
        {
            acceleration = Vector3.ClampMagnitude(acceleration, maxAcceleration);

            targetAcceleration = acceleration;
        }

        /// <summary>
        /// Rotates the helicopter to face in the right direction
        /// </summary>
        /// <param name="thrustVector">the direction we want to apply thrust</param>
        /// <param name="aimVector">the direction we want to aim the nose</param>
        /// <param name="aimLerp">0 to pitch to match the thrust direction, 1 to pitch to match the aim direction</param>
        private void FlyAttitude(Vector3 thrustVector, Vector3 aimVector, Vector3 targetVelocity, float aimLerp)
        {
            Debug.DrawRay(transform.position, targetVelocity.normalized, Color.red);

            float throttle = throttlePID.Evaluate(targetVelocity.magnitude, rb.velocity.magnitude, 0f, Time.fixedDeltaTime);
            engines.throttle = throttle;
            airbrake.airbrakeInput = -throttle;

            Vector3 accelerationDir = (targetVelocity - rb.velocity) * rollResponse + Vector3.up * 9.81f;
            Vector3 rightVec = Vector3.Cross(rb.velocity, accelerationDir).normalized;
            Vector3 upVec = Vector3.Cross(rightVec, rb.velocity).normalized;

            //fixedWing.rollInput = rollPID.Evaluate(Vector3.Dot(transform.up, rightVec), 0f, -localAngularVel.z, Time.fixedDeltaTime);

            float angleToTarget = Mathf.Atan2(Vector3.Dot(upVec, targetVelocity.normalized), Vector3.Dot(rb.velocity.normalized, targetVelocity.normalized));

            Vector3 steerVector = Quaternion.AngleAxis(Mathf.Clamp(aoaPID.Evaluate(angleToTarget, 0f, 0f, Time.fixedDeltaTime), -1f, 1f) * maxAoA, rightVec) * rb.velocity.normalized;
            Debug.DrawRay(transform.position, steerVector, Color.white);
            //fixedWing.pitchInput = pitchPID.Evaluate(rb.transform.InverseTransformDirection(steerVector).y, 0, localAngularVel.x, Time.fixedDeltaTime);


            float vtolMode = Mathf.Clamp01(vtolTransition.Evaluate(rb.velocity.magnitude));
            float vtolEngineMode = Mathf.Clamp01(vtolEngineTransition.Evaluate(rb.velocity.magnitude));

            thrustVector -= Physics.gravity * (1f - vtolMode);
            Vector3 upTgt = Vector3.Slerp(thrustVector.normalized, upVec.normalized, vtolMode);
            Vector3 fwdTgt = Vector3.Slerp(aimVector.normalized, steerVector.normalized, vtolMode).normalized;

            Debug.DrawRay(transform.position, fwdTgt * 10f, Color.blue);
            Debug.DrawRay(transform.position, upTgt * 10f, Color.green);

            Vector3 localThrustVector = tf.InverseTransformDirection(upTgt.normalized);
            Vector3 localAimVector = tf.InverseTransformDirection(fwdTgt.normalized);

            Vector3 localAngularVel = tf.InverseTransformDirection(rb.angularVelocity);

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
            float pitch = pitchPID.Evaluate(Mathf.Lerp(localThrustVector.z, -localAimVector.y, vtolMode), 0, localAngularVel.x, Time.fixedDeltaTime);
            float roll = rollPID.Evaluate(localThrustVector.x, 0, localAngularVel.z, Time.fixedDeltaTime);
            float yaw = yawPID.Evaluate(localAimVector.x, 0, localAngularVel.y, Time.fixedDeltaTime);

            engines.pitch = pitch;
            engines.roll = roll;
            engines.yaw = yaw;
            engines.vtolRestriction = vtolEngineMode;

            fixedWing.pitchInput = pitch;
            fixedWing.rollInput = roll;
            fixedWing.yawInput = yaw;
        }
    }
}