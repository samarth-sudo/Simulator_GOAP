using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoJetEngine : MonoBehaviour
    {
        public Rigidbody rb;
        public float TWR = 0.8f;

        public float throttle;

        private void FixedUpdate()
        {
            throttle = Mathf.Clamp01(throttle);

            rb.AddForce(transform.forward * throttle * TWR * 9.81f, ForceMode.Acceleration);
        }
    }
}