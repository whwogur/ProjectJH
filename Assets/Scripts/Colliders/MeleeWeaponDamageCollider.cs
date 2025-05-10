using UnityEngine;

namespace JH
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager instigatorCharacter;
    }
}
