using UnityEngine;

namespace Experience
{
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField, Min(0)] private int currentExperience;

        public int CurrentExperience => currentExperience;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExperience += amount;
            Debug.Log($"Player gained {amount} experience. Total: {currentExperience}", this);
        }
    }
}
