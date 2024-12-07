using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoFixedWingPilot : MonoBehaviour
    {
        public Rigidbody rb;
        public DemoFixedWingFlightModel flightModel;
        public DemoJetEngine engine;

        public Vector3 targetVelocity;
        public Vector3 steerVector;

        public float rollResponse = 0.1f;
        public float maxAoA = 30f;

        public PIDController pitchPID;
        public PIDController rollPID;

        public PIDController aoaPID;

        public PIDController throttlePID;

        private void FixedUpdate()
        {
            Debug.DrawRay(transform.position, targetVelocity.normalized, Color.red);

            Vector3 localAngularVel = transform.InverseTransformDirection(rb.angularVelocity);

            engine.throttle = throttlePID.Evaluate(targetVelocity.magnitude, rb.velocity.magnitude, 0f, Time.fixedDeltaTime);


            Vector3 accelerationDir = (targetVelocity - rb.velocity) * rollResponse + Vector3.up * 9.81f;
            Vector3 rightVec = Vector3.Cross(rb.velocity, accelerationDir).normalized;
            Vector3 upVec = Vector3.Cross(rightVec, rb.velocity).normalized;

            flightModel.rollInput = rollPID.Evaluate(Vector3.Dot(transform.up, rightVec), 0f, -localAngularVel.z, Time.fixedDeltaTime);

            float angleToTarget = Mathf.Atan2(Vector3.Dot(upVec, targetVelocity.normalized), Vector3.Dot(rb.velocity.normalized, targetVelocity.normalized));

            Vector3 steerVector = Quaternion.AngleAxis(Mathf.Clamp(aoaPID.Evaluate(angleToTarget, 0f, 0f, Time.fixedDeltaTime), -1f, 1f) * maxAoA, rightVec) * rb.velocity.normalized;
            Debug.DrawRay(transform.position, steerVector);
            flightModel.pitchInput = pitchPID.Evaluate(rb.transform.InverseTransformDirection(steerVector).y, 0, localAngularVel.x, Time.fixedDeltaTime);
        }
    }
}