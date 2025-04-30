using UnityEngine;

namespace JH
{
    public class CharacterManager : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

