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
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;

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
            LoadAllCharacterProfiles();
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
        public string DecideCharacterFileNameBasedOnCharacterSlot(CharacterSlot designatedCharacterSlot)
        {
            string scopedFileName = "";
            switch (designatedCharacterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    scopedFileName = "CharacterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                    scopedFileName = "CharacterSlot_02";
                    break;
                case CharacterSlot.CharacterSlot_03:
                    scopedFileName = "CharacterSlot_03";
                    break;
                case CharacterSlot.CharacterSlot_04:
                    scopedFileName = "CharacterSlot_04";
                    break;
                case CharacterSlot.CharacterSlot_05:
                    scopedFileName = "CharacterSlot_05";
                    break;
            }

            return scopedFileName;
        }

        public string GetCharacterName(CharacterSlot designatedCharacterSlot)
        {
            switch (designatedCharacterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    return characterSlot01.characterName;
                case CharacterSlot.CharacterSlot_02:
                    return characterSlot02.characterName;
                case CharacterSlot.CharacterSlot_03:
                    return characterSlot03.characterName;
                case CharacterSlot.CharacterSlot_04:
                    return characterSlot04.characterName;
                case CharacterSlot.CharacterSlot_05:
                    return characterSlot05.characterName;
            }

            return string.Empty;
        }

        public void CreateNewGame()
        {
            // create new file with a file name depending on which slot is used
            fileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

            currentCharacterData = new CharacterSaveData();
        }

        public void LoadGame()
        {
            // load a saved file, with a file name depending on which slot is used
            fileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //generally works on multiple machine types, safe to use
            saveGameDataWriter.saveFileName = fileName;
            currentCharacterData = saveGameDataWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {
            // Save the current file under a file name depending on which slot is used
            fileName = DecideCharacterFileNameBasedOnCharacterSlot(currentCharacterSlot);

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveGameDataWriter.saveFileName = fileName;

            // pass the player's info from game to their save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // write info onto a json file which is then saved to the machine
            saveGameDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
        }

        /* Pre-Load all Character Profiles when starting game */
        private void LoadAllCharacterProfiles()
        {
            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            for (int i = 0; i < (int)CharacterSlot.END; ++i)
            {
                CharacterSlot slot = (CharacterSlot)System.Enum.Parse(typeof(CharacterSlot), $"CharacterSlot_0{i + 1}");
                saveGameDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlot(slot);
                var profile = saveGameDataWriter.LoadSaveFile();

                switch (i)
                {
                    case 0/*CharacterSlot_01*/: characterSlot01 = profile; break;
                    case 1/*CharacterSlot_02*/: characterSlot02 = profile; break;
                    case 2/*CharacterSlot_03*/: characterSlot03 = profile; break;
                    case 3/*CharacterSlot_04*/: characterSlot04 = profile; break;
                    case 4/*CharacterSlot_05*/: characterSlot05 = profile; break;
                }
            }
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
