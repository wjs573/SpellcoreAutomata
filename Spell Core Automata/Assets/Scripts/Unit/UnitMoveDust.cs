using UnityEngine;

[RequireComponent(typeof(UnitMoveStateDetector))]
public class UnitMoveDust : MonoBehaviour
{
    [Header("Particle Settings")]
    [Tooltip("Dust particle prefab (should be looping)")]
    [SerializeField] private ParticleSystem _dustParticlePrefab;
    [Tooltip("Spawn offset from character feet")]
    [SerializeField] private Vector3 _spawnOffset = new Vector3(0, 0.05f, 0);
    [Tooltip("How quickly particle emission ramps up/down")]
    [SerializeField] private float _emissionSmoothTime = 0.3f;
    [Tooltip("Maximum emission rate when moving")]
    [SerializeField] private float _maxEmissionRate = 20f;

    private UnitMoveStateDetector _moveDetector;
    private ParticleSystem _dustParticle;
    private ParticleSystem.EmissionModule _emission;
    private float _currentEmissionRate;
    private float _emissionVelocity;

    private void Awake()
    {
        _moveDetector = GetComponent<UnitMoveStateDetector>();
        InitializeParticle();
    }

    private void InitializeParticle()
    {
        if (_dustParticlePrefab == null)
        {
            Debug.LogWarning("Dust particle prefab not assigned!");
            return;
        }

        _dustParticle = Instantiate(_dustParticlePrefab, transform);
        _dustParticle.transform.localPosition = _spawnOffset;
        _emission = _dustParticle.emission;
        _currentEmissionRate = 0f;
        _emission.enabled = true; // Keep emission always enabled
    }

    private void Update()
    {
        if (_dustParticle == null) return;

        // Calculate target emission rate based on movement state
        float targetEmission = _moveDetector.IsMoving ? 
            Mathf.Lerp(0, _maxEmissionRate, _moveDetector.MovementMagnitude / _moveDetector._movementThreshold) : 
            0f;

        // Smoothly transition emission rate
        _currentEmissionRate = Mathf.SmoothDamp(
            _currentEmissionRate,
            targetEmission,
            ref _emissionVelocity,
            _emissionSmoothTime
        );

        // Apply emission rate
        _emission.rateOverTime = _currentEmissionRate;

        // Adjust particle position to stay on ground
        if (_moveDetector.IsGrounded)
        {
            _dustParticle.transform.position = _moveDetector._groundHit.point + _spawnOffset;
        }
    }
}