using UnityEngine;

namespace JH
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(in CharacterManager weaponOwnerCharacter, in WeaponItem weapon)
        {
            meleeDamageCollider.instigatorCharacter = weaponOwnerCharacter;
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.fireDamage = weapon.fireDamage;
            meleeDamageCollider.darkDamage = weapon.darkDamage;
            meleeDamageCollider.holyDamage = weapon.holyDamage;
        }
    }
}
