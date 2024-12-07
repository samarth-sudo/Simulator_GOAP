using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_SetRTBWaypoint", menuName = "Demo/Action/Base/SetRTBWaypoint")]
    public class DemoAction_SetRTBWaypoint : DemoAction
    {
        public override void StartAction()
        {
            unit.waypointPos = unit.spawnPoint;
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            return DemoSequence.SequenceStatus.Completed;
        }
    }
}