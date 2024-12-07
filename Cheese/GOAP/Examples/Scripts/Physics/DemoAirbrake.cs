using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoAirbrake : MonoBehaviour
    {
        public Rigidbody rb;

        public float airbrakeInput;
        public float airbrakeForce;

        private void FixedUpdate()
        {
            airbrakeInput = Mathf.Clamp01(airbrakeInput);
            rb.AddForce(rb.velocity * -airbrakeForce);
        }
    }
}
