using UnityEngine;
using UnityEngine.Rendering;

namespace JH
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;
        [HideInInspector] public float verticalMovemnt;
        [HideInInspector] public float horizontalMovemnt;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2.0f;
        [SerializeField] float runningSpeed = 5.0f;
        [SerializeField] float sprintingSpeed = 10.0f;
        [SerializeField] float rotationSpeed = 15.0f;

        [Header("Dodge")]
        private Vector3 rollDirection;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovemnt;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovemnt;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovemnt = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovemnt = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
        }
        public void HandleAllMovement()
        {
            HandleRotation();
            HandleGroundedMovement();
        }

        private void GetMovementValues()
        {
            verticalMovemnt = PlayerInputManager.instance.verticalInput;
            horizontalMovemnt = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }

        private void HandleGroundedMovement()
        {
            if (!player.canMove)
                return;

            GetMovementValues();

            moveDirection = PlayerCamera.instance.transform.forward * verticalMovemnt;
            moveDirection += PlayerCamera.instance.transform.right * horizontalMovemnt;
            moveDirection.y = 0.0f;
            moveDirection.Normalize();

            if (player.playerNetworkManager.isSprinting.Value) // Sprinting
            {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else // Not Sprinting
            {
                if (0.5f < PlayerInputManager.instance.moveAmount)
                {
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (0.5f >= PlayerInputManager.instance.moveAmount)
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
        }

        private void HandleRotation()
        {
            if (!player.canRotate)
                return;

            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovemnt;
            targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovemnt;
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

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                // set sprinting to false
                player.playerNetworkManager.isSprinting.Value = false;
            }

            // if out of stamina, set sprinting to false

            if (0.5f <= moveAmount) // if moving, set sprinting to true
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else // if stationary, set sprinting to false
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

        }
        /* 움직이고 있다면 구르고, 가만히 있는 상태에서 눌렀으면 백스텝 */
        public void AttempToPerformDodge()
        {
            if (player.isPerformingAction)
            {
                return;
            }

            if (0 < PlayerInputManager.instance.moveAmount)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

                rollDirection.y = 0.0f;
                rollDirection.Normalize();
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                // Perform roll animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Dodge_F", true, true, false, false);
            }
            else
            {
                // perform backstep animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Quickstep_B", true, true, false, true);
            }
        }
    }
}


