using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class CharacterSaveData
    {
        [Header("Scene Index")]
        public int sceneIndex;

        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Player")]
        public float secondsPlayed;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;
    }
}
