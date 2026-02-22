using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// UI射线检测调试器
/// 用于实时显示当前鼠标位置射线检测到的UI元素
/// 挂载到任意激活的GameObject上即可使用
/// </summary>
public class UIRaycastDebugger : MonoBehaviour
{
    [Header("显示设置")]
    [Tooltip("是否在控制台打印日志")]
    public bool logToConsole = true;
    
    [Tooltip("是否在Scene视图中绘制调试信息")]
    public bool showInSceneView = true;
    
    [Tooltip("显示检测结果在Game视图")]
    public bool showOnScreenGUI = true;
    
    [Header("过滤设置")]
    [Tooltip("只显示包含这些关键词的物体（留空显示全部）")]
    public List<string> filterKeywords = new List<string>();
    
    [Tooltip("忽略这些物体")]
    public List<GameObject> ignoreObjects = new List<GameObject>();
    
    // 当前检测到的结果
    private List<RaycastResult> currentResults = new List<RaycastResult>();
    private GameObject topObject;
    private PointerEventData pointerData;
    private EventSystem eventSystem;
    
    // GUI 显示用
    private string displayText = "";
    private Vector2 screenPos;

    void Start()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("[UIRaycastDebugger] 未找到 EventSystem！");
            enabled = false;
            return;
        }
        
        pointerData = new PointerEventData(eventSystem);
        Debug.Log("[UIRaycastDebugger] 调试器已启动，移动鼠标查看射线检测结果");
    }

    void Update()
    {
        if (eventSystem == null) return;
        
        // 获取鼠标位置
        screenPos = Input.mousePosition;
        
        // 设置指针数据
        pointerData.position = screenPos;
        
        // 清空并执行射线检测
        currentResults.Clear();
        eventSystem.RaycastAll(pointerData, currentResults);
        
        // 找到第一个有效的（未被忽略的）物体
        topObject = null;
        foreach (var result in currentResults)
        {
            if (result.gameObject == null) continue;
            if (ignoreObjects.Contains(result.gameObject)) continue;
            
            // 如果有过滤关键词，检查是否匹配
            if (filterKeywords.Count > 0)
            {
                bool match = false;
                foreach (var keyword in filterKeywords)
                {
                    if (result.gameObject.name.Contains(keyword))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match) continue;
            }
            
            topObject = result.gameObject;
            break;
        }
        
        // 更新显示文本
        UpdateDisplayText();
        
        // 控制台日志（只在物体变化时打印，避免刷屏）
        if (logToConsole && topObject != null && topObject != lastLoggedObject)
        {
            Debug.Log($"[UIRaycastDebugger] 当前最上层UI: {GetObjectPath(topObject)}", topObject);
            lastLoggedObject = topObject;
        }
    }
    
    private GameObject lastLoggedObject;
    
    void UpdateDisplayText()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== UI射线检测结果 ===");
        sb.AppendLine($"鼠标位置: {screenPos}");
        sb.AppendLine($"检测到的物体数: {currentResults.Count}");
        sb.AppendLine("");
        
        if (topObject != null)
        {
            sb.AppendLine($"【最上层】{topObject.name}");
            sb.AppendLine($"  路径: {GetObjectPath(topObject)}");
            sb.AppendLine($"  Layer: {LayerMask.LayerToName(topObject.layer)}");
            
            // 检查是否有 Image 组件
            var image = topObject.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                sb.AppendLine($"  Image.raycastTarget: {image.raycastTarget}");
                sb.AppendLine($"  Image.color.alpha: {image.color.a:F2}");
            }
            
            // 检查是否有其他接收事件的组件
            var hasPointerEnter = topObject.GetComponent<IPointerEnterHandler>() != null;
            var hasPointerClick = topObject.GetComponent<IPointerClickHandler>() != null;
            if (hasPointerEnter) sb.AppendLine($"  ✓ 实现了 IPointerEnterHandler");
            if (hasPointerClick) sb.AppendLine($"  ✓ 实现了 IPointerClickHandler");
        }
        else
        {
            sb.AppendLine("【未检测到任何UI】");
        }
        
        // 列出前5个检测到的物体
        if (currentResults.Count > 0)
        {
            sb.AppendLine("");
            sb.AppendLine("--- 所有检测结果（前5个）---");
            int count = Mathf.Min(5, currentResults.Count);
            for (int i = 0; i < count; i++)
            {
                var result = currentResults[i];
                string marker = (result.gameObject == topObject) ? ">>> " : "    ";
                sb.AppendLine($"{marker}[{i}] {result.gameObject.name} (depth:{result.depth}, sortingOrder:{result.sortingOrder})");
            }
        }
        
        displayText = sb.ToString();
    }
    
    string GetObjectPath(GameObject obj)
    {
        if (obj == null) return "null";
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    // 在Game视图中显示GUI
    void OnGUI()
    {
        if (!showOnScreenGUI) return;
        
        // 背景框
        float boxWidth = 450;
        float boxHeight = 300;
        float x = Screen.width - boxWidth - 10;
        float y = 10;
        
        GUI.Box(new Rect(x - 5, y - 5, boxWidth + 10, boxHeight + 10), "");
        GUI.Label(new Rect(x, y, boxWidth, boxHeight), displayText, new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            normal = { textColor = Color.white }
        });
        
        // 在鼠标位置画一个标记
        if (topObject != null)
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(screenPos.x + 15, Screen.height - screenPos.y - 15, 200, 20), $"→ {topObject.name}");
            GUI.color = Color.white;
        }
    }

    // 在Scene视图中绘制
    void OnDrawGizmos()
    {
        if (!showInSceneView || !Application.isPlaying) return;
        
        // 在鼠标位置绘制十字
        Gizmos.color = Color.red;
        Vector3 worldPos = Camera.main ? Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10)) : Vector3.zero;
        Gizmos.DrawLine(worldPos - Vector3.right * 0.5f, worldPos + Vector3.right * 0.5f);
        Gizmos.DrawLine(worldPos - Vector3.up * 0.5f, worldPos + Vector3.up * 0.5f);
    }
}
