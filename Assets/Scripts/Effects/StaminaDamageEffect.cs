using UnityEngine;

namespace JH
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Stamina Damage")]
    public class StaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            // compare base stamina damage to other player effects / notifiers
            // change the value before subtracting / adding it
            // play sound FX or VFX during effect

            if (character.IsOwner)
            {
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
                Debug.Log($"Character took {staminaDamage} StaminaDMG");
            }
        }
    }
}
