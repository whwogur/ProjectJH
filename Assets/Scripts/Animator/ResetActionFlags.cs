using UnityEngine;

namespace JH
{
    public class ResetActionFlags : StateMachineBehaviour
    {
        CharacterManager character;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (null == character)
            {
                character = animator.GetComponent<CharacterManager>();
            }

            character.ResetActionFlags();
        }
    }
}
