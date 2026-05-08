using System;
using UnityEngine;
using UnityEngine.UI;

namespace Experience
{
    public class LevelUpWeaponOptionView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text nameText;
        [SerializeField] private Text actionText;
        [SerializeField] private Text statsText;

        private AttackData _attackData;
        private Action<AttackData> _optionSelected;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(Select);
                button.onClick.AddListener(Select);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(Select);
            }
        }

        public void SetOption(
            AttackData attackData,
            bool isEquipped,
            int currentLevel,
            int currentDamage,
            int nextDamage,
            float currentCooldown,
            float nextCooldown,
            Action<AttackData> optionSelected)
        {
            _attackData = attackData;
            _optionSelected = optionSelected;

            if (nameText != null)
            {
                nameText.text = attackData.attackName;
            }

            if (actionText != null)
            {
                string actionLabel = isEquipped ? "Upgrade" : "Equip";
                string levelLabel = isEquipped ? $"Lv {currentLevel} -> {currentLevel + 1}" : "New";
                actionText.text = $"{actionLabel} | {levelLabel}";
            }

            if (statsText != null)
            {
                string damageText = isEquipped ? $"Damage {currentDamage} -> {nextDamage}" : $"Damage {currentDamage}";
                string cooldownText = isEquipped ? $"Cooldown {currentCooldown:0.##}s -> {nextCooldown:0.##}s" : $"Cooldown {currentCooldown:0.##}s";
                statsText.text = $"{damageText}\n{cooldownText}";
            }
        }

        private void Select()
        {
            _optionSelected?.Invoke(_attackData);
        }
    }
}
