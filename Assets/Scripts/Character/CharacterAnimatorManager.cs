using UnityEngine;
using Unity.Netcode;

namespace JH
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;
        int vertical;
        int horizontal;
            
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float scopedHorizontal = horizontalValue;
            float scopedVertical = verticalValue;

            if (isSprinting)
            {
                scopedVertical = 2.0f;
            }

            character.animator.SetFloat(horizontal, scopedHorizontal, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, scopedVertical, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(
            in string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            // 서버에 알림
            character.characterNetworkManager.NotifyAnimation_ServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}


