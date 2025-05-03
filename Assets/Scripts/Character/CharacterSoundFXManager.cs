using UnityEngine;

namespace JH
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayDodgeSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.dodgeSFX);
        }
    }
}
