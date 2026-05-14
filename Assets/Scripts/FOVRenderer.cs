using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class FOVRenderer : MonoBehaviour
{
    [Header("Aparência")]
    public Color corNormal = new Color(1f, 0.92f, 0.016f, 0.25f);
    public Color corAlerta = new Color(1f, 0.15f, 0f, 0.50f);
    [Range(8, 80)]
    public int qtdRaios = 40;

    private FieldOfView  _fov;
    private Mesh         _mesh;
    private Material     _mat;
    private MeshRenderer _mr;

    void Awake()
    {
        _fov = GetComponent<FieldOfView>();

        GameObject go = new GameObject("FOV_Cone");
        go.transform.SetParent(transform, false);

        var mf = go.AddComponent<MeshFilter>();
        _mr    = go.AddComponent<MeshRenderer>();

        _mesh    = new Mesh { name = "FOV_ConeMesh" };
        mf.mesh  = _mesh;

        Shader shader = Shader.Find("Sprites/Default")
                     ?? Shader.Find("Universal Render Pipeline/Unlit");
        _mat          = new Material(shader) { color = corNormal };
        _mr.material  = _mat;

        // Herda sorting layer do sprite do inimigo e fica um nível atrás
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            _mr.sortingLayerID = sr.sortingLayerID;
            _mr.sortingOrder   = sr.sortingOrder - 1;
        }
        else
        {
            _mr.sortingOrder = -1;
        }
    }

    void LateUpdate()
    {
        if (_fov == null) return;
        _mat.color = _fov.canSeePlayer ? corAlerta : corNormal;
        ReconstruirMalha();
    }

    void ReconstruirMalha()
    {
        int n     = qtdRaios;
        var verts = new Vector3[n + 2];
        var tris  = new int[n * 3];

        verts[0]  = Vector3.zero;
        float passo = _fov.angle / n;

        for (int i = 0; i <= n; i++)
        {
            float   graus = -_fov.angle / 2f + passo * i;
            Vector3 dir   = DirecaoDoAngulo(graus);

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, dir, _fov.radius, _fov.obstructionMask);

            float   dist    = hit.collider ? hit.distance : _fov.radius;
            verts[i + 1]    = transform.InverseTransformPoint(transform.position + dir * dist);
        }

        for (int i = 0; i < n; i++)
        {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        _mesh.Clear();
        _mesh.vertices  = verts;
        _mesh.triangles = tris;
        _mesh.RecalculateNormals();
    }

    // Mesma lógica do FieldOfView.DirectionFromAngle
    Vector3 DirecaoDoAngulo(float graus)
    {
        graus -= transform.eulerAngles.z;
        return new Vector3(
            Mathf.Sin(graus * Mathf.Deg2Rad),
            Mathf.Cos(graus * Mathf.Deg2Rad),
            0f);
    }
}
