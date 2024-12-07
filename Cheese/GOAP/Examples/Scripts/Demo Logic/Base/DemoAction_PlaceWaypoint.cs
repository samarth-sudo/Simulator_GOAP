using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [CreateAssetMenu(fileName = "Action_PlaceWaypoint", menuName = "Demo/Action/Base/PlaceWaypoint")]
    public class DemoAction_PlaceWaypoint : DemoAction
    {
        public bool placeOnFlat;

        public override void StartAction()
        {
            DemoWaypointPlacer.instance.StartWaypointPlacement(placeOnFlat);
        }

        public override DemoSequence.SequenceStatus GetSequenceStatus()
        {
            switch (DemoWaypointPlacer.instance.placingState)
            {
                case DemoWaypointPlacer.PlacingState.None:
                case DemoWaypointPlacer.PlacingState.Failed:
                case DemoWaypointPlacer.PlacingState.Canceled:
                    return DemoSequence.SequenceStatus.Failed;
                case DemoWaypointPlacer.PlacingState.Placed:
                    unit.waypointPos = DemoWaypointPlacer.instance.placedPos;
                    unit.waypointNormal = DemoWaypointPlacer.instance.placedNormal;
                    return DemoSequence.SequenceStatus.Completed;
                case DemoWaypointPlacer.PlacingState.Placing:
                    return DemoSequence.SequenceStatus.Running;
            }
            return DemoSequence.SequenceStatus.Failed;
        }
    }
}