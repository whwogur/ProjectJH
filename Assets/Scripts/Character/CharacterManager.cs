using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using System.Collections;

namespace JH
{
    public class CharacterManager : NetworkBehaviour
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;

        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;
        public bool isGrounded = true;
        public bool isJumping = false;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        }

        protected virtual void Update()
        {
            animator.SetBool("IsGrounded", isGrounded);

            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                // Position
                transform.position = Vector3.SmoothDamp(
                    transform.position, characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime
                );

                // Rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkRotationSmoothTime
                );
            }
        }

        protected virtual void LateUpdate()
        {

        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                if (characterNetworkManager.currentHealth.Value > 0) // 이미 0이면 변경하지 않음
                {
                    characterNetworkManager.currentHealth.Value = 0;
                }
                characterNetworkManager.isDead.Value = true;

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }

            yield return new WaitForSeconds(5);
        }

        public virtual void ReviveCharacter()
        {

        }
    }
}

