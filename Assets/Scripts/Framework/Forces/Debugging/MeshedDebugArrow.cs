using UnityEngine;

public class MeshedDebugArrow : DebugArrow
{
    private Renderer _renderer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _mesh;
    private Color _emissionColor;

    private Vector3[] _vertices;
    private static int[] _triangles = 
    {
        0, 1, 6,
        1, 5, 6,
        2, 3, 4
    };
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Vector2 arrowHeadDimensions = new Vector2(0.4f, 0.4f);
    [SerializeField] private float arrowWidth = 0.2f;
    [SerializeField] private Force asForce;

    public Renderer Renderer =>
        _renderer ??= GetComponent<Renderer>();

    public MeshFilter MeshFilter =>
        _meshFilter ??= GetComponent<MeshFilter>();

    public Mesh Mesh => _mesh ??= new Mesh();

    public MeshRenderer MeshRenderer =>
        _meshRenderer ??= GetComponent<MeshRenderer>();

    public Vector2 ArrowHeadDimensions => arrowHeadDimensions;
    public float BaseLength => Direction.magnitude * ScaleModifier;
    public float ArrowWidth => arrowWidth;
    public override Color Color
    {
        get => _emissionColor;
        set => _emissionColor = value;
    }
    public override Vector2 Direction => Force.CurrentForce == Vector3.zero ?
        Force.Direction : Force.CurrentForce;
    public override ForceDebugInfo DebugInfo { get; set; }
    
    private void Update()
    {
        SetPosition();
        RenderArrow();
    }
    
    private void RenderArrow()
    {
        CreateShape();
        UpdateMesh();
        RotateTowardsDirection();
        Renderer.material.SetColor(EmissionColorId, _emissionColor);
        asForce = DebugInfo.Force as Force;
    }

    private void CreateShape()
    {
        _vertices = new[]
        {
            new Vector3(0, arrowWidth, 0),
            new Vector3(BaseLength, arrowWidth, 0),
            new Vector3(BaseLength, arrowHeadDimensions.y, 0),

            new Vector3(BaseLength + arrowHeadDimensions.x, 0, 0),

            new Vector3(BaseLength, -arrowHeadDimensions.y, 0),
            new Vector3(BaseLength, -arrowWidth, 0),
            new Vector3(0, -arrowWidth, 0)
        };
    }

    private void UpdateMesh()
    {
        Mesh.Clear();
        Mesh.vertices = _vertices;
        Mesh.triangles = _triangles;
        MeshFilter.mesh = Mesh;
    }

    private void RotateTowardsDirection()
    {
        Vector3 normalized = Direction.normalized;
        var eulerAngles = new Vector3(0, 0, Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg);
        transform.eulerAngles = eulerAngles;
    }

    private void SetPosition()
    {
        // todo: dit zou nog wat minder hard gekoppeld kunnen worden. Arrows kennen nu hard dat er gedebugged wordt voor forces
        transform.position = BodyDebugInfo.ForceBody.transform.position + Offset;
    }
}
