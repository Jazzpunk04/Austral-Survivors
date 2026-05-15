using System;
using UnityEngine;

namespace Experience
{
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField, Min(0)] private int currentExperience;
        [SerializeField, Min(1)] private int currentLevel = 1;
        [SerializeField, Min(1)] private int experienceToNextLevel = 5;
        [SerializeField, Min(0)] private int experiencePerLevelIncrease = 5;

        public int CurrentExperience => currentExperience;
        public int CurrentLevel => currentLevel;
        public int ExperienceToNextLevel => experienceToNextLevel;

        public event Action<int> LeveledUp;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExperience += amount;
            ProcessLevelUps();
        }

        public void RestoreState(int savedExperience, int savedLevel, int savedExperienceToNextLevel)
        {
            currentExperience = Mathf.Max(0, savedExperience);
            currentLevel = Mathf.Max(1, savedLevel);
            experienceToNextLevel = Mathf.Max(1, savedExperienceToNextLevel);
        }

        private void ProcessLevelUps()
        {
            while (currentExperience >= experienceToNextLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel += experiencePerLevelIncrease;

                LeveledUp?.Invoke(currentLevel);
            }
        }
    }
}
