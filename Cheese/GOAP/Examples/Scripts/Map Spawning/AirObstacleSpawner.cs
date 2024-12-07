using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class AirObstacleSpawner : MonoBehaviour
    {
        public int seed;


        public GameObject skyObstaclePrefab;

        public float obstacleCount;
        public Vector3 bounds;

        [ContextMenu("Spawn Buildings")]
        private void SpawnBuildings()
        {
            DestroyBuildings();

            Random.InitState(seed);

            for (int i = 0; i < obstacleCount; i++)
            {
                GameObject spawnedPrefab = Instantiate(skyObstaclePrefab, transform);

                spawnedPrefab.transform.localPosition = new Vector3(Random.Range(-bounds.x, bounds.x),
                    Random.Range(-bounds.y, bounds.y),
                    Random.Range(-bounds.z, bounds.z));
                spawnedPrefab.transform.rotation = Quaternion.LookRotation(Random.onUnitSphere, Random.onUnitSphere);
            }
        }

        [ContextMenu("Destroy Buildings")]
        private void DestroyBuildings()
        {
            while (transform.childCount > 0)
            {
                Transform childTf = transform.GetChild(0);
                DestroyImmediate(childTf.gameObject);
            }
        }
    }
}