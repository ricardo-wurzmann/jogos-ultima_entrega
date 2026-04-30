using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int TotalObjectives { get; private set; }
    public int CollectedObjectives { get; private set; }
    public bool LevelCompleted { get; private set; }

    public bool HasAllObjectives => TotalObjectives > 0 && CollectedObjectives >= TotalObjectives;

    private bool _isEnding;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterObjective()
    {
        TotalObjectives++;
    }

    public void CollectObjective()
    {
        if (_isEnding) return;
        CollectedObjectives++;
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
}
