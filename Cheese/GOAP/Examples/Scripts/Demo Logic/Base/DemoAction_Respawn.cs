using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_Respawn", menuName = "Demo/Action/Base/Respawn")]
    public class DemoAction_Respawn : DemoAction
    {
        public override void StartAction()
        {
            unit.Respawn();
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            return DemoSequence.SequenceStatus.Completed;
        }
    }
}