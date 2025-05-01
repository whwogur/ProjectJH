using UnityEngine;
using UnityEngine.Rendering;

namespace JH
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;
        public float verticalMovemnt;
        public float horizontalMovemnt;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2.0f;
        [SerializeField] float runningSpeed = 5.0f;
        [SerializeField] float rotationSpeed = 15.0f;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            HandleRotation();
            HandleGroundedMovement();
        }

        private void GetVerticalAndHorizontalInputs()
        {
            verticalMovemnt = PlayerInputManager.instance.verticalInput;
            horizontalMovemnt = PlayerInputManager.instance.horizontalInput;
        }

        private void HandleGroundedMovement()
        {
            GetVerticalAndHorizontalInputs();

            moveDirection = PlayerCamera.Instance.transform.forward * verticalMovemnt;
            moveDirection += PlayerCamera.Instance.transform.right * horizontalMovemnt;
            moveDirection.y = 0.0f;
            moveDirection.Normalize();

            if (0.5f < PlayerInputManager.instance.moveAmount)
            {
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (0.5f >= PlayerInputManager.instance.moveAmount)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovemnt;
            targetRotationDirection += PlayerCamera.Instance.cameraObject.transform.right * horizontalMovemnt;
            targetRotationDirection.y = 0.0f;
            targetRotationDirection.Normalize();

            if (Vector3.zero == targetRotationDirection)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}


