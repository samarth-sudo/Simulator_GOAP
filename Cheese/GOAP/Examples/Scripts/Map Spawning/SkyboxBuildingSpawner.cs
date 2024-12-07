using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class SkyboxBuildingSpawner : MonoBehaviour
    {
        public int seed;

        public float buildingCount;
        public float buildingRadius;

        public GameObject buildingPrefab;

        public float rotationNoiseScale = 100;
        private Vector2 directionNoiseOffsetX;
        private Vector2 directionNoiseOffsetY;
        private Vector2 heightNoiseOffset;

        public AnimationCurve buildingHeightCurve;
        public float buildingHeightNoiseScale = 100;
        public float buildingHeightNoiseIntensity = 20;

        public LayerMask buidlingLayerMask;
        public float padding = 20;

        private List<SkyboxBuilding> buildings;

        [ContextMenu("Spawn Buildings")]
        private void SpawnBuildings()
        {
            DestroyBuildings();
            buildings = new List<SkyboxBuilding>();

            Random.InitState(seed);

            directionNoiseOffsetX = Random.insideUnitCircle * 500;
            directionNoiseOffsetY = Random.insideUnitCircle * 500;
            heightNoiseOffset = Random.insideUnitCircle * 500;

            for (int i = 0; i < buildingCount; i++)
            {
                Vector3 localPos = Random.insideUnitCircle * buildingRadius;
                localPos.z = localPos.y;
                localPos.y = 0;

                Vector2 pos2D = new Vector2(localPos.x, localPos.z);

                Vector3 buildingLookDir = new Vector3(NormalisedPerlin(pos2D + directionNoiseOffsetX, rotationNoiseScale), 0, NormalisedPerlin(pos2D + directionNoiseOffsetY, rotationNoiseScale)).normalized;
                Quaternion buildingRot = Quaternion.LookRotation(buildingLookDir);

                float buildingHeight = buildingHeightCurve.Evaluate(pos2D.magnitude) + NormalisedPerlin(pos2D + heightNoiseOffset, buildingHeightNoiseScale) * buildingHeightNoiseIntensity;

                Vector3 pos = transform.TransformPoint(localPos);
                Vector3 bounding = new Vector3(20 + padding, 1, 10 + padding);
                if (Physics.BoxCast(pos + Vector3.up * 1000, bounding / 2f, Vector3.down, buildingRot, 990, buidlingLayerMask) == false)
                {
                    if (ClosestBuildingDistnace(pos) - 15 > 0)
                    {
                        GameObject spawnedPrefab = Instantiate(buildingPrefab, transform);
                        SkyboxBuilding building = spawnedPrefab.GetComponent<SkyboxBuilding>();

                        spawnedPrefab.transform.localPosition = localPos;
                        spawnedPrefab.transform.rotation = buildingRot;

                        building.SetHeight(buildingHeight);

                        buildings.Add(building);
                    }
                }
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

        private float NormalisedPerlin(Vector2 point, float scale)
        {
            point /= scale;

            return Mathf.Lerp(-1, 1, Mathf.PerlinNoise(point.x, point.y));
        }

        private float ClosestBuildingDistnace(Vector3 pos)
        {
            float closest = Mathf.Infinity;
            foreach (SkyboxBuilding building in buildings)
            {
                float distance = (pos - building.transform.position).magnitude - building.buildingRadius;
                if (distance < closest)
                {
                    closest = distance;
                }
            }
            return closest;
        }
    }
}