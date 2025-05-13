using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

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

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        }

        protected virtual void Start()
        {
            IgnoreOwnColliders();
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

        public virtual void ResetActionFlags()
        {
            isPerformingAction = false;
            canMove = true;
            canRotate = true;
            applyRootMotion = false;
            if (IsOwner)
            {
                characterNetworkManager.isJumping.Value = false;
            }
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

        protected virtual void IgnoreOwnColliders()
        {
            // Get the main collider
            Collider mainCollider = GetComponent<Collider>();
            if (mainCollider == null)
            {
                Debug.LogWarning($"No Collider found on {gameObject.name}. Skipping IgnoreOwnColliders.");
                return;
            }

            // Get all colliders in children, including the main collider
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            if (0 == allColliders.Length)
            {
                Debug.LogWarning($"No child Colliders found on {gameObject.name}.");
                return;
            }
            else if (allColliders.Length <= 1)
            {
                return; // No need to ignore collisions with fewer than 2 colliders
            }

            // Ignore collisions between the main collider and each child collider
            foreach (Collider childCollider in allColliders)
            {
                if (childCollider != mainCollider)
                {
                    Physics.IgnoreCollision(mainCollider, childCollider, true);
                }
            }

            // Ignore collisions between child colliders
            for (int i = 0; i < allColliders.Length; i++)
            {
                for (int j = i + 1; j < allColliders.Length; j++)
                {
                    Physics.IgnoreCollision(allColliders[i], allColliders[j], true);
                }
            }
        }
    }
}

