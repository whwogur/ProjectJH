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
            base.Update();

            LocomotionManager.HandleAllMovement();
        }
    }
}
