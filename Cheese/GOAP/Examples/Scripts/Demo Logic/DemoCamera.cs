using System.IO;
using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class DemoCamera : MonoBehaviour
    {
        public enum CameraMode
        {
            Orbit,
            FPV,
            MouseLook,
            Tracking
        }

        [Header("Main Settings")]
        public DemoUnit cameraTarget;

        public CameraMode mode;

        public Camera cam;
        public Transform cameraTf;


        [Header("Mouse Look")]
        public Vector2 rotation;
        public float mouseSensitivity;

        public float cameraDistance;
        public float minDistance;
        public float maxDistance;
        public float distanceSensitivity;

        [Header("Mouse Smooth Look")]
        public bool smoothing;
        private Vector2 rotationalVelocity;
        public float acceleration;
        public float damping;

        private float zoomVelocity;
        public float zoomSpring;
        public float zoomDamping;

        [Header("Camera Collisions")]
        public float cameraCollisionRadius;
        public LayerMask layerMask;

        [Header("Zoom")]
        public float minFov = 1f;
        public float maxFov = 60f;
        public float currentFov = 60f;

        [Header("Mount")]
        public Transform mountObject;
        private Transform targetMountTf;
        private Vector3 localMountPos;
        private Quaternion localMountRot;

        [Header("Noise")]
        public NoiseHelper3D posNoise;
        public NoiseHelper3D rotNoise;

        [Header("Tracking Camera")]
        public float trackingAcceleration;
        public float trackingDamping;

        public RenderTexture renderTexture;

        private void Start()
        {
            posNoise.Setup();
            rotNoise.Setup();
        }

        private void LateUpdate()
        {
            UpdateTarget();
            UpdateMount();
            UpdateCamera();

            if (Application.isEditor && Input.GetKeyDown(KeyCode.F12))
            {
                Screenshot();
            }
        }

        private void UpdateTarget()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    DemoUnit unit = hit.collider.gameObject.GetComponentInParent<DemoUnit>();
                    if (unit != null)
                    {
                        DemoManager.instance.SetUnit(unit);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.RightBracket))
                DemoManager.instance.NextUnit();

            if (Input.GetKeyDown(KeyCode.LeftBracket))
                DemoManager.instance.PreviousUnit();

            if (Input.GetKeyDown(KeyCode.C) && smoothing)
            {
                mode++;
                if ((int)mode > 3)
                {
                    mode = 0;
                }
            }
        }

        private void UpdateMount()
        {
            if (Input.GetKeyDown(KeyCode.V) && smoothing)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    targetMountTf = hit.transform;
                    localMountPos = hit.transform.InverseTransformPoint(hit.point);
                    localMountRot = Quaternion.identity;
                }
            }

            if (targetMountTf != null && smoothing)
            {
                mountObject.position = targetMountTf.TransformPoint(localMountPos);
                mountObject.rotation = targetMountTf.rotation * localMountRot;
            }
        }

        private void UpdateCamera()
        {
            cameraTarget = DemoManager.instance.demoUnit;

            if (cameraTarget == null)
                return;

            switch (mode)
            {
                case CameraMode.Orbit:
                    UpdateOrbit();
                    break;
                case CameraMode.FPV:
                    UpdateFPV();
                    break;
                case CameraMode.MouseLook:
                    cam.transform.position = mountObject.position + mountObject.up * 0.5f;
                    UpdateMouseRotation();
                    UpdateFov(60);
                    break;
                case CameraMode.Tracking:
                    cam.transform.position = mountObject.position + mountObject.up * 0.5f;
                    UpdateDrama();
                    break;
            }
        }

        private void UpdateOrbit()
        {
            cameraDistance += Input.GetAxis("Mouse ScrollWheel") * distanceSensitivity * cameraDistance;
            cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance);

            UpdateMouseRotation();

            RaycastHit hit;
            Vector3 tgtPos = cameraTarget.visualCenter.position;
            if (Physics.SphereCast(tgtPos, cameraCollisionRadius, -transform.forward, out hit, cameraDistance, layerMask))
            {
                transform.position = tgtPos + -transform.forward * hit.distance;
            }
            else
            {
                transform.position = tgtPos + -transform.forward * cameraDistance;
            }

            UpdateFov(60);
            UpdateWobble();
        }

        private void UpdateFPV()
        {
            cameraTf.position = cameraTarget.fpv.position;
            cameraTf.rotation = cameraTarget.fpv.rotation;

            UpdateFov(60);
            UpdateWobble();
        }

        private void UpdateDrama()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                mode = CameraMode.MouseLook;
            }

            Vector3 offset = cameraTarget.visualCenter.position - cameraTf.position;
            cameraTf.rotation = Quaternion.LookRotation(offset);

            //this code was nice, but too jittery at high zoom, maybe i will fix it later
            //Quaternion currentRot = Quaternion.Euler((Vector2)rotation);

            //rotationalVelocity.y += Vector3.Dot(currentRot * Vector3.right, offset.normalized) * trackingAcceleration;
            //rotationalVelocity.x += Vector3.Dot(currentRot * Vector3.up, offset.normalized) * -trackingAcceleration;

            //rotationalVelocity -= rotationalVelocity * Time.unscaledDeltaTime * trackingDamping;

            //rotation += rotationalVelocity * Time.unscaledDeltaTime;
            //rotation.x = Mathf.Clamp(rotation.x, -90, 90);

            float targetSize = 50;
            if (Input.GetKey(KeyCode.Equals))
            {
                targetSize = 20;
            }
            else if (Input.GetKey(KeyCode.Minus))
            {
                targetSize = 200;
            }

            //transform.eulerAngles = (Vector2)rotation;
            UpdateFov(offset, transform.rotation, targetSize);
            UpdateWobble();
        }

        private void UpdateMouseRotation()
        {
            if (smoothing)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    rotationalVelocity.y += Input.GetAxis("Mouse X") * acceleration;
                    rotationalVelocity.x += Input.GetAxis("Mouse Y") * -acceleration;
                }

                rotationalVelocity -= rotationalVelocity * Time.unscaledDeltaTime * damping;
                rotation += rotationalVelocity * Time.unscaledDeltaTime;
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    rotation.y += Input.GetAxis("Mouse X") * mouseSensitivity;
                    rotation.x += Input.GetAxis("Mouse Y") * -mouseSensitivity;

                    rotation.x = Mathf.Clamp(rotation.x, -90, 90);
                }
            }
            transform.eulerAngles = (Vector2)rotation;
            UpdateWobble();
        }

        private void UpdateFov(Vector3 targetOffset, Quaternion currentRotation, float targetSize)
        {
            UpdateFov(Mathf.Lerp(Mathf.Atan2(targetSize, targetOffset.magnitude) * Mathf.Rad2Deg,
                maxFov,
                Vector3.Angle(currentRotation * Vector3.forward, targetOffset) / 30f));
        }

        private void UpdateFov(float targetFov)
        {
            targetFov = Mathf.Clamp(targetFov, minFov, maxFov);

            zoomVelocity += (targetFov - currentFov) * Time.unscaledDeltaTime * zoomSpring;
            zoomVelocity -= zoomVelocity * Time.unscaledDeltaTime * zoomDamping;

            currentFov += zoomVelocity * Time.unscaledDeltaTime;
            currentFov = Mathf.Clamp(currentFov, minFov, maxFov);

            cam.fieldOfView = currentFov;
        }

        private void UpdateWobble()
        {
            cameraTf.position += posNoise.Evaluate();
            cameraTf.rotation = cameraTf.rotation * Quaternion.Euler(posNoise.Evaluate());
        }

        private void Screenshot()
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            cam.targetTexture = renderTexture;
            cam.Render();
            cam.targetTexture = null;

            Texture2D Image = new Texture2D(renderTexture.width, renderTexture.height);
            Image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            Image.Apply();
            RenderTexture.active = currentRT;

            var Bytes = Image.EncodeToPNG();
            Destroy(Image);

            string path = $@"C:\Users\oscar\Pictures\CAPNScreenshots{Random.Range(0, 1000000)}.png";
            File.WriteAllBytes($@"C:\Users\oscar\Pictures\CAPNScreenshots\screenshot{Random.Range(0, 1000000)}.png", Bytes);
            Debug.Log($"Saved image to {path}");
        }
    }
}