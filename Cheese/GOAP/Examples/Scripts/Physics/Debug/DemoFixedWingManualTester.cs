using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoFixedWingManualTester : MonoBehaviour
    {
        public DemoFixedWingFlightModel plane;
        public DemoFixedWingPilot pilot;
        public DemoJetEngine engine;
        public float targetSpeed;

        public bool flyKeyboard;
        public bool flyMouse;

        private void FixedUpdate()
        {

            if (flyKeyboard)
            {
                engine.throttle = engine.rb.velocity.magnitude < targetSpeed ? 1f : 0f;
                plane.pitchInput = -Input.GetAxis("Vertical");
                plane.rollInput = Input.GetAxis("Horizontal");
            }
            if (flyMouse)
            {
                pilot.targetVelocity = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized * targetSpeed;
            }

            Debug.Log($"Speed: {engine.rb.velocity.magnitude}");
        }
    }
}