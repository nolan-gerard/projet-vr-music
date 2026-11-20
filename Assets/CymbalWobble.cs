using UnityEngine;

public class CymbalWobble : MonoBehaviour
{
    public string targetTag = "drumstick";
    public float tiltAmount = 12f;
    public float wobbleSpeed = 18f;
    public float damping = 3f;

    private Quaternion _initialRot;
    private float _amplitude;
    private float _time;
    private Vector3 _tiltAxis;

    void Start() => _initialRot = transform.localRotation;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        float speed = 1f;
        var estimator = other.GetComponent<VelocityEstimator>();
        if (estimator != null)
            speed = estimator.GetVelocityEstimate().magnitude;

        Vector3 localHit = transform.InverseTransformPoint(other.transform.position);
        _tiltAxis = Vector3.Cross(Vector3.up, localHit.normalized);
        if (_tiltAxis.sqrMagnitude < 0.001f) _tiltAxis = Vector3.right;

        _amplitude = Mathf.Clamp(speed * tiltAmount, 2f, 25f);
        _time = 0f;
    }

    void Update()
    {
        if (_amplitude < 0.05f) return;

        _time += Time.deltaTime;
        float angle = Mathf.Sin(_time * wobbleSpeed) * _amplitude;
        _amplitude *= Mathf.Exp(-damping * Time.deltaTime);
        transform.localRotation = _initialRot * Quaternion.AngleAxis(angle, _tiltAxis);
    }
}
