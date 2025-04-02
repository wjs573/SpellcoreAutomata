using UnityEngine;

[RequireComponent(typeof(UnitMoveStateDetector))]
public class UnitMoveDust : MonoBehaviour
{
    [Header("粒子配置")]
    [Tooltip("扬尘粒子预制件（需开启Looping）")]
    [SerializeField] ParticleSystem _dustParticlePrefab;
    [Tooltip("粒子生成位置偏移（角色脚部位置）")]
    [SerializeField] Vector3 _spawnOffset = new Vector3(0, 0.05f, 0);
    [Tooltip("最小移动速度触发阈值")]
    [SerializeField] float _moveSpeedThreshold = 0.5f;

    private UnitMoveStateDetector _moveDetector;
    private ParticleSystem _currentParticle;
    private ParticleSystem.EmissionModule _emission;

    private void Awake()
    {
        _moveDetector = GetComponent<UnitMoveStateDetector>();
        InitializeParticle();
    }

    private void InitializeParticle()
    {
        if (_dustParticlePrefab == null) return;

        _currentParticle = Instantiate(_dustParticlePrefab, transform);
        _currentParticle.transform.localPosition = _spawnOffset;
        _emission = _currentParticle.emission;
        _emission.enabled = false; // 初始禁用发射
    }

    private void Update()
    {
        if (_currentParticle == null) return;

        bool shouldEmit = _moveDetector.IsMoving
                        && _moveDetector.CurrentSpeed >= _moveSpeedThreshold
                        && _moveDetector.IsGrounded;

        // 控制粒子发射器开关
        _emission.enabled = shouldEmit;

        // 处理粒子系统播放状态
        if (shouldEmit)
        {
            if (!_currentParticle.isPlaying)
            {
                _currentParticle.Play();
            }
        }
        else
        {
            if (_currentParticle.isPlaying)
            {
                _currentParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}