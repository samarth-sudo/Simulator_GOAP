using System.Collections.Generic;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DebugGOAPVisualiser : MonoBehaviour
    {
        public static DebugGOAPVisualiser instance;

        public GameObject pathVisualPrefab;

        public GameObject pathVisualHolder;
        private List<GOAPWorldState_Vehicle> lastPath;

        public GameObject goalHolder;
        public Transform startIndicatorTf;
        public Transform goalIndicatorTf;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (DemoManager.instance.demoUnit != null)
            {
                GOAPDriver driver = DemoManager.instance.demoUnit.GetComponent<GOAPDriver>();

                //kinda janky check to see if the path is updated, but its just for the demo so whatever.
                if (driver != null && lastPath != driver.DebugGetPath())
                {
                    lastPath = driver.DebugGetPath();
                    RegeneratePath(lastPath);
                }

                if (driver is GOAPDriver_DemoHelicopter helicopter)
                {
                    startIndicatorTf.position = helicopter.defaultMoveGoal.startingPos;
                    goalIndicatorTf.position = helicopter.defaultMoveGoal.goalPos;
                    return;
                }
                if (driver is GOAPDriver_DemoFixedWing fixedWing)
                {
                    startIndicatorTf.position = fixedWing.defaultMoveGoal.startingPos;
                    goalIndicatorTf.position = fixedWing.defaultMoveGoal.goalPos;
                    return;
                }
                if (driver is GOAPDriver_DemoVtol vtol)
                {
                    startIndicatorTf.position = vtol.defaultMoveGoal.startingPos;
                    goalIndicatorTf.position = vtol.defaultMoveGoal.goalPos;
                    return;
                }
            }
        }

        public void RegeneratePath(List<GOAPWorldState_Vehicle> path)
        {
            for (int i = 0; i < pathVisualHolder.transform.childCount; i++)
            {
                Destroy(pathVisualHolder.transform.GetChild(i).gameObject);
            }

            if (path != null)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    GOAPWorldState_Vehicle point1 = path[i - 1];
                    GOAPWorldState_Vehicle point2 = path[i];

                    if (point1.position != point2.position)
                    {
                        GameObject pathVisualGo = Instantiate(pathVisualPrefab);
                        pathVisualGo.transform.parent = pathVisualHolder.transform;

                        pathVisualGo.transform.position = point1.position;
                        pathVisualGo.transform.rotation = Quaternion.LookRotation(point2.position - point1.position);
                        pathVisualGo.transform.localScale = new Vector3(1, 1, (point1.position - point2.position).magnitude);
                    }
                }
            }
        }

        public void SetGoalsVisibile(bool visible)
        {
            goalHolder.SetActive(visible);
        }

        public void SetPathVisibile(bool visible)
        {
            pathVisualHolder.SetActive(visible);
        }
    }
}