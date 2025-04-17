using UnityEngine;

public class UnitMoveStateDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Minimum movement distance to be considered moving (in meters)")]
    [SerializeField] public float _movementThreshold = 0.1f;
    [Tooltip("Ground detection layer")]
    [SerializeField] private LayerMask _groundLayer;
    [Tooltip("How often to check for movement (seconds)")]
    [SerializeField] private float _checkInterval = 0.1f;

    // Public properties
    public bool IsMoving { get; private set; }
    public bool IsGrounded { get; private set; }
    public float MovementMagnitude { get; private set; }
    
    private Vector3 _lastPosition;
    private float _lastCheckTime;
    public RaycastHit _groundHit;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        // Ground check (continuous)
        IsGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            Vector3.down,
            out _groundHit,
            0.4f,
            _groundLayer
        );

        // Movement check (interval-based)
        if (Time.time - _lastCheckTime >= _checkInterval)
        {
            Vector3 currentPosition = transform.position;
            MovementMagnitude = Vector3.Distance(currentPosition, _lastPosition);
            
            // Only consider movement if grounded and above threshold
            IsMoving = IsGrounded && MovementMagnitude >= _movementThreshold;
            
            _lastPosition = currentPosition;
            _lastCheckTime = Time.time;
        }
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (IsGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_groundHit.point, 0.1f);
        }
    }
}