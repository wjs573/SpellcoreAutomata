using UnityEngine;

public static class Toolbox
{
    public static bool RandomResult(float probability)
    {
        float r = Random.Range(0f, 1f);
        if (r < probability)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 将xoz平面上的角度转换为Vector3。y分量为0。
    /// </summary>
    /// <param name="angle">角度，以度为单位</param>
    /// <returns>转换后的Vector3</returns>
    public static Vector3 AngleToVector3(float angle)
    {
        // 将角度转换为弧度
        float angleInRadians = angle * Mathf.Deg2Rad;

        // 计算x和z分量
        float x = Mathf.Cos(angleInRadians);
        float z = Mathf.Sin(angleInRadians);

        // 创建并返回Vector3，y分量为0
        return new Vector3(x, 0, z);
    }
}
