using UnityEngine;
using TMPro;

public class NoiseSource : MonoBehaviour
{
    public float range = 5f;
    public float delay = 3f;
    public float activeDuration = 1f;

    [Header("Visual do Raio")]
    [SerializeField] private Color corEspera = new Color(0.5f, 0.8f, 1f,  0.15f);
    [SerializeField] private Color corAtivo  = new Color(1f,   0.78f, 0f, 0.38f);
    [SerializeField] private int   segmentos = 48;

    [SerializeField] private TextMeshPro countdownText;

    [Header("Audio")]
    public AudioClip activationSound;
    [Range(0f, 1f)] public float activationVolume = 1f;

    private float    _timeAlive = 0f;
    private Material _mat;
    private bool     _activationSoundPlayed;

    public bool IsActive => _timeAlive >= delay && _timeAlive < delay + activeDuration;

    private void Start()
    {
        if (countdownText == null)
            countdownText = GetComponentInChildren<TextMeshPro>();
        if (delay <= 0f && countdownText != null)
            countdownText.gameObject.SetActive(false);

        CriarCirculo();
        Destroy(gameObject, delay + activeDuration);
    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;
        AtualizarTexto();
        AtualizarCor();

        if (IsActive && !_activationSoundPlayed)
        {
            _activationSoundPlayed = true;
            if (activationSound != null)
                AudioSource.PlayClipAtPoint(activationSound, transform.position, activationVolume);
        }
    }

    void AtualizarTexto()
    {
        if (countdownText == null) return;
        float r = delay - _timeAlive;
        countdownText.text = r > 0f ? r.ToString("0.0") + "s" : "!";
    }

    void CriarCirculo()
    {
        GameObject go = new GameObject("NoiseRadius_Visual");
        go.transform.SetParent(transform, false);

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        // Disco gerado uma única vez (range não muda durante o ciclo de vida)
        int n    = segmentos;
        var mesh = new Mesh { name = "NoiseMesh" };
        mf.mesh  = mesh;

        var verts = new Vector3[n + 1];
        var tris  = new int[n * 3];
        verts[0]  = Vector3.zero;
        float step = 360f / n;

        for (int i = 0; i < n; i++)
        {
            float a  = i * step * Mathf.Deg2Rad;
            verts[i + 1] = new Vector3(Mathf.Cos(a) * range, Mathf.Sin(a) * range, 0f);
        }
        for (int i = 0; i < n; i++)
        {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = (i + 1) % n + 1;
        }
        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        Shader shader = Shader.Find("Sprites/Default")
                     ?? Shader.Find("Universal Render Pipeline/Unlit");
        _mat         = new Material(shader) { color = corEspera };
        mr.material  = _mat;
        mr.sortingOrder = -1;
    }

    void AtualizarCor()
    {
        if (_mat == null) return;

        bool  ativo = IsActive;
        Color alvo  = ativo ? corAtivo : corEspera;

        if (ativo)
        {
            float t = Mathf.PingPong(_timeAlive * 4f, 1f);
            alvo.a  = Mathf.Lerp(corAtivo.a * 0.45f, corAtivo.a, t);
        }

        _mat.color = alvo;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
