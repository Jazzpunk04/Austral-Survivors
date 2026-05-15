using System;
using System.Collections.Generic;
using System.IO;
using Combat;
using Data;
using Experience;
using Health;
using UnityEngine;
using UnityEngine.SceneManagement;
using Waves;

[Serializable]
public class SavedGameState
{
    public int version = 1;
    public string savedAtUtc;
    public int sceneBuildIndex;
    public PlayerState player = new();
    public WaveState wave = new();
    public List<StateExtension> extensions = new();
}

[Serializable]
public class PlayerState
{
    public int level = 1;
    public int currentExperience;
    public int experienceToNextLevel = 5;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public Vector3 position;
    public List<WeaponState> weapons = new();
}

[Serializable]
public class WeaponState
{
    public string id;
    public int level = 1;
}

[Serializable]
public class WaveState
{
    public int currentWaveIndex;
    public int completedWaveCount;
    public bool allWavesCompleted;
}

[Serializable]
public class StateExtension
{
    public string key;
    public string json;
}

public class GameStateManager : MonoBehaviour
{
    private const string SaveFileName = "savegame.json";

    public static GameStateManager Instance { get; private set; }
    public static SavedGameState CurrentState { get; private set; }
    public static bool HasSavedGame => File.Exists(SavePath);

    private static bool _continueRequested;

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool GetSavedGame()
    {
        return HasSavedGame;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadFromDisk();
    }

    private void Start()
    {
        ConsumeContinueStateIfNeeded();
    }

    public static void StartNewGame(int sceneBuildIndex)
    {
        _continueRequested = false;
        CurrentState = null;
        DeleteSave();
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public static bool ContinueSavedGame(int fallbackSceneBuildIndex = -1)
    {
        if (!LoadFromDisk())
        {
            if (fallbackSceneBuildIndex >= 0)
            {
                StartNewGame(fallbackSceneBuildIndex);
            }

            return false;
        }

        _continueRequested = true;
        int sceneBuildIndex = CurrentState.sceneBuildIndex >= 0 ? CurrentState.sceneBuildIndex : fallbackSceneBuildIndex;
        if (sceneBuildIndex < 0)
        {
            Debug.LogWarning("Saved game has no valid scene build index.");
            return false;
        }

        SceneManager.LoadScene(sceneBuildIndex);
        return true;
    }

    public static void SaveRoundFinished(int completedWaveIndex, int nextWaveIndex, bool allWavesCompleted)
    {
        SavedGameState state = CaptureCurrentState();
        state.wave.completedWaveCount = Mathf.Max(0, completedWaveIndex + 1);
        state.wave.currentWaveIndex = Mathf.Max(0, nextWaveIndex);
        state.wave.allWavesCompleted = allWavesCompleted;

        Save(state);
    }

    public static SavedGameState CaptureCurrentState()
    {
        SavedGameState state = new()
        {
            savedAtUtc = DateTime.UtcNow.ToString("O"),
            sceneBuildIndex = SceneManager.GetActiveScene().buildIndex
        };

        CapturePlayerState(state.player);

        WaveHandler waveHandler = FindFirstObjectByType<WaveHandler>();
        if (waveHandler != null)
        {
            state.wave.currentWaveIndex = waveHandler.CurrentWaveIndex;
            state.wave.completedWaveCount = waveHandler.CompletedWaveCount;
            state.wave.allWavesCompleted = waveHandler.AllWavesCompleted;
        }

        CurrentState = state;
        return state;
    }

    public static void Save(SavedGameState state)
    {
        if (state == null)
        {
            return;
        }

        CurrentState = state;
        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(SavePath, json);
    }

    public static bool LoadFromDisk()
    {
        if (!File.Exists(SavePath))
        {
            CurrentState = null;
            return false;
        }

        string json = File.ReadAllText(SavePath);
        CurrentState = JsonUtility.FromJson<SavedGameState>(json);
        return CurrentState != null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        CurrentState = null;
    }

    public void ApplyContinueStateIfNeeded()
    {
        ConsumeContinueStateIfNeeded();
    }

    public static void ConsumeContinueStateIfNeeded()
    {
        if (!_continueRequested || CurrentState == null)
        {
            return;
        }

        ApplyStateToScene(CurrentState);
        _continueRequested = false;
    }

    public static void ApplyStateToScene(SavedGameState state)
    {
        if (state == null)
        {
            return;
        }

        PlayerExperience experience = FindFirstObjectByType<PlayerExperience>();
        if (experience != null)
        {
            experience.RestoreState(state.player.currentExperience, state.player.level, state.player.experienceToNextLevel);
        }

        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        if (stats != null)
        {
            stats.RefreshFromExperience();
        }

        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
        if (health != null)
        {
            health.RestoreState(state.player.currentHealth, state.player.maxHealth);
        }

        PlayerAttack attack = FindFirstObjectByType<PlayerAttack>();
        if (attack != null)
        {
            attack.RestoreWeapons(state.player.weapons, FindKnownAttacks(attack));
        }

        if (state.player.position != Vector3.zero)
        {
            Controllers.PlayerController player = FindFirstObjectByType<Controllers.PlayerController>();
            if (player != null)
            {
                player.transform.position = state.player.position;
            }
        }

        WaveHandler waveHandler = FindFirstObjectByType<WaveHandler>();
        if (waveHandler != null)
        {
            waveHandler.RestoreState(state.wave.currentWaveIndex, state.wave.completedWaveCount, state.wave.allWavesCompleted);
        }
    }

    private static void CapturePlayerState(PlayerState playerState)
    {
        PlayerExperience experience = FindFirstObjectByType<PlayerExperience>();
        if (experience != null)
        {
            playerState.level = experience.CurrentLevel;
            playerState.currentExperience = experience.CurrentExperience;
            playerState.experienceToNextLevel = experience.ExperienceToNextLevel;
        }

        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
        if (health != null)
        {
            playerState.currentHealth = health.CurrentHealth;
            playerState.maxHealth = health.MaxHealth;
        }

        PlayerAttack attack = FindFirstObjectByType<PlayerAttack>();
        if (attack != null)
        {
            playerState.weapons = attack.CaptureWeapons();
        }

        Controllers.PlayerController player = FindFirstObjectByType<Controllers.PlayerController>();
        if (player != null)
        {
            playerState.position = player.transform.position;
        }
    }

    private static List<AttackData> FindKnownAttacks(PlayerAttack playerAttack)
    {
        List<AttackData> attacks = new();
        AddUniqueAttacks(attacks, playerAttack.EquippedAttacks);

        LevelUpPopupUI popup = FindFirstObjectByType<LevelUpPopupUI>();
        if (popup != null)
        {
            AddUniqueAttacks(attacks, popup.AvailableAttacks);
        }

        return attacks;
    }

    private static void AddUniqueAttacks(List<AttackData> target, IReadOnlyList<AttackData> source)
    {
        if (source == null)
        {
            return;
        }

        for (int i = 0; i < source.Count; i++)
        {
            AttackData attack = source[i];
            if (attack != null && !target.Contains(attack))
            {
                target.Add(attack);
            }
        }
    }
}
