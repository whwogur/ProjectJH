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
        [SerializeField] float sprintingStaminaCost = 1.0f;

        [Header("Jump")]
        [SerializeField] float jumpHeight = 3.14f;
        [SerializeField] float jumpMovementSpeed = 5.0f;
        [SerializeField] float freeFallMovementSpeed = 3.0f;
        private Vector3 jumpDirection;

        [Header("Dodge")]
        private Vector3 dodgeDirection;
        [SerializeField] float dodgeStaminaCost = 2.0f;

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
            HandleJumpingMovement();
            HandleFreeFallMovement();
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

        private void HandleJumpingMovement()
        {
            if (player.playerNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpDirection * jumpMovementSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeFallMovement()
        {
            if (!player.isGrounded)
            {
                Vector3 freeFallDirection;
                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
                freeFallDirection.y = 0.0f;

                player.characterController.Move(freeFallDirection * freeFallMovementSpeed * Time.deltaTime);
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
            if (0 >= player.playerNetworkManager.currentStamina.Value)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            if (0.5f <= moveAmount) // if moving, set sprinting to true
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else // if stationary, set sprinting to false
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }

        }
        /* 움직이고 있다면 구르고, 가만히 있는 상태에서 눌렀으면 백스텝 */
        public void AttempToPerformDodge()
        {
            if (player.isPerformingAction ||
                player.playerNetworkManager.currentStamina.Value < 0)
            {
                return;
            }

            if (0 < PlayerInputManager.instance.moveAmount)
            {
                dodgeDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                dodgeDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

                dodgeDirection.y = 0.0f;
                dodgeDirection.Normalize();
                Quaternion playerRotation = Quaternion.LookRotation(dodgeDirection);
                player.transform.rotation = playerRotation;

                // Perform roll animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Dodge_F", true, true, false, false);
            }
            else
            {
                // perform backstep animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Quickstep_B", true, true, false, true);
            }

            //Debug.Log($"{player.playerNetworkManager.currentStamina.Value} - {dodgeStaminaCost}");
            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }

        public void AttempToPerformJump()
        {
            if (player.isPerformingAction || !player.isGrounded || player.playerNetworkManager.isJumping.Value)
            {
                return;
            }

            player.playerAnimatorManager.PlayTargetActionAnimation("Jump", false, true);

            player.playerNetworkManager.isJumping.Value = true;

            jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            jumpDirection.y = 0.0f;

            if (Vector3.zero != jumpDirection)
            {
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1.0f;
                }
                else if (0.5f < PlayerInputManager.instance.moveAmount)
                {
                    jumpDirection *= 0.5f;
                }
                else if (0.5f >= PlayerInputManager.instance.moveAmount)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            // apply upward velocity
            Velocity.y = Mathf.Sqrt(jumpHeight * -1.0f * gravityForce);
        }
    }
}


