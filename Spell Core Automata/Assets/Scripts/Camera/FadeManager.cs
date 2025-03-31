using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FadeManager : MonoBehaviour
{
    [Header("核心参数")]
    public Transform cameraTransform;
    public Transform characterTransform;
    public LayerMask occlusionLayers;
    [Range(0.1f, 2f)] public float detectionWidth = 0.5f;
    [Range(0.1f, 1f)] public float fadeDuration = 0.3f;

    [Header("调试选项")]
    public bool showDebugRays = true;
    public Color debugColor = new Color(1, 0.8f, 0, 0.25f);

    // 状态管理
    private Dictionary<int, FadeController> _activeOccluders = new Dictionary<int, FadeController>();
    private HashSet<int> _currentFrameIDs = new HashSet<int>();

    void Update()
    {
        if (Time.frameCount % 3 == 0) // 降低检测频率
        {
            DetectOcclusion();
            ProcessFadeStates();
        }
    }

    void DetectOcclusion()
    {
        _currentFrameIDs.Clear();

        // 安全校验
        if (cameraTransform == null || characterTransform == null) return;

        Vector3 camPos = cameraTransform.position;
        Vector3 charPos = characterTransform.position + Vector3.up * 0.5f;
        Vector3 dir = (charPos - camPos).normalized;
        float dist = Vector3.Distance(camPos, charPos);

        // 优化检测方式：使用OverlapBox + 方向过滤
        Collider[] colliders = Physics.OverlapBox(
            center: camPos + dir * dist * 0.5f,
            halfExtents: new Vector3(detectionWidth * 0.5f, detectionWidth * 0.5f, dist * 0.5f),
            orientation: Quaternion.LookRotation(dir),
            layerMask: occlusionLayers
        );

        foreach (Collider col in colliders)
        {
            if (IsValidOccluder(col.gameObject))
            {
                _currentFrameIDs.Add(col.gameObject.GetInstanceID());
                if (showDebugRays) DrawDebugCube(col.bounds);
            }
        }
    }

    void ProcessFadeStates()
    {
        // 处理新增遮挡物
        foreach (int id in _currentFrameIDs)
        {
            if (!_activeOccluders.TryGetValue(id, out FadeController controller))
            {
                GameObject obj = GetGameObjectByID(id);
                if (obj == null) continue;

                controller = obj.GetComponent<FadeController>() ?? obj.AddComponent<FadeController>();
                controller.Initialize();
                _activeOccluders[id] = controller;
            }
            
            if (controller.CurrentAlpha > 0.5f)
            {
                controller.FadeOut(fadeDuration);
            }
        }

        // 处理消失的遮挡物
        List<int> toRemove = new List<int>();
        foreach (var pair in _activeOccluders)
        {
            if (!_currentFrameIDs.Contains(pair.Key))
            {
                pair.Value.FadeIn(fadeDuration);
                toRemove.Add(pair.Key);
            }
        }

        // 延迟移除防止闪烁
        foreach (int key in toRemove)
        {
            _activeOccluders.Remove(key);
        }
    }

    bool IsValidOccluder(GameObject obj)
    {
        // 排除无效对象
        if (obj == null) return false;
        if (obj.transform.IsChildOf(characterTransform)) return false;
        return obj.GetComponent<Renderer>() != null;
    }

    GameObject GetGameObjectByID(int instanceID)
    {
        // 优化查找方式
        return (from obj in FindObjectsOfType<GameObject>()
                where obj.GetInstanceID() == instanceID
                select obj).FirstOrDefault();
    }

    void DrawDebugCube(Bounds bounds)
    {
        Debug.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), debugColor);
        Debug.DrawLine(bounds.min, new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), debugColor);
        Debug.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), debugColor);
        Debug.DrawLine(bounds.max, new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), debugColor);
    }
}