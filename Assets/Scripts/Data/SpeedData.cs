using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SpeedData", menuName = "Data/Speed Data")]
    public class SpeedData : ScriptableObject
    {
        [SerializeField][Range(1,10)]private int speedValue = 1;

        public int SpeedValue => speedValue;
    }
}