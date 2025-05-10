using UnityEngine;

namespace JH
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage Effect")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager instigatorCharacter; // character causing damage - if damage is caused by another character's attack it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0.0f;
        public float magicDamage    = 0.0f;
        public float fireDamage     = 0.0f;
        public float darkDamage     = 0.0f;
        public float holyDamage     = 0.0f;

        [Header("Final Damage")]
        private int finalDamage = 0; // The damage the character takes after all calculations have been made

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Poise")]
        public float poiseDamage = 0.0f;
        public bool poiseIsBroken = false; // if a character's poise is 'broken' it will be stunned

        [Header("SoundFX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSFX; // used on top of regular SFX if there is elemental damage present

        [Header("Direction Damage Taken From")]
        public float angleHitFrom = 0.0f; // used to determine what damage animation to play
        public Vector3 contactPoint = Vector3.zero; // used to determine where the hit FX (blood, etc..) should be spawned

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.characterNetworkManager.isDead.Value)
            {
                return;
            }

            // check for invulnerability

            CalculateDamage(character);
            // check which direction damage came from
            // play damage animation
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner)
            {
                return;
            }

            if (null != instigatorCharacter)
            {
                // check for damage modifiers and modify base damage ( buff.. etc )
            }

            // check character for flat defenses and subtract them from the damage

            // check character for armor absorb and subtract the percentage from the damage

            // add all damage types together, and apply final damage
            finalDamage = Mathf.RoundToInt(physicalDamage + fireDamage + darkDamage + holyDamage);

            if (0 >= finalDamage)
            {
                finalDamage = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamage;

            // calculate poise damage to determine if the character will be stunned
        }
    }
}
