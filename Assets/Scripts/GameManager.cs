using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float backgroundMusicVolume = 0.45f;

    [Header("Progressão de fases")]
    [Tooltip("Nome da próxima cena a carregar quando esta fase for concluída. Se vazio, recarrega a fase atual.")]
    [SerializeField] private string nextSceneName;
    [Tooltip("Tempo (s) antes de carregar a próxima fase após concluir.")]
    [SerializeField] private float nextSceneDelay = 0.45f;

    private readonly HashSet<DeliveryPoint> _deliveryPoints     = new HashSet<DeliveryPoint>();
    private readonly HashSet<DeliveryPoint> _completedDeliveries = new HashSet<DeliveryPoint>();

    public int  TotalDeliveries     => _deliveryPoints.Count;
    public int  CompletedDeliveries => _completedDeliveries.Count;
    public bool LevelCompleted      { get; private set; }

    public bool HasAllDeliveries => TotalDeliveries > 0 && CompletedDeliveries >= TotalDeliveries;

    private bool _isEnding;
    private static AudioSource _persistentMusicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Time.timeScale = 1f;
            PlayBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DeliveryPoint[] dps = FindObjectsByType<DeliveryPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var dp in dps)
            RegisterDeliveryPoint(dp);
    }

    public void RegisterDeliveryPoint(DeliveryPoint dp)
    {
        if (dp == null) return;
        _deliveryPoints.Add(dp);
    }

    public void CompleteDelivery(DeliveryPoint dp)
    {
        if (_isEnding || dp == null) return;
        RegisterDeliveryPoint(dp);
        if (!_completedDeliveries.Add(dp)) return;
        Debug.Log($"Entrega concluída ({CompletedDeliveries}/{TotalDeliveries}).");
    }

    public void TryFinishLevel()
    {
        if (_isEnding) return;
        if (!HasAllDeliveries)
        {
            int faltam = TotalDeliveries - CompletedDeliveries;
            Debug.Log($"Ainda faltam {faltam} entrega(s) antes de sair.");
            return;
        }

        LevelCompleted = true;
        _isEnding = true;
        Debug.Log("Fase concluída!");
        StartCoroutine(LoadNextSceneAfterDelay(nextSceneDelay));
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            CameraJuice.Shake(0.16f, 0.25f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        CameraJuice.Shake(0.16f, 0.25f);
        StartCoroutine(ShowGameOverAfterDelay(Mathf.Max(0f, reloadDelay)));
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ShowGameOverAfterDelay(float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        GameOverScreen.GetOrCreate(mainMenuSceneName).Show();
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        if (_persistentMusicSource == null)
        {
            GameObject musicHost = new GameObject("PersistentBackgroundMusic");
            DontDestroyOnLoad(musicHost);
            _persistentMusicSource = musicHost.AddComponent<AudioSource>();
            _persistentMusicSource.loop = true;
            _persistentMusicSource.playOnAwake = false;
            _persistentMusicSource.spatialBlend = 0f;
        }

        _persistentMusicSource.volume = backgroundMusicVolume;

        if (_persistentMusicSource.clip != backgroundMusic)
        {
            _persistentMusicSource.clip = backgroundMusic;
            _persistentMusicSource.Play();
        }
        else if (!_persistentMusicSource.isPlaying)
        {
            _persistentMusicSource.Play();
        }
    }
}
