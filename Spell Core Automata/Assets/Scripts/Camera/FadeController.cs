using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class FadeController : MonoBehaviour
{
    // 状态属性
    public float CurrentAlpha { get; private set; } = 1f;
    public bool IsFadedOut => CurrentAlpha < 0.5f;

    // 材质管理
    private Material[] _originalMaterials;
    private Material[] _fadeMaterials;
    private Coroutine _activeCoroutine;

    public void FadeOut(float duration)
    {
        if (IsFadedOut) return;
        StartFadeCoroutine(0.3f, duration);
    }

    public void FadeIn(float duration)
    {
        if (!IsFadedOut) return;
        StartFadeCoroutine(1f, duration);
    }

    void StartFadeCoroutine(float targetAlpha, float duration)
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
        }
        _activeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = CurrentAlpha;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            CurrentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            
            foreach (Material mat in _fadeMaterials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = CurrentAlpha;
                    mat.color = color;
                }
            }
            yield return null;
        }

        CurrentAlpha = targetAlpha;
        _activeCoroutine = null;
    }

    void SetupMaterialBlending(Material material)
    {
        // 确保材质支持透明度
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.EnableKeyword("_ALPHABLEND_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void OnDestroy()
    {
        // 安全获取渲染器组件
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && _originalMaterials != null)
        {
            // 过滤空材质引用
            Material[] validMaterials = _originalMaterials
                .Where(m => m != null)
                .ToArray();

            // 安全恢复材质
            if (validMaterials.Length > 0)
            {
                renderer.materials = validMaterials;
            }
        }

        // 安全销毁材质实例
        if (_fadeMaterials != null)
        {
            foreach (Material mat in _fadeMaterials)
            {
                if (mat != null && !mat.Equals(null))
                {
                    // 异步销毁防止编辑器警告
                    if (Application.isPlaying)
                    {
                        Destroy(mat);
                    }
                    else
                    {
                        DestroyImmediate(mat);
                    }
                }
            }
        }
    }

    public void Initialize()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("FadeController 需要 Renderer 组件");
            return;
        }

        // 安全获取原始材质
        _originalMaterials = renderer.sharedMaterials
            .Where(m => m != null)
            .ToArray();

        // 空材质检查
        if (_originalMaterials.Length == 0)
        {
            Debug.LogWarning($"物体 {gameObject.name} 没有有效材质", gameObject);
            return;
        }

        // 初始化材质实例
        _fadeMaterials = new Material[_originalMaterials.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            if (_originalMaterials[i] != null)
            {
                _fadeMaterials[i] = new Material(_originalMaterials[i])
                {
                    name = $"{_originalMaterials[i].name}_FadeInstance"
                };
                SetupMaterialBlending(_fadeMaterials[i]);
            }
        }

        renderer.materials = _fadeMaterials;
    }
}