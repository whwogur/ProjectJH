using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            if (0/*TODO*/ == SceneManager.GetActiveScene().buildIndex)
            {
                currentCharacterData.sceneIndex = 1;
            }
            else
            {
                currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            }
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();

            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;

            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;

            Vector3 characterPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = characterPosition;

            InitializeStamina(currentCharacterData);
            InitializeHealth(currentCharacterData);

            currentCharacterData.FinishInitialization();
        }

        private void InitializeStamina(in CharacterSaveData currentCharacterData)
        {
            float calculatedMaxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
            if (0 >= calculatedMaxStamina || float.IsNaN(calculatedMaxStamina) || float.IsInfinity(calculatedMaxStamina))
            {
                Debug.LogError($"Invalid calculated stamina: {calculatedMaxStamina}");
            }

            playerNetworkManager.maxStamina.Value = calculatedMaxStamina;
            if (currentCharacterData.NewGame)
            {
                playerNetworkManager.currentStamina.Value = calculatedMaxStamina;
            }
            else
            {
                playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            }
            

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

        private void InitializeHealth(in CharacterSaveData currentCharacterData)
        {
            float calculatedMaxHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);
            if (0 >= calculatedMaxHealth || float.IsNaN(calculatedMaxHealth) || float.IsInfinity(calculatedMaxHealth))
            {
                Debug.LogError($"Invalid calculated health: {calculatedMaxHealth}");
            }

            playerNetworkManager.maxHealth.Value = calculatedMaxHealth;
            if (currentCharacterData.NewGame)
            {
                playerNetworkManager.currentHealth.Value = calculatedMaxHealth;
            }
            else
            {
                playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            }
                

            if (null != PlayerUIManager.instance.playerHUDManager.healthBar)
            {
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerHUDManager.SetNewHealthValue;
                PlayerUIManager.instance.playerHUDManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
            }
            else
            {
                Debug.LogError("HealthBar is not initialized!");
            }
        }
    }
}
