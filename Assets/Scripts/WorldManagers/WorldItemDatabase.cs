using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JH
{

    public class WorldItemDatabase : MonoBehaviour
    {
        private static WorldItemDatabase _instance;
        public static WorldItemDatabase instance => _instance;

        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
        private Dictionary<int, Item> items = new Dictionary<int, Item>();

        private void Awake()
        {
            if (null == _instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            for (int i = 0; i < weapons.Count; ++i)
            {
                weapons[i].itemID = i; // TEMP
                items.Add(i, weapons[i]);
            }
        }

        public WeaponItem GetWeaponByID(int id)
        {
            return weapons.FirstOrDefault(weapon => id == weapon.itemID);
        }

        public Item GetItemByID(int id)
        {
            if (items.TryGetValue(id, out Item item))
            {
                return item;
            }
            return null; // Return null if the item is not found
        }
    }
}
