using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class EngineAnimation : MonoBehaviour
    {
        protected float rpm;

        public virtual void SetRpm(float rpm)
        {
            this.rpm = rpm;
        }
    }
}