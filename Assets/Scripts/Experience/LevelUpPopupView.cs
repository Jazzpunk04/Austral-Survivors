using System;
using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Experience
{
    public class LevelUpPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text titleText;
        [SerializeField] private List<LevelUpWeaponOptionView> optionViews = new();

        private void Awake()
        {
            if (root == null)
            {
                root = gameObject;
            }
        }

        public void Show(int level, IReadOnlyList<AttackData> options, PlayerAttack playerAttack, Action<AttackData> optionSelected)
        {
            if (root == null)
            {
                root = gameObject;
            }

            if (titleText != null)
            {
                titleText.text = $"Level {level}";
            }

            for (int i = 0; i < optionViews.Count; i++)
            {
                bool hasOption = i < options.Count;
                optionViews[i].gameObject.SetActive(hasOption);

                if (hasOption)
                {
                    AttackData attackData = options[i];
                    bool isEquipped = playerAttack.IsAttackEquipped(attackData);
                    int currentLevel = playerAttack.GetAttackLevel(attackData);
                    int displayLevel = Mathf.Max(1, currentLevel);
                    int currentDamage = playerAttack.GetAttackDamageAtLevel(attackData, displayLevel);
                    int nextDamage = playerAttack.GetAttackDamageAtLevel(attackData, displayLevel + 1);
                    float currentCooldown = playerAttack.GetAttackCooldownAtLevel(attackData, displayLevel);
                    float nextCooldown = playerAttack.GetAttackCooldownAtLevel(attackData, displayLevel + 1);

                    optionViews[i].SetOption(
                        attackData,
                        isEquipped,
                        currentLevel,
                        currentDamage,
                        nextDamage,
                        currentCooldown,
                        nextCooldown,
                        optionSelected);
                }
            }

            root.SetActive(true);
        }

        public void Hide()
        {
            if (root == null)
            {
                root = gameObject;
            }

            root.SetActive(false);
        }
    }
}
