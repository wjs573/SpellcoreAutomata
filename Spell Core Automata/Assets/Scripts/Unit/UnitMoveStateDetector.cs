using UnityEngine;

public class UnitMoveStateDetector : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("移动速度判定阈值（避免微小移动误判）")]
    [SerializeField] float _speedThreshold = 0.3f;
    [Tooltip("地面检测层级")]
    [SerializeField] LayerMask _groundLayer;

    private Rigidbody _rb;
    private Vector3 _lastPosition;
    public bool IsMoving { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _lastPosition = transform.position;
    }

    private void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        // 实时速度计算（无间隔检测）
        CurrentSpeed = _rb != null
            ? _rb.velocity.magnitude
            : (transform.position - _lastPosition).magnitude / Time.deltaTime;

        IsMoving = CurrentSpeed > _speedThreshold;
        _lastPosition = transform.position;

        // 地面检测
        IsGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.2f,
            _groundLayer
        );
    }
}