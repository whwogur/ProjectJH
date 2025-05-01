using System;
using UnityEngine;

namespace JH
{
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotionManager LocomotionManager;
        protected override void Awake()
        {
            base.Awake();

            LocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        protected override void Update()
        {

            if (!IsOwner)
            {
                return;
            }

            base.Update();
            LocomotionManager.HandleAllMovement();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
            {
                return;
            }

            base.LateUpdate();
            PlayerCamera.instance.HandleAllCameraActions();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
            }
        }
    }
}
