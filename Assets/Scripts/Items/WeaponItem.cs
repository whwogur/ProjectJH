using UnityEngine;

namespace JH
{
    public class WeaponItem : Item
    {
        // Animator controller override ( Change Attack Animations based on weapon )
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        public int strREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int darkDamage = 0;
        public int holyDamage = 0;

        [Header("Weapon Base Poise Damage")]
        public float poiseDamage = 10.0f;

        [Header("Stamina Costs")]
        public int baseStaminaCost = 20;
    }
}
