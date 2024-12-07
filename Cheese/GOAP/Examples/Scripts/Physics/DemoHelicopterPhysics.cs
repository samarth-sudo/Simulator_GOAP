using UnityEngine;

namespace Cheese.GOAP.Demo
{
    /// <summary>
    /// The flight model used by the attack helicopter
    /// </summary>
    public class DemoHelicopterPhysics : MonoBehaviour
    {
        public Rigidbody rb;
        public Transform tf;

        public float maxRpm = 500;
        public float rpm;
        public float rotorForce;
        public float rotorTorque;
        public float rotorYawTorque;


        //the collective inputs
        public float idleCollective;
        [Range(0, 1)]
        public float collective;

        //the cyclic inputs
        [Range(-1, 1)]
        public float pitch;
        [Range(-1, 1)]
        public float yaw;
        [Range(-1, 1)]
        public float roll;

        public bool engineOn;
        public float spoolSpeed = 10f;

        public EngineAnimation[] animations;

        private void Start()
        {
            idleCollective = rb.mass * 9.81f / (rotorForce * maxRpm);//calculates the ammount of collecitve needed to hover perfectly
            collective = idleCollective;

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
            rpm = maxRpm;
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
            rpm = Mathf.Clamp(rpm, 0, maxRpm);

            //clamp the inputs so they are not out of range
            collective = Mathf.Clamp(collective, 0, 1);

            pitch = Mathf.Clamp(pitch, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);

            //apply rotor blade thrust
            rb.AddForce(tf.up * rpm * rotorForce * collective);

            //apply pitch roll and yaw torque
            Vector3 pitchRoll = new Vector3(roll, pitch);
            pitchRoll = Vector3.ClampMagnitude(pitchRoll, 1);

            rb.AddRelativeTorque((-Vector3.forward * pitchRoll.x + -Vector3.right * pitchRoll.y) * rotorTorque * rpm);
            rb.AddRelativeTorque((Vector3.up * yaw) * rotorYawTorque * rpm);

            foreach (EngineAnimation animation in animations)
            {
                animation.SetRpm(rpm);
            }
        }
    }
}