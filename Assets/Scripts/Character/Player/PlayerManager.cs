using System;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
        }

        protected override void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            base.Update();
            // Handle Movement
            playerLocomotionManager.HandleAllMovement();

            // Stats
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
            {
                return;
            }

            base.LateUpdate();
            PlayerCamera.instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                if (null == PlayerUIManager.instance || null == PlayerUIManager.instance.playerHUDManager)
                {
                    Debug.LogError("PlayerUIManager or playerHUDManager is not initialized!");
                    return;
                }

                float calculatedStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
                if (0 >= calculatedStamina || float.IsNaN(calculatedStamina) || float.IsInfinity(calculatedStamina))
                {
                    Debug.LogError($"Invalid calculated stamina: {calculatedStamina}");
                    calculatedStamina = 1.0f; // ±âº»°ª
                }

                playerNetworkManager.maxStamina.Value = calculatedStamina;
                playerNetworkManager.currentStamina.Value = calculatedStamina;

                if (null != PlayerUIManager.instance.playerHUDManager.staminaBar)
                {
                    playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewStaminaValue;
                    playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
                    PlayerUIManager.instance.playerHUDManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
                }
                else
                {
                    Debug.LogError("StaminaBar is not initialized!");
                }
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();

            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;


        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;

            Vector3 characterPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = characterPosition;
        }
    }
}
