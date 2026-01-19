using UnityEngine;
using MyDice;
namespace SceneManagement.MyCamera
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(InputManager))]
    public class CameraOrbit : MonoBehaviour
    {
        #region variable
        #region public
        public bool Enable = true;
        [Header("Target")]
        public GameObject target;
        [Min(1.5f)]
        public float maxDistance = 40f;
        [Min(1.5f)]
        public float minDistance = 20f;

        [Range(0, 90)]
        public float maxHeightAngle = 90;
        [Range(0, 90)]
        public float minHeightAngle = 0;

        [Header("Sensitivity")]
        [Min(0)]
        public float mouseSensitivity = 1f;
        [Min(0)]
        public float scrollSensitivity = 1f;
        [Min(0)]
        public float orbitDampening = 10f;
        [Min(0)]
        public float scrollDampening = 6f;
        [Header("Extra")]
        public bool showCameraArea;
        public bool showSnapPoints;
        public SnapPoint[] snapPoints;
        #endregion
        #region protected
        protected Transform x_camera;
        protected Transform x_target;

        protected Vector3 localRotation;
        protected float cameraDistance = 20f;
        #endregion
        #region private
        private InputManager inputManager;
        #region AdsorbentPoint 
        private bool rest = true;
        private SnapPoint snapPoint;
        #endregion
        #endregion
        #endregion
        #region Functions
        private void Awake()
        {
            if (target == null)
            {
                var ground = FindObjectOfType<BoardGameManager>();
                if (ground != null && ground.gameObject != null)
                {
                    target = ground.gameObject;
                }
            }
        }
        private void OnEnable()
        {
            inputManager = GetComponent<InputManager>();
        }
        private void Start()
        {
            init();
        }
        private void LateUpdate()
        {
            if (!Enable) return;
            update_SceneStatus_Normal();
        }
        private void OnDrawGizmos()
        {
            if (x_target != null && x_camera != null)
            {
                Vector3 center = x_target.position;
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(center, x_camera.position);
                if (target != null && target.transform.position == x_target.position)
                {
                    Gizmos.color = Color.blue;
                    Vector3 v = new Vector3(x_camera.position.x, center.y, x_camera.position.z).normalized * cameraDistance;
                    Gizmos.DrawLine(x_target.position, v);
                }
            }
        }
        #endregion
        #region functions
        #region init
        private void init()
        {
            if (inputManager == null)
            {
                Debug.Log("InputManager not found.");
                Extensions.Quit();
            }
            x_camera = this.transform;
            if (this.transform.parent == null)
            {
                GameObject parent = new GameObject("Camera pivot");
                if (target != null) parent.transform.position = target.transform.position;
                x_camera.transform.SetParent(parent.transform);
            }
            x_target = this.transform.parent;
            if (minDistance > maxDistance)
                Extensions.Swap<float>(ref minDistance, ref maxDistance);
            if (minHeightAngle > maxHeightAngle)
                Extensions.Swap<float>(ref minHeightAngle, ref maxHeightAngle);
            updateZoom(.1f);
            localRotation.y = Mathf.Clamp(localRotation.y, minHeightAngle, maxHeightAngle);
        }
        #endregion
        #region rotate
        private void updateRotation(Vector2 deltaPosition)
        {
            updateRotation(deltaPosition.x, deltaPosition.y);
        }
        private void updateRotation(float inputX, float inputY)
        {
            if (inputX == 0 && inputY == 0) return;

            localRotation.x += inputX * mouseSensitivity;
            localRotation.y -= inputY * mouseSensitivity;

            localRotation.y = Mathf.Clamp(localRotation.y, minHeightAngle, maxHeightAngle);
        }
        #endregion
        #region zoom
        private void updateZoom(float inputScroll)
        {
            if (inputScroll == 0) return;
            float scrollAmount = inputScroll * scrollSensitivity;
            // faster zoom in shortest targer distance
            scrollAmount *= (cameraDistance * .3f);
            cameraDistance -= scrollAmount;

            cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance);
        }
        #endregion
        #region camera
        private void updateCameraTransform()
        {
            Quaternion q = Quaternion.Euler(localRotation.y, localRotation.x, 0);
            x_target.rotation = Quaternion.Lerp(x_target.rotation, q, Time.deltaTime * orbitDampening);
            if (x_camera.localPosition.z != -cameraDistance)
            {
                x_camera.localPosition = new Vector3(0, 0, Mathf.Lerp(x_camera.localPosition.z, -cameraDistance, Time.deltaTime * scrollDampening));
                x_camera.rotation = Quaternion.LookRotation(x_target.position - x_camera.position);
            }
        }
        #endregion
        #region adsorbentPoints
        public int getAdsorbentPointsCount()
        {
            return snapPoints == null ? -1 : snapPoints.Length;
        }
        #endregion
        #region SceneStatus.Normal
        private void update_SceneStatus_Normal()
        {
            Vector2 deltaPosition = inputManager.getDeltaPosition();
            float deltaDistance = inputManager.getDeltaDistance();
            bool dpZero = deltaPosition == Vector2.zero;
            bool ddZero = deltaDistance == 0;
            #region rotate camera
            if (dpZero && ddZero)
            {
                if (!rest && getAdsorbentPointsCount() > 0)
                {
                    if (snapPoint != null)
                    {
                        if (snapPoint.EqualToPivot(x_camera.position, x_target.position, .1f))
                        {
                            rest = true;
                            snapPoint = null;
                        }
                        else
                        {
                            updateRotation(new Vector2(snapPoint.getDeltaX(x_camera.position, x_target.position, snapPoint.orbitDampening), 0));
                        }
                    }
                    else
                    {
                        snapPoint = null;
                        foreach (SnapPoint node in snapPoints)
                        {
                            if (node.isInRegion(x_camera.position, x_target.position))
                            {
                                snapPoint = node;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                snapPoint = null;
                rest = false;
                updateRotation(deltaPosition);
                updateZoom(deltaDistance);
            }
            updateCameraTransform();
            #endregion
        }
        #endregion
        #region other
        public void ToggleEnable()
        {
            Enable = !Enable;
        }
        #endregion
        #endregion
    }
}