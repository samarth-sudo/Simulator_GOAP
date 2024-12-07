using UnityEngine;

namespace Cheese.GOAP.Demo
{
    /// <summary>
    /// A decorational building used as part of the skybox
    /// </summary>
    public class SkyboxBuilding : MonoBehaviour
    {
        [Tooltip("The hight of the model before scaling")]
        public int defaultHeight = 100;
        [Tooltip("The model to scale")]
        public Transform modelTf;
        [Tooltip("The radius of a building, used to avoid collisions when placing")]
        public float buildingRadius = 20;

        /// <summary>
        /// Scales the model to be a certain height
        /// </summary>
        /// <param name="height">the height to scale to</param>
        public void SetHeight(float height)
        {
            modelTf.localScale = new Vector3(1, height / defaultHeight, 1);
        }
    }
}