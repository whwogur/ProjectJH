using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class CharacterSaveData
    {
        public bool NewGame = false;

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

        [Header("Resources")]
        public float currentHealth;
        public float currentStamina;

        [Header("Stats")]
        public int vitality;
        public int endurance;

        public void FreshStart()
        {
            vitality = 10;
            endurance = 10;

            NewGame = true; // set to false after initialization
        }

        /* Sets bNewGame to false */
        public void FinishInitialization()
        {
            NewGame = false;
        }
    }
}
