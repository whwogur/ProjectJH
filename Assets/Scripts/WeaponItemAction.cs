using UnityEngine;

namespace JH
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;

        public virtual void AttemptToPerformAction(in PlayerManager playerPerformingAction, in WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.currentWeapon.Value = weaponPerformingAction.itemID;
            }

            Debug.Log("Action has fired");
        }
    }
}
