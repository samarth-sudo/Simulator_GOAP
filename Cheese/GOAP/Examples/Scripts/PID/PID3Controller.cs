using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [System.Serializable]
    public class PID3Controller
    {
        public float p = 1;
        public float i = 0;
        public float d = 1;

        Vector3 iValue;
        public float iMax = 0.25f;

        public Vector3 Evaluate(Vector3 target, Vector3 current, Vector3 velocity, float deltaTime)
        {
            iValue += (target - current) * i * deltaTime;
            if (iValue.magnitude > iMax)
            {
                iValue = iValue.normalized * iMax;
            }
            return (target - current) * p + -velocity * d + iValue;
        }
    }
}