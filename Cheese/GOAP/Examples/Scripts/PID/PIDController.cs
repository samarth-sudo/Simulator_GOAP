using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [System.Serializable]
    public class PIDController
    {
        public float p = 1;
        public float i = 0;
        public float d = 1;

        float iValue = 0;
        public float iMin = -0.25f;
        public float iMax = 0.25f;

        public float Evaluate(float target, float current, float velocity, float deltaTime)
        {
            iValue += (target - current) * i * deltaTime;
            iValue = Mathf.Clamp(iValue, iMin, iMax);
            return (target - current) * p + -velocity * d + iValue;
        }
    }
}