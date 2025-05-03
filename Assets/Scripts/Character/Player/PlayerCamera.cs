using Mono.Cecil;
using UnityEngine;

namespace JH
{
    public class PlayerCamera : MonoBehaviour
    {
        private static PlayerCamera _instance;
        public static PlayerCamera instance => _instance;
        public Camera cameraObject;
        public PlayerManager player;
        [SerializeField] Transform cameraPivotTransform;

        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1.0f; // 이 값이 클수록 lerp 시간 늘어남
        [SerializeField, Range(1, 100)] float upAndDownRotationSpeed = 50.0f;
        [SerializeField, Range(1, 100)] float leftAndRightRotationSpeed = 50.0f;
        [SerializeField] float minimumPivot = -30.0f;
        [SerializeField] float maximumPivot = 60.0f;
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask layersToCollideWith;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // 카메라 충돌 계산용
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition; // 카메라 충돌 계산용
        private float targetCameraZPosition; // 카메라 충돌 계산용

        private void Awake()
        {
            if (null == _instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z; 
        }

        public void HandleAllCameraActions()
        {
            if (null != player)
            {
                HandleFollowTarget();
                HandleRotation();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotation()
        {
            float mouseSensitivity = 0.07f;

            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed * mouseSensitivity) * Time.deltaTime;
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed * mouseSensitivity) * Time.deltaTime;

            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), layersToCollideWith))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f/*lerp time*/);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}

