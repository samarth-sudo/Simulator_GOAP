using UnityEngine;

namespace Cheese.GOAP.Demo
{

    public class PilotInfo : MonoBehaviour
    {
        public RectTransform pitchTf;
        public RectTransform rollTf;
        public RectTransform throttleTf;

        private float lastPitch;
        private float lastRoll;
        private float lastThrottle;

        private void Update()
        {
            if (DemoManager.instance.demoUnit == null)
                return;

            DemoManager.instance.demoUnit.GetPYRT(out float pitch, out _, out float roll, out float throttle);

            lastPitch = Mathf.Lerp(lastPitch, Mathf.Clamp(pitch, -1f, 1f), Time.deltaTime * 3f);
            pitchTf.localPosition = lastPitch * Vector2.up * 90f;

            lastRoll = Mathf.Lerp(lastRoll, Mathf.Clamp(roll, -1f, 1f), Time.deltaTime * 3f);
            rollTf.localPosition = Vector2.right * lastRoll * 90f;

            lastThrottle = Mathf.Lerp(lastThrottle, Mathf.Clamp(throttle, 0f, 1f), Time.deltaTime * 3f);
            throttleTf.localPosition = lastThrottle * Vector2.up * 180f;
        }
    }
}
