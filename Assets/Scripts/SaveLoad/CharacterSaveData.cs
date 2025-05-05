using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class CharacterSaveData
    {
        [Header("Character Name")]
        public string characterName;

        [Header("Time Player")]
        public float secondsPlayed;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;
    }
}
