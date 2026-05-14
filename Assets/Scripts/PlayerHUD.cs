using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private TMP_Text noiseCountText;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TMP_Text objectiveText;

    void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
        }
    }

    void Update()
    {
        if (player == null) return;

        if (noiseCountText != null)
        {
            noiseCountText.text = $"Barulhos: {player.NoisesRemaining}/{player.maxNoises}";
        }

        if (cooldownFill != null)
        {
            float remaining = player.NoiseCooldownRemaining;
            float ratio = player.noiseCooldown > 0f ? 1f - (remaining / player.noiseCooldown) : 1f;
            cooldownFill.fillAmount = Mathf.Clamp01(ratio);

            bool noNoisesLeft = player.NoisesRemaining <= 0;
            cooldownFill.color = noNoisesLeft ? new Color(0.4f, 0.4f, 0.4f) : Color.white;
        }

        if (objectiveText != null && GameManager.instance != null)
        {
            int  concluidas = GameManager.instance.CompletedDeliveries;
            int  total      = GameManager.instance.TotalDeliveries;
            bool tudo       = GameManager.instance.HasAllDeliveries;

            string carregando = player != null && player.IsCarrying
                ? $"Carregando: <b>{player.CarriedDisplayName}</b>\n"
                : "";

            if (tudo)
            {
                objectiveText.text  = $"{carregando}Entregas: {concluidas}/{total} — vá até a saída!";
                objectiveText.color = Color.green;
            }
            else
            {
                objectiveText.text  = $"{carregando}Entregas: {concluidas}/{total}";
                objectiveText.color = player != null && player.IsCarrying ? Color.yellow : Color.white;
            }
        }
    }
}
