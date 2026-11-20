using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(Collider))]
public class DrumMeshDeformer : MonoBehaviour
{
    [Header("Paramètres de déformation (en mètres réels)")]
    public string targetTag = "drumstick";
    public float deformRadius = 0.08f;     // 8 cm autour de la baguette
    public float maxIndent = 0.015f;       // 1,5 cm d'enfoncement max
    public float forceMultiplier = 0.01f;

    [Header("Ressort et amortissement")]
    public float springStiffness = 180f;
    public float damping = 12f;

    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private Vector3[] _normals;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;
    private Collider _collider;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _mesh.vertices;
        _normals = _mesh.normals;
        _displacedVertices = new Vector3[_originalVertices.Length];
        _vertexVelocities = new Vector3[_originalVertices.Length];
        System.Array.Copy(_originalVertices, _displacedVertices, _originalVertices.Length);
        _collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        float speed = 1f;
        var estimator = other.GetComponent<VelocityEstimator>();
        if (estimator != null)
            speed = estimator.GetVelocityEstimate().magnitude;

        Vector3 lossy = transform.lossyScale;
        float scaleFactor = Mathf.Max((Mathf.Abs(lossy.x) + Mathf.Abs(lossy.y) + Mathf.Abs(lossy.z)) / 3f, 0.0001f);

        float localRadius = deformRadius / scaleFactor;
        float localMaxIndent = maxIndent / scaleFactor;
        float localIndent = Mathf.Clamp(speed * forceMultiplier, 0f, maxIndent) / scaleFactor;

        Vector3 worldHit = _collider.ClosestPoint(other.transform.position);
        Vector3 localHit = transform.InverseTransformPoint(worldHit);
        Vector3 stickDirWorld = (other.transform.position - worldHit).normalized;

        for (int i = 0; i < _displacedVertices.Length; i++)
        {
            float dist = Vector3.Distance(_originalVertices[i], localHit);
            if (dist < localRadius)
            {
                Vector3 worldNormal = transform.TransformDirection(_normals[i]).normalized;
                if (stickDirWorld.sqrMagnitude > 0.001f && Vector3.Dot(worldNormal, stickDirWorld) < 0.2f)
                    continue;

                float falloff = 1f - (dist / localRadius);
                Vector3 pushDir = -_normals[i].normalized;
                _vertexVelocities[i] += pushDir * (localIndent * falloff * 15f);
            }
        }
    }

    void Update()
    {
        bool needsUpdate = false;

        for (int i = 0; i < _displacedVertices.Length; i++)
        {
            Vector3 displacement = _displacedVertices[i] - _originalVertices[i];
            if (displacement.sqrMagnitude > 1e-12f || _vertexVelocities[i].sqrMagnitude > 1e-12f)
            {
                Vector3 springForce = -springStiffness * displacement;
                _vertexVelocities[i] += springForce * Time.deltaTime;
                _vertexVelocities[i] *= Mathf.Clamp01(1f - damping * Time.deltaTime);
                _displacedVertices[i] += _vertexVelocities[i] * Time.deltaTime;
                needsUpdate = true;
            }
        }

        if (needsUpdate)
        {
            _mesh.vertices = _displacedVertices;
            _mesh.RecalculateNormals();
        }
    }
}