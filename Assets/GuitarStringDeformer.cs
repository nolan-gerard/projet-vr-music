using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GuitarStringDeformer : MonoBehaviour
{
    [Header("Déclenchement")]
    public string handTag = "RightHand"; 

    [Header("Paramètres de vibration (en mètres réels)")]
    public float maxAmplitude = 0.003f;  // 3 mm de vibration au centre de la corde
    public float frequency = 70f;        // Rapidité de l'oscillation
    public float damping = 4f;           // Amortissement (durée de la vibration)

    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private Vector3[] _deformedVertices;
    private float _minX;
    private float _lengthX;
    private float _amplitude;
    private float _time;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _mesh.vertices;
        _deformedVertices = new Vector3[_originalVertices.Length];
        System.Array.Copy(_originalVertices, _deformedVertices, _originalVertices.Length);

        _minX = _mesh.bounds.min.x;
        _lengthX = Mathf.Max(_mesh.bounds.size.x, 1e-6f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(handTag)) return;

        Vector3 lossy = transform.lossyScale;
        float scaleFactor = Mathf.Max((Mathf.Abs(lossy.x) + Mathf.Abs(lossy.y) + Mathf.Abs(lossy.z)) / 3f, 0.0001f);

        _amplitude = maxAmplitude / scaleFactor;
        _time = 0f;
    }

    void Update()
    {
        if (_amplitude <= 1e-8f) return;

        _time += Time.deltaTime;
        float wave = Mathf.Sin(_time * frequency) * _amplitude;
        _amplitude *= Mathf.Exp(-damping * Time.deltaTime);

        for (int i = 0; i < _originalVertices.Length; i++)
        {
            Vector3 v = _originalVertices[i];

            // Maintient les deux extrémités fixes (0 et 1) et courbe le milieu (0.5)
            float normalizedX = Mathf.Clamp01((v.x - _minX) / _lengthX);
            float arch = Mathf.Sin(normalizedX * Mathf.PI);

            v.y += wave * arch;
            v.z += wave * 0.5f * arch;
            _deformedVertices[i] = v;
        }

        _mesh.vertices = _deformedVertices;
    }
}