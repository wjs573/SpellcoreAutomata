
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WJS
{
    /// <summary>
    /// UI过渡类型
    /// </summary>
    public enum UITransitionType
    {
        /// <summary>
        /// 淡入淡出
        /// </summary>
        Fade,

        /// <summary>
        /// 缩放
        /// </summary>
        Scale,

        /// <summary>
        /// 滑动
        /// </summary>
        Slide,

        /// <summary>
        /// 旋转
        /// </summary>
        Rotate,

        /// <summary>
        /// 弹跳
        /// </summary>
        Bounce,

        /// <summary>
        /// 组合
        /// </summary>
        Combined
    }

    /// <summary>
    /// UI过渡方向
    /// </summary>
    public enum UITransitionDirection
    {
        /// <summary>
        /// 上
        /// </summary>
        Up,

        /// <summary>
        /// 下
        /// </summary>
        Down,

        /// <summary>
        /// 左
        /// </summary>
        Left,

        /// <summary>
        /// 右
        /// </summary>
        Right,

        /// <summary>
        /// 中心
        /// </summary>
        Center
    }

    /// <summary>
    /// UI过渡效果系统
    /// 用于处理UI元素之间的过渡效果
    /// </summary>
    public class UITransition : MonoBehaviour
    {
        [Header("过渡设置")]
        [SerializeField]
        private UITransitionType transitionType = UITransitionType.Fade;

        [SerializeField]
        private UITransitionDirection transitionDirection = UITransitionDirection.Center;

        [SerializeField]
        private float duration = 0.5f;

        [SerializeField]
        private float delay = 0f;

        [SerializeField]
        private bool useUnscaledTime = false;

        [SerializeField]
        private AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        private bool useCustomCurve = false;

        private Coroutine transitionCoroutine;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private bool isInitialized = false;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            // 获取组件
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            rectTransform = GetComponent<RectTransform>();

            // 保存原始状态
            originalPosition = rectTransform.anchoredPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;

            isInitialized = true;
        }

        /// <summary>
        /// 播放进入过渡
        /// </summary>
        public void PlayEnterTransition(Action onComplete = null)
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(PlayTransitionCoroutine(true, onComplete));
        }

        /// <summary>
        /// 播放退出过渡
        /// </summary>
        public void PlayExitTransition(Action onComplete = null)
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = StartCoroutine(PlayTransitionCoroutine(false, onComplete));
        }

        /// <summary>
        /// 播放过渡协程
        /// </summary>
        private IEnumerator PlayTransitionCoroutine(bool isEnter, Action onComplete)
        {
            Initialize();

            // 等待延迟
            if (delay > 0)
            {
                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }
            // 设置初始状态
            Vector3 startPosition = rectTransform.anchoredPosition;
            Quaternion startRotation = rectTransform.localRotation;
            Vector3 startScale = rectTransform.localScale;
            float startAlpha = canvasGroup.alpha;

            // 计算结束状态
            Vector3 endPosition = originalPosition;
            Quaternion endRotation = originalRotation;
            Vector3 endScale = originalScale;
            float endAlpha = isEnter ? 1f : 0f;

            // 根据过渡类型和方向设置初始状态
            if (isEnter)
            {
                SetInitialStateForEnter();
            }
            else
            {
                SetInitialStateForExit();
            }

            // 播放过渡
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float progress = elapsedTime / duration;
                // 应用缓动曲线
                float easedProgress = useCustomCurve ? customCurve.Evaluate(progress) : EaseInOutQuad(progress);
                // 应用过渡效果
                ApplyTransitionEffect(easedProgress, isEnter);

                yield return null;
            }

            // 确保过渡完成
            ApplyTransitionEffect(1f, isEnter);

            // 调用完成回调
            onComplete?.Invoke();
            transitionCoroutine = null;
        }

        /// <summary>
        /// 设置进入过渡的初始状态
        /// </summary>
        private void SetInitialStateForEnter()
        {
            switch (transitionType)
            {
                case UITransitionType.Fade:
                    canvasGroup.alpha = 0f;
                    break;

                case UITransitionType.Scale:
                    rectTransform.localScale = Vector3.zero;
                    break;

                case UITransitionType.Slide:
                    SetSlidePosition(true);
                    break;

                case UITransitionType.Rotate:
                    rectTransform.localRotation = Quaternion.Euler(0, 0, 180f);
                    break;

                case UITransitionType.Bounce:
                    canvasGroup.alpha = 0f; // 加上这一行，确保起点一致
                    rectTransform.localScale = Vector3.zero;
                    break;

                case UITransitionType.Combined:
                    canvasGroup.alpha = 0f;
                    rectTransform.localScale = Vector3.zero;
                    SetSlidePosition(true);
                    break;
            }
        }

        /// <summary>
        /// 设置退出过渡的初始状态
        /// </summary>
        private void SetInitialStateForExit()
        {
            // 恢复原始状态
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// 设置UI元素的滑动位置，用于界面过渡动画
        /// </summary>
        /// <param name="isEnter">布尔值，表示是进入(true)还是退出(false)动画</param>
        private void SetSlidePosition(bool isEnter)
        {
            // 计算偏移量，为屏幕宽度的一半
            float offset = Screen.width * 0.5f;

            // 根据过渡方向设置UI元素的锚点位置
            switch (transitionDirection)
            {
                // 向上过渡：进入时从下方进入，退出时回到原位
                case UITransitionDirection.Up:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x, isEnter ? -offset : originalPosition.y);
                    break;

                case UITransitionDirection.Down:
                    rectTransform.anchoredPosition = new Vector2(originalPosition.x, isEnter ? offset : originalPosition.y);
                    break;

                case UITransitionDirection.Left:
                    rectTransform.anchoredPosition = new Vector2(isEnter ? offset : originalPosition.x, originalPosition.y);
                    break;

                case UITransitionDirection.Right:
                    rectTransform.anchoredPosition = new Vector2(isEnter ? -offset : originalPosition.x, originalPosition.y);
                    break;

                case UITransitionDirection.Center:
                    rectTransform.anchoredPosition = originalPosition;
                    break;
            }
        }

        /// <summary>
        /// 应用过渡效果
        /// </summary>
        /// <param name="progress">进度</param>
        /// <param name="isEnter">是否为进入过渡</param>
        private void ApplyTransitionEffect(float progress, bool isEnter)
        {
            switch (transitionType)
            {
                case UITransitionType.Fade:
                    canvasGroup.alpha = isEnter ? progress : 1f - progress;
                    break;

                case UITransitionType.Scale:
                    rectTransform.localScale = Vector3.Lerp(
                        isEnter ? Vector3.zero : originalScale,
                        isEnter ? originalScale : Vector3.zero,
                        progress
                    );
                    break;

                case UITransitionType.Slide:
                    ApplySlideEffect(progress, isEnter);
                    break;

                case UITransitionType.Rotate:
                    float startAngle = isEnter ? 180f : 0f;
                    float endAngle = isEnter ? 0f : -180f;
                    rectTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startAngle, endAngle, progress));
                    break;

                case UITransitionType.Bounce:
                    float easedProgress = isEnter ? EaseOutBack(progress) : EaseInBack(progress);
                    canvasGroup.alpha = isEnter ? easedProgress : 1f - easedProgress;
                    rectTransform.localScale = Vector3.Lerp(
                        isEnter ? Vector3.zero : originalScale,
                        isEnter ? originalScale : Vector3.zero,
                        easedProgress
                    );
                    break;

                case UITransitionType.Combined:
                    canvasGroup.alpha = isEnter ? progress : 1f - progress;
                    rectTransform.localScale = Vector3.Lerp(
                        isEnter ? Vector3.zero : originalScale,
                        isEnter ? originalScale : Vector3.zero,
                        progress
                    );
                    ApplySlideEffect(progress, isEnter);
                    break;
            }
        }

        /// <summary>
        /// 应用滑动效果
        /// </summary>
        /// <param name="progress">进度</param>
        /// <param name="isEnter">是否为进入过渡</param>
        private void ApplySlideEffect(float progress, bool isEnter)
        {
            float offset = Screen.width * 0.5f;
            Vector2 startPosition = originalPosition;
            Vector2 endPosition = originalPosition;

            switch (transitionDirection)
            {
                case UITransitionDirection.Up:
                    startPosition = new Vector2(originalPosition.x, isEnter ? -offset : originalPosition.y);
                    endPosition = new Vector2(originalPosition.x, isEnter ? originalPosition.y : offset);
                    break;

                case UITransitionDirection.Down:
                    startPosition = new Vector2(originalPosition.x, isEnter ? offset : originalPosition.y);
                    endPosition = new Vector2(originalPosition.x, isEnter ? originalPosition.y : -offset);
                    break;

                case UITransitionDirection.Left:
                    startPosition = new Vector2(isEnter ? offset : originalPosition.x, originalPosition.y);
                    endPosition = new Vector2(isEnter ? originalPosition.x : -offset, originalPosition.y);
                    break;

                case UITransitionDirection.Right:
                    startPosition = new Vector2(isEnter ? -offset : originalPosition.x, originalPosition.y);
                    endPosition = new Vector2(isEnter ? originalPosition.x : offset, originalPosition.y);
                    break;

                case UITransitionDirection.Center:
                    startPosition = originalPosition;
                    endPosition = originalPosition;
                    break;
            }

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);
        }

        /// <summary>
        /// EaseInOutQuad缓动函数
        /// </summary>
        private float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        /// <summary>
        /// EaseOutBack缓动函数
        /// </summary>
        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }

        /// <summary>
        /// EaseInBack缓动函数
        /// </summary>
        private float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return c3 * t * t * t - c1 * t * t;
        }

        /// <summary>
        /// 停止过渡
        /// </summary>
        public void StopTransition()
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
                transitionCoroutine = null;
            }
        }

        /// <summary>
        /// 重置到原始状态
        /// </summary>
        public void ResetToOriginalState()
        {
            Initialize();

            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
            canvasGroup.alpha = 1f;
        }
    }
}
