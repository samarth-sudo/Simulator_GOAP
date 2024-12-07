using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoAction : ScriptableObject
    {
        public DemoUnit unit;

        public virtual void SetUp(DemoUnit unit)
        {
            this.unit = unit;
        }

        public virtual void StartAction()
        {

        }

        public virtual void UpdateAction()
        {

        }

        public virtual DemoSequence.SequenceStatus GetSequenceStatus()
        {
            return DemoSequence.SequenceStatus.Failed;
        }
    }
}