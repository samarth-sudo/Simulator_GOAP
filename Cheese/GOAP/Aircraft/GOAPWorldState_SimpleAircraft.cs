using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP.Aircraft
{
    public class GOAPWorldState_SimpleAircraft : GOAPWorldState_Vehicle
    {
        public GOAPWorldState_SimpleAircraft() : base()
        {

        }

        public GOAPWorldState_SimpleAircraft(Vector3 position, Vector3 velocity) : base(position, velocity)
        {

        }

        public GOAPWorldState_SimpleAircraft(Transform tf, Rigidbody rb) : base(tf, rb)
        {

        }

        public GOAPWorldState_SimpleAircraft(Rigidbody rb) : base(rb)
        {

        }

        public GOAPWorldState_SimpleAircraft(GOAPWorldState_SimpleAircraft stateToCopy) : base(stateToCopy)
        {

        }

        public override bool IsValidStartPoint()
        {
            GOAPPathRequest_SimpleAircraft aircraftRequest = request as GOAPPathRequest_SimpleAircraft;
            return Physics.OverlapSphere(position, aircraftRequest.vehicleSpecs.radius, aircraftRequest.vehicleSpecs.layerMask).Length <= 0;
        }

        public override bool ExploreFutureStates(ref List<GOAPWorldState> openNodes, float simStepTime)
        {
            GOAPPathRequest_SimpleAircraft aircraftRequest = request as GOAPPathRequest_SimpleAircraft;

            float velocityChange = aircraftRequest.specs.acceleration;
            bool goalFound = false;
            ExploreFutureState(aircraftRequest, ref openNodes, Vector3.zero * velocityChange, simStepTime, ref goalFound);
            ExploreFutureState(aircraftRequest, ref openNodes, Vector3.forward * velocityChange, simStepTime, ref goalFound);
            ExploreFutureState(aircraftRequest, ref openNodes, Vector3.left * velocityChange, simStepTime, ref goalFound);
            ExploreFutureState(aircraftRequest, ref openNodes, Vector3.right * velocityChange, simStepTime, ref goalFound);
            ExploreFutureState(aircraftRequest, ref openNodes, Vector3.back * velocityChange, simStepTime, ref goalFound);
            if (velocity.y < aircraftRequest.specs.maxClimbRate)
            {
                ExploreFutureState(aircraftRequest, ref openNodes, Vector3.up * velocityChange, simStepTime, ref goalFound);
            }
            if (velocity.y > -aircraftRequest.specs.maxDescentRate)
            {
                ExploreFutureState(aircraftRequest, ref openNodes, Vector3.down * velocityChange, simStepTime, ref goalFound);
            }

            return goalFound;
        }

        public void ExploreFutureState(GOAPPathRequest_SimpleAircraft aircraftRequest, ref List<GOAPWorldState> openNodes, Vector3 acceleration, float time, ref bool goalFound)
        {
            if (dead)
                return;

            GOAPWorldState_SimpleAircraft newState = new GOAPWorldState_SimpleAircraft(this);

            newState.velocity += acceleration * time;

            newState.velocity = Vector3.ClampMagnitude(newState.velocity, aircraftRequest.vehicleSpecs.maxSpeed);
            if (newState.velocity.magnitude < aircraftRequest.vehicleSpecs.minSpeed)
            {
                newState.velocity = newState.velocity.normalized * aircraftRequest.vehicleSpecs.minSpeed;
            }
            newState.velocity.y = Mathf.Clamp(newState.velocity.y, -aircraftRequest.specs.maxDescentRate, aircraftRequest.specs.maxClimbRate);

            newState.position += velocity * time + 0.5f * acceleration * time * time;
            newState.timeSinceBegining += time;

            newState.UpdateValue();

            if (newState.position.y < aircraftRequest.specs.minAltitude
                || newState.position.y > aircraftRequest.specs.maxAltitude)
            {
                newState.dead = true;
                return;
            }

            Vector3 delta = newState.position - position;
            RaycastHit hit;
            if (Physics.SphereCast(position, aircraftRequest.vehicleSpecs.radius, delta, out hit, delta.magnitude, aircraftRequest.vehicleSpecs.layerMask))
            {
                newState.dead = true;
                return;
            }
            if (Physics.OverlapSphere(newState.position, aircraftRequest.vehicleSpecs.radius, aircraftRequest.vehicleSpecs.layerMask).Length > 0)
            {
                newState.dead = true;
                return;
            }
            openNodes.Add(newState);

            if (newState.request.goal.GetIsGoal(newState))
            {
                goalFound = true;
                return;
            }
        }

        public override void DrawStateDebugGizmo()
        {
            Gizmos.DrawSphere(position, 1);
            Gizmos.DrawRay(position, velocity);
        }

        public override void StatePathDebugGizmo()
        {
            if (parentState != null)
            {
                Gizmos.DrawLine(position, (parentState as GOAPWorldState_SimpleAircraft).position);
                parentState.StatePathDebugGizmo();
            }
        }
    }
}