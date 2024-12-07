using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class PlanningVisualiser : MonoBehaviour
    {
        public static PlanningVisualiser instance;

        public GameObject pathSegmentPrefab;
        public Transform pathSegmentHolder;
        public List<GameObject> pathSegments;
        private bool showPlanning = false;

        private void Awake()
        {
            instance = this;
        }

        public void ShowPlanning(bool showPlanning)
        {
            this.showPlanning = showPlanning;
            pathSegmentHolder.gameObject.SetActive(showPlanning);
        }

        private void Update()
        {
            if (showPlanning)
            {
                GenerateNodeVisuals(GOAPPathing.instance.DebugGetClosedStates());
            }
        }

        private void GenerateNodeVisuals(List<GOAPWorldState> nodes)
        {
            if (nodes.Count == 0)
                return;

            if (pathSegments.Count < nodes.Count)
            {
                for (int i = 0; i < nodes.Count - pathSegments.Count; i++)
                {
                    pathSegments.Add(Instantiate(pathSegmentPrefab, pathSegmentHolder));
                }
            }

            for (int i = 0; i < pathSegments.Count; i++)
            {
                if (i >= nodes.Count)
                {
                    if (pathSegments[i].activeInHierarchy)
                    {
                        pathSegments[i].SetActive(false);
                    }
                }
                else
                {
                    if (nodes[i] is GOAPWorldState_Vehicle vehicleState)
                    {
                        if (vehicleState.velocity.magnitude > 0.1f)
                        {
                            if (!pathSegments[i].activeInHierarchy)
                            {
                                pathSegments[i].SetActive(true);
                            }
                            pathSegments[i].transform.position = vehicleState.position;
                            pathSegments[i].transform.rotation = Quaternion.LookRotation(vehicleState.velocity);
                            pathSegments[i].transform.localScale = new Vector3(1, 1, vehicleState.velocity.magnitude);
                        }
                    }
                }
            }
        }
    }
}