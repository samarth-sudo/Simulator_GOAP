﻿using UnityEngine;

namespace Cheese.GOAP
{
    public class GOAPWorldState_Vehicle : GOAPWorldState
    {
        public Vector3 position;
        public Vector3 velocity;

        public GOAPWorldState_Vehicle() : base()
        {

        }

        public GOAPWorldState_Vehicle(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }

        public GOAPWorldState_Vehicle(Transform tf, Rigidbody rb)
        {
            position = tf ? tf.position : Vector3.zero;
            velocity = rb ? rb.velocity : Vector3.zero;
        }

        public GOAPWorldState_Vehicle(Rigidbody rb)
        {
            position = rb ? rb.position : Vector3.zero;
            velocity = rb ? rb.velocity : Vector3.zero;
        }

        public GOAPWorldState_Vehicle(GOAPWorldState_Vehicle stateToCopy) : base(stateToCopy)
        {
            position = stateToCopy.position;
            velocity = stateToCopy.velocity;
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
                Gizmos.DrawLine(position, (parentState as GOAPWorldState_Vehicle).position);
                parentState.StatePathDebugGizmo();
            }
        }
    }
}