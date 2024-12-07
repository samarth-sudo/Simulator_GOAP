using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoFixedWingFlightModel : MonoBehaviour
    {
        public Rigidbody rb;

        public float length;
        public float width;
        public float liftCo;
        public float dragCo;

        private float lift;
        public AnimationCurve liftCurve;
        private float drag;
        public AnimationCurve dragCurve;
        public float baseDrag = 0.05f;

        public float maxElevatorAngle;
        public float elevatorTorqueMax;
        public float elevatorDragMax;

        public float rollTorqueMax;
        public float rollDragMax;

        public float pitchInput;
        public float yawInput;
        public float rollInput;

        private void Start()
        {
            lift = liftCo * length * width;
            drag = dragCo * length * width;
        }

        private void FixedUpdate()
        {
            Vector3 velocity = rb.velocity;

            Vector3 localVelocity = rb.transform.InverseTransformDirection(velocity);

            ApplyWingForce();
            ApplyControlSurfaceTorque(localVelocity, Vector3.right, Mathf.Clamp(pitchInput, -1f, 1f) * maxElevatorAngle);
            ApplyControlSurfaceTorque(localVelocity, Vector3.up, Mathf.Clamp(yawInput, -1f, 1f));
            ApplyRollTorque(Mathf.Clamp(rollInput, -1f, 1f));
        }

        private void ApplyWingForce()
        {
            Vector3 velocity = rb.velocity;

            Vector3 localVelocity = rb.transform.InverseTransformDirection(velocity);

            Vector3 wingNormal = rb.transform.up;
            Vector3 wingRight = Vector3.Cross(wingNormal, rb.velocity);
            Vector3 liftNormal = Vector3.Cross(rb.velocity, wingRight).normalized;
            if (localVelocity.y > 0)
            {
                liftNormal *= -1;
            }
            float aoa = 90 - Vector3.Angle(localVelocity, Vector3.up);

            rb.AddForce(liftNormal * Mathf.Pow(velocity.magnitude, 2f) * lift * liftCurve.Evaluate(Mathf.Abs(aoa)));
            rb.AddForce(-velocity.normalized * Mathf.Pow(velocity.magnitude, 2f) * drag * (baseDrag + dragCurve.Evaluate(Mathf.Abs(aoa))));

            //Debug.DrawRay(rb.transform.position, liftNormal * Mathf.Pow(velocity.magnitude, 2) * lift * liftCurve.Evaluate(Mathf.Abs(aoa)) * 1f / rb.mass, Color.green);
            //Debug.DrawRay(rb.transform.position, -velocity.normalized * Mathf.Pow(velocity.magnitude, 2) * drag * (baseDrag + dragCurve.Evaluate(Mathf.Abs(aoa))) * 1f / rb.mass, Color.red);
        }

        private void ApplyControlSurfaceTorque(Vector3 localVelocity, Vector3 torqueAxis, float controlSurfaceAngle)
        {
            float aoa = 90 - Vector3.Angle(localVelocity, Vector3.Cross(Vector3.forward, torqueAxis).normalized) + controlSurfaceAngle;

            if (aoa > 0)
            {
                torqueAxis *= -1;
            }

            float torque = elevatorTorqueMax * Mathf.Pow(localVelocity.magnitude, 2f) * liftCurve.Evaluate(Mathf.Abs(aoa))
                + -elevatorDragMax * Mathf.Pow(rb.velocity.magnitude, 2) * Vector3.Dot(transform.InverseTransformDirection(rb.angularVelocity), torqueAxis);
            rb.AddRelativeTorque(torqueAxis * torque);
        }

        private void ApplyRollTorque(float rollInput)
        {
            rollInput = Mathf.Clamp(rollInput, -1f, 1f);
            float torque = -rollTorqueMax * Mathf.Pow(rb.velocity.magnitude, 2) * rollInput
                + -rollDragMax * Mathf.Pow(rb.velocity.magnitude, 2) * transform.InverseTransformDirection(rb.angularVelocity).z;
            rb.AddRelativeTorque(Vector3.forward * torque);
        }

        private void OnDrawGizmosSelected()
        {
            if (rb == null)
                return;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(rb.transform.position, rb.transform.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, 0.1f, length));
        }
    }
}