using System.Globalization;
using UnityEngine;

namespace JH
{
    public class CharacterStatsManager : MonoBehaviour
    {
        protected CharacterManager character;

        [Header("Stamina Regeneration")]
        private float staminaRegenerationTimer = 0.0f;
        private float staminaTickTimer = 0.0f;
        [SerializeField] float regenerationDelay = 2.0f;
        [SerializeField] float staminaRegenAmount = 2.0f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            int health = 0;

            health = vitality * 10;

            return health;
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            int stamina = 0;

            stamina = endurance * 10;

            return stamina;
        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
            {
                return;
            }

            if (character.characterNetworkManager.isSprinting.Value ||
                character.isPerformingAction)
                return;

            staminaRegenerationTimer += Time.deltaTime;

            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                if (regenerationDelay <= staminaRegenerationTimer)
                {
                    staminaTickTimer += Time.deltaTime;
                    if (0.1f <= staminaTickTimer)
                    {
                        staminaTickTimer = 0.0f;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenAmount * Time.deltaTime;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
        {
            if (newValue < oldValue)
            {
                staminaRegenerationTimer = 0.0f;
            }
        }
    }
}
