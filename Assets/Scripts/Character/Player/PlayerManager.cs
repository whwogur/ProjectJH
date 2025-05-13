using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JH
{
    public class PlayerManager : CharacterManager
    {
        [Header("Debug Menu")]
        [SerializeField] bool respawnCharacter = false;
        [SerializeField] bool switchMainWeapon = false;

        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();
            if (!IsOwner)
            {
                return;
            }

            // Handle Movement
            playerLocomotionManager.HandleAllMovement();

            // Stats
            playerStatsManager.RegenerateStamina();

            DebugMenu();
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

            // Equipment
            playerNetworkManager.currentMainWeaponID.OnValueChanged += playerNetworkManager.OnMainWeaponIDChange;
            playerNetworkManager.currentSubWeaponID.OnValueChanged += playerNetworkManager.OnSubWeaponIDChange;
            playerNetworkManager.currentWeapon.OnValueChanged += playerNetworkManager.OnCurrentWeaponIDChange;
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerPopupManager.SendDeathPopup();
            }

            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if (IsOwner)
            {
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                playerNetworkManager.isDead.Value = false;

                playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
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
            Debug.Log($"{currentCharacterData.characterName}");
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;

            Vector3 characterPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = characterPosition;

            InitializeStamina(currentCharacterData);
            InitializeHealth(currentCharacterData);

            currentCharacterData.FinishInitialization();
        }

        private void InitializeStamina(in CharacterSaveData currentCharacterData)
        {
            int calculatedMaxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
            if (0 >= calculatedMaxStamina)
            {
                Debug.LogError($"Invalid calculated stamina: {calculatedMaxStamina}");
            }

            playerNetworkManager.maxStamina.Value = calculatedMaxStamina;
            if (currentCharacterData.IsNewGame)
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
            int calculatedMaxHealth = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);

            playerNetworkManager.maxHealth.Value = calculatedMaxHealth;
            if (currentCharacterData.IsNewGame)
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

            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
        }

        /* TEMP; NEEDS TO BE DELETED LATER */
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }

            if (switchMainWeapon)
            {
                switchMainWeapon = false;
                playerEquipmentManager.SwitchMainWeapon();
            }
        }
    }
}
