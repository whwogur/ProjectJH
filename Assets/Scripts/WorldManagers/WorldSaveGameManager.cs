using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JH
{

    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] PlayerManager player;

        [Header("Save/Load")]
        [SerializeField] bool saveGame;// TEMP
        [SerializeField] bool loadGame;// TEMP

        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Game Data Writer")]
        private SaveGameDataWriter saveGameDataWriter;

        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlot;
        public CharacterSaveData currentCharacterData;
        private string fileName;

        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        //public CharacterSaveData characterSlot02;
        //public CharacterSaveData characterSlot03;
        //public CharacterSaveData characterSlot04;
        //public CharacterSaveData characterSlot05;

        private void Awake()
        {
            if (null == instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() // TEMP
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }
        private void DecideCharacterFileNameBasedOnCharacterSlot()
        {
            switch (currentCharacterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    fileName = "CharacterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                    fileName = "CharacterSlot_02";
                    break;
                case CharacterSlot.CharacterSlot_03:
                    fileName = "CharacterSlot_03";
                    break;
                case CharacterSlot.CharacterSlot_04:
                    fileName = "CharacterSlot_04";
                    break;
                case CharacterSlot.CharacterSlot_05:
                    fileName = "CharacterSlot_05";
                    break;
            }
        }

        public void CreateNewGame()
        {
            // create new file with a file name depending on which slot is used
            DecideCharacterFileNameBasedOnCharacterSlot();

            currentCharacterData = new CharacterSaveData();
        }

        public void LoadGame()
        {
            // load a saved file, with a file name depending on which slot is used
            DecideCharacterFileNameBasedOnCharacterSlot();

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //generally works on multiple machine types, safe to use
            saveGameDataWriter.saveFileName = fileName;
            currentCharacterData = saveGameDataWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {
            // Save the current file under a file name depending on which slot is used
            DecideCharacterFileNameBasedOnCharacterSlot();

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveGameDataWriter.saveFileName = fileName;

            // pass the player's info from game to their save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // write info onto a json file which is then saved to the machine
            saveGameDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
        }

        public IEnumerator LoadWorldScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}
