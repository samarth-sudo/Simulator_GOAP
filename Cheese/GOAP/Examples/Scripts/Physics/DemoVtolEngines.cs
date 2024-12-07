using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoVtolEngines : MonoBehaviour
    {
        public Rigidbody rb;
        public Transform tf;

        public Transform tiltRotorTf;
        private float currentAngle;
        public float maxPitch;
        public float minPitch;
        public float maxRotateSpeed;

        public bool engineOn;
        public float spoolSpeed = 10f;
        public float rpm;

        public float maxThrust;
        public float rotorTorque;

        public Vector3 targetAcceleration;
        public float throttle;
        public float vtolRestriction;
        private float finalThrottle;

        //the cyclic inputs
        [Range(-1, 1)]
        public float pitch;
        [Range(-1, 1)]
        public float yaw;
        [Range(-1, 1)]
        public float roll;

        public EngineAnimation[] animations;

        private void Start()
        {
            if (engineOn)
            {
                StartEngineImmediate();
            }
        }

        public void StartEngine()
        {
            engineOn = true;
        }

        public void StartEngineImmediate()
        {
            engineOn = true;
            rpm = 1f;
        }

        public void ShutdownEngine()
        {
            engineOn = false;
        }

        public void ShutdownEngineImmediate()
        {
            engineOn = false;
            rpm = 0;
        }

        private void FixedUpdate()
        {
            rpm += (engineOn ? spoolSpeed : -spoolSpeed) * Time.fixedDeltaTime;
            rpm = Mathf.Clamp01(rpm);

            //clamp the inputs so they are not out of range
            throttle = Mathf.Clamp01(throttle);
            vtolRestriction = Mathf.Clamp01(vtolRestriction);

            pitch = Mathf.Clamp(pitch, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);


            Vector3 flattenedTargetAcceleration = Vector3.ProjectOnPlane(targetAcceleration, tf.right);
            float targetAngle = Vector3.SignedAngle(tf.forward, flattenedTargetAcceleration, tf.right);

            targetAngle = Mathf.Clamp(targetAngle, Mathf.Lerp(minPitch, 0, vtolRestriction), Mathf.Lerp(maxPitch, 0f, vtolRestriction));
            currentAngle += Mathf.Clamp(targetAngle - currentAngle, -maxRotateSpeed * Time.fixedDeltaTime, maxRotateSpeed * Time.fixedDeltaTime);

            tiltRotorTf.localEulerAngles = Vector3.right * currentAngle;

            float vtolThrottle = Vector3.Dot(tiltRotorTf.forward, targetAcceleration) * rb.mass / maxThrust;
            vtolThrottle = Mathf.Clamp01(vtolThrottle);


            //apply rotor blade thrust
            finalThrottle = Mathf.Lerp(vtolThrottle, throttle, vtolRestriction);
            rb.AddForce(tiltRotorTf.forward * rpm * maxThrust * finalThrottle);

            //apply pitch roll and yaw torque
            Vector3 pitchRoll = new Vector3(roll, pitch);
            pitchRoll = Vector3.ClampMagnitude(pitchRoll, 1f);

            rb.AddRelativeTorque((-Vector3.forward * pitchRoll.x
                + -Vector3.right * pitchRoll.y
                + Vector3.up * yaw) * rotorTorque * rpm * (1f - vtolRestriction));


            foreach (EngineAnimation animation in animations)
            {
                animation.SetRpm(rpm);
            }
        }

        public float GetFinalThrottle()
        {
            return finalThrottle;
        }
    }
}