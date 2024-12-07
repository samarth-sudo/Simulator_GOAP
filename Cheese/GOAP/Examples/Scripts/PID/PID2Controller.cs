using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [System.Serializable]
    public class PID2Controller
    {
        public float p = 1;
        public float i = 0;
        public float d = 1;

        Vector2 iValue;
        public float iMax = 0.25f;

        public Vector2 Evaluate(Vector2 target, Vector2 current, Vector2 velocity, float deltaTime)
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