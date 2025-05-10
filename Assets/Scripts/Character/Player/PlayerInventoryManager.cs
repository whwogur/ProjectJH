using UnityEngine;

namespace JH
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem currentMainWeapon;
        public WeaponItem currentSubWeapon;

        [Header("Quick Slots")]
        public WeaponItem[] weaponsInMainWeaponSlots = new WeaponItem[3];
        public int mainWeaponIndex = 0;

        public WeaponItem[] weaponsInSubWeaponSlots = new WeaponItem[3];
        public int subWeaponIndex = 0;
    }
}
