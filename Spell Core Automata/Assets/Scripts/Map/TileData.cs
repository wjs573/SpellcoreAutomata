using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Map/Tile Config")]
public class TileData : ScriptableObject
{
    [Header("Base Settings")]
    public GameObject prefab;
    public Vector2Int baseSize = Vector2Int.one;

    [Header("Interface Config")]
    public List<EdgeInterface> interfaces = new List<EdgeInterface>();

#if UNITY_EDITOR
    // 在 Inspector 中显示顶视图（仅 Editor 模式）
    [ShowInInspector, PreviewField(100), PropertyOrder(-1), HideLabel]
    private Texture2D TopViewTexture
    {
        get
        {
            if (prefab == null) return null;

            // 只在需要时生成（避免频繁渲染）
            if (_topViewTexture == null || _lastPrefab != prefab)
            {
                GenerateTopView();
                _lastPrefab = prefab;
            }
            return _topViewTexture;
        }
    }

    private Texture2D _topViewTexture;
    private GameObject _lastPrefab;

    private void GenerateTopView()
    {
        // 创建临时实例（不污染 Hierarchy）
        GameObject tempInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        tempInstance.hideFlags = HideFlags.HideAndDontSave;

        // 渲染顶视图
        _topViewTexture = RenderTopView(tempInstance);

        // 清理临时对象
        DestroyImmediate(tempInstance);
    }

    private Texture2D RenderTopView(GameObject target)
    {
        Bounds bounds = CalculateVisualBounds(target);

        // 相机安全距离计算（比常规更大）
        float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float safeDistance = maxDimension * 2f; // 2倍安全距离

        // 创建相机
        GameObject cameraObj = new GameObject("TopViewCamera");
        cameraObj.hideFlags = HideFlags.HideAndDontSave;
        Camera camera = cameraObj.AddComponent<Camera>();

        camera.orthographic = true;
        camera.orthographicSize = maxDimension * 0.6f; // 比实际需要大20%
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = safeDistance * 2f;
        camera.transform.position = bounds.center + Vector3.up * safeDistance;
        camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0);

        // 强制渲染所有层
        camera.cullingMask = ~0;

        // 临时禁用可能影响渲染的组件
        MonoBehaviour[] scripts = target.GetComponentsInChildren<MonoBehaviour>();
        Dictionary<MonoBehaviour, bool> originalStates = new Dictionary<MonoBehaviour, bool>();
        foreach (var script in scripts)
        {
            if (script.enabled && script.GetType().GetMethod("OnWillRenderObject") != null)
            {
                originalStates.Add(script, script.enabled);
                script.enabled = false;
            }
        }

        // 渲染
        RenderTexture rt = new RenderTexture(1024, 1024, 32);
        camera.targetTexture = rt;
        camera.Render();

        // 恢复组件状态
        foreach (var kvp in originalStates)
        {
            kvp.Key.enabled = kvp.Value;
        }

        // 生成纹理
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        // 清理
        RenderTexture.active = null;
        DestroyImmediate(cameraObj);
        DestroyImmediate(rt);

        return texture;
    }
#endif

    // 原有方法保持不变
    public void UpdateTileSize()
    {
        for (int i = 0; i < interfaces.Count; i++)
        {
            var edgeInterface = interfaces[i];
            interfaces[i] = edgeInterface;
        }
    }

    private Bounds CalculateVisualBounds(GameObject target)
    {
        // 方法1：强制包含所有子物体的MeshFilter顶点
        Bounds bounds = new Bounds(target.transform.position, Vector3.zero);

        // 包含所有Renderer的包围盒
        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // 方法2：直接分析所有Mesh顶点（更精确但更耗性能）
        MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length > 0)
        {
            bool hasInitialized = false;
            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.sharedMesh == null) continue;

                Vector3[] vertices = mf.sharedMesh.vertices;
                foreach (Vector3 vertex in vertices)
                {
                    Vector3 worldPos = mf.transform.TransformPoint(vertex);
                    if (!hasInitialized)
                    {
                        bounds = new Bounds(worldPos, Vector3.zero);
                        hasInitialized = true;
                    }
                    else
                    {
                        bounds.Encapsulate(worldPos);
                    }
                }
            }
        }

        // 安全阈值：确保最小尺寸
        if (bounds.size.magnitude < 0.1f)
        {
            bounds.size = Vector3.one;
        }

        return bounds;
    }
}