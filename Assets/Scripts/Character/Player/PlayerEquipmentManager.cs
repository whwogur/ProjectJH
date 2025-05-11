using UnityEngine;

namespace JH
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;
        public WeaponInstantiationSlot mainWeaponSlot;
        public WeaponInstantiationSlot subWeaponSlot;

        [SerializeField] WeaponManager mainWeaponManager;
        [SerializeField] WeaponManager subWeaponManager;

        public GameObject mainWeaponModel;
        public GameObject subWeaponModel;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();

            InitializeSlots();
        }

        protected override void Start()
        {
            base.Start();
            LoadAllWeapons();
        }

        private void InitializeSlots()
        {
            WeaponInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponInstantiationSlot>();

            foreach (WeaponInstantiationSlot weaponSlot in weaponSlots)
            {
                if (WeaponModelSlot.MainWeapon == weaponSlot.weaponSlot)
                {
                    mainWeaponSlot = weaponSlot;
                }
                else if (WeaponModelSlot.SubWeapon == weaponSlot.weaponSlot)
                {
                    subWeaponSlot = weaponSlot;
                }
            }
        }

        public void LoadAllWeapons()
        {
            LoadMainWeapon();
            LoadSubWeapon();
        }

        //=================
        // Main Weapon
        //=================
        public void LoadMainWeapon()
        {
            if (null != player.playerInventoryManager.currentMainWeapon)
            {
                // remove original weapon
                mainWeaponSlot.UnloadWeapon();

                // equip new weapon
                mainWeaponModel = Instantiate(player.playerInventoryManager.currentMainWeapon.weaponModel);
                mainWeaponSlot.LoadWeapon(mainWeaponModel);
                // Assign weapon damage to its collider
                mainWeaponManager = mainWeaponModel.GetComponent<WeaponManager>();
                mainWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentMainWeapon);
            }
        }

        public void SwitchMainWeapon()
        {
            if (!player.IsOwner)
            {
                return;
            }

            // Equip / Unequip Animations
            player.playerAnimatorManager.PlayTargetActionAnimation("TakeItemOut", false);

            WeaponItem selectedWeapon = null;

            player.playerInventoryManager.mainWeaponIndex += 1;
            if (player.playerInventoryManager.mainWeaponIndex < 0 || player.playerInventoryManager.mainWeaponIndex > 2)
            {
                player.playerInventoryManager.mainWeaponIndex = 0;
                int weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < player.playerInventoryManager.weaponsInMainWeaponSlots.Length; ++i)
                {
                    if (player.playerInventoryManager.weaponsInMainWeaponSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;

                        if (null == firstWeapon)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInMainWeaponSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.mainWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentMainWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.mainWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentMainWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInMainWeaponSlots)
            {
                // check to see if ths is not the unarmed weapon
                if (player.playerInventoryManager.weaponsInMainWeaponSlots[player.playerInventoryManager.mainWeaponIndex].itemID 
                    != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponsInMainWeaponSlots[player.playerInventoryManager.mainWeaponIndex];

                    // Assign network weaponID
                    player.playerNetworkManager.currentMainWeaponID.Value 
                        = player.playerInventoryManager.weaponsInMainWeaponSlots[player.playerInventoryManager.mainWeaponIndex].itemID;
                    return;
                }
            }

            if (null == selectedWeapon && 2 >= player.playerInventoryManager.mainWeaponIndex)
            {
                SwitchMainWeapon();
            }
        }
        //=================
        // Sub Weapon
        //=================
        public void LoadSubWeapon()
        {
            if (null != player.playerInventoryManager.currentSubWeapon)
            {
                // remove original weapon
                subWeaponSlot.UnloadWeapon();

                // equip new weapon
                subWeaponModel = Instantiate(player.playerInventoryManager.currentSubWeapon.weaponModel);
                subWeaponSlot.LoadWeapon(subWeaponModel);
                // Assign weapon damage to its collider
                subWeaponManager = subWeaponModel.GetComponent<WeaponManager>();
                subWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentSubWeapon);
            }
        }

        public void SwitchSubWeapon()
        {

        }
    }
}
