using UnityEngine;

public class JumpingY : MonoBehaviour
{
    float elapsedTime;
    float totalJumpTime;
    float initialJumpHeight;
    float jumpHeight;
    bool isJumping = false;

    Transform viewContainer;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        viewContainer = transform.GetChild(0).transform;
    }

    public void JumpStart(float t, float height)
    {
        if (isJumping)
        {
            // 在跳跃过程中再次触发跳跃
            totalJumpTime += t;
            jumpHeight += height;
        }
        else
        {
            // 开始新的跳跃
            totalJumpTime = t;
            jumpHeight = height;
            elapsedTime = 0f;
            isJumping = true;
            initialJumpHeight = viewContainer.position.y;
        }
    }

    private void FixedUpdate()
    {
        if (!isJumping) return;

        elapsedTime += Time.deltaTime;

        // 计算当前高度
        Vector3 position = viewContainer.position;
        position.y = initialJumpHeight + GetYPos(elapsedTime);
        viewContainer.position = position;

        // 判断跳跃是否结束
        if (elapsedTime >= totalJumpTime)
        {
            isJumping = false;
            OnJumpEnd();
        }
    }

    float GetYPos(float time)
    {
        if (time <= totalJumpTime)
        {
            float jumpProgress = time / totalJumpTime;
            return -4 * jumpHeight * jumpProgress * (jumpProgress - 1);
        }
        return 0;
    }

    void OnJumpEnd()
    {
        // 跳跃结束时的逻辑，可以在这里重置状态或触发其他事件
        ResetJump();
    }

    public void ResetJump()
    {
        // 重置跳跃状态
        elapsedTime = 0;
        isJumping = false;
        initialJumpHeight = viewContainer.position.y;
    }
}
