using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCFW
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Data")]
        public int currentLevel; // current level number, accessible via Singleton
        public Transform levelDataParent; // parent transform for levels
        public LevelConfig current; // container for current level config
        public List<LevelConfig> levels = new List<LevelConfig>(); // list of all levels

        [Header("Cycles Config")]
        [Tooltip("How much levels in a cycle - global value for all cycles.")]
        public int cycleLength = 1; // how much levels in a cycle. If 1 then levels are linear: 1,2,3, if 2 then each cycle has 2 levels etc
        private int currentLevelInCycle = 1; // counter for current level in cycle
        private int currentLevelIndexInCycle = 0; // indexing

        public int gameCycleIndex; // current game cycle index
        private int dataGameCycleIndex; // just a reference to data stored

        [Header("Cycle Data")]
        public List<LevelsCycle> levelCycleData = new List<LevelsCycle>();

        [Header("Debug")]
        public LevelsCycle activeLevelCycle;

        int lastRandomLevelIndex;

        public LevelConfig Current // returns current level or in case max level is reached, last level
        {
            get
            {
                current = levels[GetRealCurrentLevelIndexFromCycle() - 1];
                return current;
            }
        }

        private void Awake()
        {
            foreach (Transform child in levelDataParent)
            {
                levels.Add(child.GetComponent<LevelConfig>()); // grab all child levels in level container transform
                child.gameObject.SetActive(false);
            }
        }

        // Use this for initialization
        void Start()
        {
            currentLevel = GameManager.Instance.activeProfile.currentLevel;
            InitLevelCycleData(); // calculate everything on the cycle part at start, from data
            current = Current;

            // first level cycle data with index 0 was set by GameManager at creation of profile
        }

        public void IncreaseCurrentLevel()
        {
            currentLevel += 1; // update local level reference
            UpdateLevelCycleData();
            GameManager.Instance.activeProfile.currentLevel += 1; // update active profile level reference
            DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile; // and save to json because in JSON current level is stored
        }

        void InitLevelCycleData()
        {
            LevelsCycle lc = GameManager.Instance.activeProfile.currentLevelsCycleData;
            dataGameCycleIndex = lc.cycleIndex;
            gameCycleIndex = dataGameCycleIndex;
            activeLevelCycle = lc;
            int maxLevelForCycle = (gameCycleIndex * cycleLength) + cycleLength;
            int totalLevelsInPastCycles = gameCycleIndex * cycleLength; // eg cycle index 3, with 3 levels would mean 9, if player is on cycle 4
            currentLevelInCycle = currentLevel - totalLevelsInPastCycles; // eg level 13 on cycles of 3 is 1st level of cycle 4 lol
            currentLevelIndexInCycle = currentLevelInCycle - 1; // for ze indexss kek
        }

        int GetRealCurrentLevelIndexFromCycle()
        {
            return activeLevelCycle.levelNumbers[currentLevelInCycle - 1];
        }

        void UpdateLevelCycleData()
        {
            currentLevelInCycle += 1;
            currentLevelIndexInCycle += 1;
            if (currentLevelInCycle > cycleLength)
            {
                gameCycleIndex += 1;
                GameManager.Instance.activeProfile.currentLevelsCycleData = GetCycleByCycleIndex(gameCycleIndex);
            }
        }

        public List<int> GenerateRandomLevelCycleLevels(List<LevelConfig> levels)
        {
            List<int> result = new List<int>();

            int n = 0;
            while (n < cycleLength)
            {
                int randLevelNumber = Random.Range(1, levels.Count + 1); // actual level number as it's easier later
                if (!result.Contains(randLevelNumber))
                {
                    result.Add(randLevelNumber);
                    n++;
                }
            }
            return result;
        }

        public LevelsCycle GetCycleByCycleIndex(int ind)
        {
            if (ind > levelCycleData.Count - 1)
            {
                LevelsCycle newLc = new LevelsCycle()
                {
                    cycleIndex = gameCycleIndex,
                    cycleName = "Random" + Random.Range(1000, 10000),
                    levelNumbers = GenerateRandomLevelCycleLevels(levels)
                    //rewardShopItemIndex = -1
                };
                GameManager.Instance.activeProfile.currentLevelsCycleData = newLc;
                currentLevelInCycle = 1;
                currentLevelIndexInCycle = 0;
                return newLc;
            }
            else
            {
                levelCycleData[ind].cycleIndex = gameCycleIndex;
                return levelCycleData[ind];
            }

        }

        #region Testing

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                //CalculateLevelCycleData();
            }
        }

        #endregion   

    }


    [System.Serializable]
    public class LevelData
    {
        public GameObject levelDataContainer; // this is just reference to child transform where level objects and similar can be located. But also to LevelConfig.
    }

    [System.Serializable]
    public class LevelsCycle
    {
        [Tooltip("Cycle name, if used for anything")]
        public string cycleName;
        [HideInInspector]
        public int cycleIndex = 0;
        //public int rewardShopItemIndex = -1;
        [Tooltip("Level NUMBERS (eg 1,2,3 not 0,1,2 etc) to include in cycle")]
        public List<int> levelNumbers = new List<int>();
    }

}
