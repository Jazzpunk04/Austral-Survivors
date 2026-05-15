using System.Collections.Generic;
using Combat;
using UnityEngine;

namespace Experience
{
    public class LevelUpPopupUI : MonoBehaviour
    {
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private PlayerAttack playerAttack;
        [SerializeField] private LevelUpPopupView popupView;
        [SerializeField, Range(1, 3)] private int maxOptions = 3;
        [SerializeField] private bool pauseGameWhileChoosing = true;
        [SerializeField] private List<AttackData> availableAttacks = new();

        private readonly List<AttackData> _options = new();
        private float _previousTimeScale = 1f;
        private int _pendingLevelUps;
        private bool _isShowing;

        public IReadOnlyList<AttackData> AvailableAttacks => availableAttacks;

        private void Awake()
        {
            if (playerExperience == null)
            {
                playerExperience = GetComponent<PlayerExperience>();
            }

            if (playerAttack == null)
            {
                playerAttack = GetComponent<PlayerAttack>();
            }
        }

        private void OnEnable()
        {
            playerExperience.LeveledUp += HandleLeveledUp;
        }

        private void OnDisable()
        {
            playerExperience.LeveledUp -= HandleLeveledUp;
        }

        private void HandleLeveledUp(int level)
        {
            if (_isShowing)
            {
                _pendingLevelUps++;
                return;
            }

            Show(level);
        }

        private void Show(int level)
        {
            if (popupView == null)
            {
                Debug.LogWarning("Assign the disabled popup UI element to LevelUpPopupUI.", this);
                return;
            }

            BuildOptions();
            if (_options.Count == 0)
            {
                Debug.LogWarning("No level-up weapon options configured.", this);
                return;
            }

            _isShowing = true;
            if (pauseGameWhileChoosing)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            popupView.Show(level, _options, playerAttack, HandleOptionSelected);
        }

        private void BuildOptions()
        {
            _options.Clear();

            for (int i = 0; i < availableAttacks.Count && _options.Count < maxOptions; i++)
            {
                AttackData attackData = availableAttacks[i];
                if (attackData == null || ContainsOption(attackData))
                {
                    continue;
                }

                _options.Add(attackData);
            }
        }

        private bool ContainsOption(AttackData attackData)
        {
            for (int i = 0; i < _options.Count; i++)
            {
                if (_options[i] == attackData)
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleOptionSelected(AttackData attackData)
        {
            if (playerAttack.IsAttackEquipped(attackData))
            {
                playerAttack.UpgradeAttack(attackData);
            }
            else
            {
                playerAttack.EquipAttack(attackData);
            }

            Close();
        }

        private void Close()
        {
            if (popupView != null)
            {
                popupView.Hide();
            }

            if (pauseGameWhileChoosing)
            {
                Time.timeScale = _previousTimeScale;
            }

            _isShowing = false;

            if (_pendingLevelUps <= 0)
            {
                return;
            }

            _pendingLevelUps--;
            Show(playerExperience.CurrentLevel);
        }
    }
}
