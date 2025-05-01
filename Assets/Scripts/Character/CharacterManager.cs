using UnityEngine;
using Unity.Netcode;

namespace JH
{
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterController characterController;

        CharacterNetworkManager characterNetworkManager;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update()
        {
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
    }
}

