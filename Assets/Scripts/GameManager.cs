using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int TotalObjectives { get; private set; }
    public int CollectedObjectives { get; private set; }
    public bool LevelCompleted { get; private set; }

    public bool HasAllObjectives => TotalObjectives > 0 && CollectedObjectives >= TotalObjectives;

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
        if (LevelCompleted) return;
        CollectedObjectives++;
        Debug.Log($"Objetivo coletado ({CollectedObjectives}/{TotalObjectives}).");
    }

    public void TryFinishLevel()
    {
        if (LevelCompleted) return;
        if (!HasAllObjectives)
        {
            int faltam = Mathf.Max(0, TotalObjectives - CollectedObjectives);
            Debug.Log($"Ainda faltam {faltam} objetivo(s) antes de sair.");
            return;
        }

        LevelCompleted = true;
        Debug.Log("Fase concluída!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
