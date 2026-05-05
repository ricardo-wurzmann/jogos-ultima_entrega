using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Audio")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float backgroundMusicVolume = 0.45f;

    private readonly HashSet<Objective> _registeredObjectives = new HashSet<Objective>();
    private readonly HashSet<Objective> _collectedObjectives = new HashSet<Objective>();

    public int TotalObjectives => _registeredObjectives.Count;
    public int CollectedObjectives => _collectedObjectives.Count;
    public bool LevelCompleted { get; private set; }

    public bool HasAllObjectives => TotalObjectives > 0 && CollectedObjectives >= TotalObjectives;

    private bool _isEnding;
    private AudioSource _backgroundMusicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PlayBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Objective[] objectives = FindObjectsByType<Objective>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Objective objective in objectives)
        {
            RegisterObjective(objective);
        }
    }

    public void RegisterObjective(Objective objective)
    {
        if (objective == null) return;
        _registeredObjectives.Add(objective);
    }

    public void CollectObjective(Objective objective)
    {
        if (_isEnding) return;
        if (objective == null) return;

        RegisterObjective(objective);
        if (!_collectedObjectives.Add(objective)) return;

        Debug.Log($"Objetivo coletado ({CollectedObjectives}/{TotalObjectives}).");
    }

    public void TryFinishLevel()
    {
        if (_isEnding) return;
        if (!HasAllObjectives)
        {
            int faltam = Mathf.Max(0, TotalObjectives - CollectedObjectives);
            Debug.Log($"Ainda faltam {faltam} objetivo(s) antes de sair.");
            return;
        }

        LevelCompleted = true;
        _isEnding = true;
        Debug.Log("Fase concluida!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        GameOver(0f);
    }

    public void GameOver(float reloadDelay)
    {
        if (_isEnding) return;
        _isEnding = true;

        if (reloadDelay <= 0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        StartCoroutine(ReloadSceneAfterDelay(reloadDelay));
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        _backgroundMusicSource = GetComponent<AudioSource>();
        if (_backgroundMusicSource == null)
        {
            _backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        }

        _backgroundMusicSource.clip = backgroundMusic;
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.playOnAwake = false;
        _backgroundMusicSource.spatialBlend = 0f;
        _backgroundMusicSource.volume = backgroundMusicVolume;

        if (!_backgroundMusicSource.isPlaying)
        {
            _backgroundMusicSource.Play();
        }
    }
}
