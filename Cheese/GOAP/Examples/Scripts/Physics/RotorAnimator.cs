using UnityEngine;

namespace Cheese.GOAP.Demo
{
    /// <summary>
    /// Animates a single rotor on the helicopter based on the RPM, used for the main and tail rotor
    /// </summary>
    public class RotorAnimator : EngineAnimation
    {
        [Tooltip("The transform to rotates")]
        public Transform rotorTf;
        [Tooltip("The local axis the rotor rotates around")]
        public Vector3 axis = Vector3.up;

        [Tooltip("Scales the rotor speed for a more stylised/less teleporty look")]
        public float rotorSpeedScale = 1;

        private void Update()
        {
            rotorTf.Rotate(axis * rpm * rotorSpeedScale * Time.deltaTime, Space.Self);
        }
    }
}