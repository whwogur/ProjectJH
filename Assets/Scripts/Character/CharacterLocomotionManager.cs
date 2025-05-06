using System.Threading;
using UnityEngine;

namespace JH
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckSphereRadius = 0.3f;
        [SerializeField] private Vector3 sphereTraceOffset;
        [SerializeField] private LayerMask groundLayer = 0/*Default*/;
        [SerializeField] protected float gravityForce = -15.0f;
        [SerializeField] protected Vector3 Velocity = Vector3.zero; // force applied to the character every frame
        [SerializeField] protected float groundedYVelocity = -5.0f; // force at which the character is sticking to the ground whilst they are grounded
        [SerializeField] protected float fallStartYVelocity = -10.0f; // force at which the character begins to fall when they become ungrounded ( rises as it falls )
        protected float inAirTimer = 0.0f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            sphereTraceOffset = new Vector3(0.0f, -0.2f , 0.0f);
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded && Velocity.y <= 0)
            {
                inAirTimer = 0f;
                Velocity.y = groundedYVelocity;
            }
            else
            {
                if (!character.isJumping && Velocity.y <= 0)
                {
                    Velocity.y = fallStartYVelocity;
                }

                Velocity.y += gravityForce * Time.deltaTime;
                Velocity.y = Mathf.Max(Velocity.y, -40.0f); // Clamp max fall speed
                inAirTimer += Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer);
            }

            character.characterController.Move(Velocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position + sphereTraceOffset, groundCheckSphereRadius, groundLayer);
        }

        protected void OnDrawGizmosSelected()
        {
            /* Draw debug ground check sphere */
            Vector3 position;
            if (character != null)
            {
                position = character.transform.position;
            }
            else
            {
                position = transform.position;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + sphereTraceOffset, groundCheckSphereRadius);
        }
    }
}
