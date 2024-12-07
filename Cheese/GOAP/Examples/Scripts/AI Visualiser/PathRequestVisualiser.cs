using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class PathRequestVisualiser : MonoBehaviour
    {
        public static PathRequestVisualiser instance;

        public GameObject pathRequestPrefab;
        public Transform pathRequestHolder;
        public List<GameObject> pathRequests;
        private bool showRequests = false;

        private void Awake()
        {
            instance = this;
        }

        public void ShowRequests(bool showRequests)
        {
            this.showRequests = showRequests;
            pathRequestHolder.gameObject.SetActive(showRequests);
        }

        private void Update()
        {
            if (showRequests)
            {
                GenerateRequestVisuals(GOAPPathing.instance.DebugGetPathRequests());
            }
        }

        private void GenerateRequestVisuals(List<GOAPPathRequest> requests)
        {
            if (pathRequests.Count < requests.Count)
            {
                for (int i = 0; i < requests.Count - pathRequests.Count; i++)
                {
                    pathRequests.Add(Instantiate(pathRequestPrefab, pathRequestHolder));
                }
            }

            for (int i = 0; i < pathRequests.Count; i++)
            {
                if (i >= requests.Count)
                {
                    if (pathRequests[i].activeInHierarchy)
                    {
                        pathRequests[i].SetActive(false);
                    }
                }
                else
                {
                    if (requests[i].agent.IsAlive()
                        && requests[i] is GOAPPathRequest_Vehicle vehicleRequest
                        && vehicleRequest.agent.GetCurrentState() is GOAPWorldState_Vehicle vehicleState
                        && requests[i].goal is GOAPGoal_MoveTo moveToGoal)
                    {
                        if (!pathRequests[i].activeInHierarchy)
                        {
                            pathRequests[i].SetActive(true);
                        }

                        Vector3 offset = moveToGoal.goalPos - vehicleState.position;

                        pathRequests[i].transform.position = vehicleState.position;
                        pathRequests[i].transform.rotation = Quaternion.LookRotation(offset);
                        pathRequests[i].transform.localScale = new Vector3(1, 1, offset.magnitude);
                    }
                }
            }
        }
    }
}