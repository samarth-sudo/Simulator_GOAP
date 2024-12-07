using UnityEngine;
using UnityEngine.EventSystems;

namespace Cheese.GOAP.Demo
{
    public class DemoWaypointPlacer : MonoBehaviour
    {
        public enum PlacingState
        {
            None,
            Placing,
            Placed,
            Failed,
            Canceled
        }

        public static DemoWaypointPlacer instance;

        public Camera cam;
        public Transform indicatorTf;

        public PlacingState placingState;

        public Vector3 placedPos;
        public Vector3 placedNormal;

        public Renderer[] renderers;

        public Material valid;
        public Material invalid;

        public float angle = 10f;

        private DemoUnit unit;

        private void Awake()
        {
            instance = this;
        }

        public void StartWaypointPlacement(bool mustBeFlat)
        {
            placingState = PlacingState.Placing;

            angle = mustBeFlat ? 10f : 360f;

            unit = DemoManager.instance.demoUnit;
        }

        private void LateUpdate()
        {
            if (placingState == PlacingState.Placing)
            {
                // cancel if the user switches unit
                if (unit != DemoManager.instance.demoUnit)
                {
                    placingState = PlacingState.Canceled;

                    indicatorTf.gameObject.SetActive(false);
                    return;
                }

                // cancel placing if we click on the UI
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    indicatorTf.gameObject.SetActive(false);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        placingState = PlacingState.Canceled;

                        indicatorTf.gameObject.SetActive(false);
                    }
                    return;
                }

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50000))
                {
                    placedPos = hit.point;
                    placedNormal = hit.normal;

                    indicatorTf.position = placedPos;
                    indicatorTf.rotation = Quaternion.LookRotation(Vector3.Cross(placedNormal, cam.transform.right), placedNormal);

                    indicatorTf.gameObject.SetActive(true);

                    if (Vector3.Angle(hit.normal, Vector3.up) < angle)
                    {
                        SetMaterial(valid);

                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            placingState = PlacingState.Placed;

                            indicatorTf.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        SetMaterial(invalid);

                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            placingState = PlacingState.Failed;

                            indicatorTf.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    indicatorTf.gameObject.SetActive(false);

                    //cancel placing if we click on the sky
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        placingState = PlacingState.Canceled;
                    }
                }
            }
        }

        private void SetMaterial(Material mat)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material = mat;
            }
        }
    }
}