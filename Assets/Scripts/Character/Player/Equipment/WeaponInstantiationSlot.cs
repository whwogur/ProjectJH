using UnityEngine;

namespace JH
{
    public class WeaponInstantiationSlot : MonoBehaviour
    {
        public GameObject currentWeaponModel;
        public WeaponModelSlot weaponSlot;

        public void UnloadWeapon()
        {
            if (null != currentWeaponModel)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weaponModel)
        {
            // parenting weapon
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }
    }
}
