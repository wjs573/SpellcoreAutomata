using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static bool SectorHitCircle(Vector2 sectorCenter, float innerRadius, float outerRadius, float sectorAngle, Vector2 circlePivot, float circleRadius)
    {
        // 首先检测内圆与圆形的碰撞
        float distanceToCenter = Vector2.Distance(sectorCenter, circlePivot);
        if (distanceToCenter < innerRadius + circleRadius)
        {
            return true; // 内圆和圆形相交
        }

        // 然后检测外扇形与圆形的碰撞
        float angleToCircle = Vector2.Angle(Vector2.up, circlePivot - sectorCenter);
        if (angleToCircle < sectorAngle / 2 && distanceToCenter <= outerRadius + circleRadius)
        {
            return true; // 外扇形和圆形相交
        }

        return false; // 未相交
    }


    public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, List<Rect> rects)
    {
        if (rects.Count <= 0) return false;
        for (var i = 0; i < rects.Count; i++)
        {
            if (Utils.CircleHitRect(circlePivot, circleRadius, rects[i]) == true)
            {
                return true;
            }
        }
        return false;
    }
    public static bool CircleHitRects(Vector2 circlePivot, float circleRadius, Rect[] rects)
    {
        List<Rect> rl = new List<Rect>();
        for (var i = 0; i < rects.Length; i++)
        {
            rl.Add(rects[i]);
        }
        return CircleHitRects(circlePivot, circleRadius, rl);
    }

    public static bool CircleHitRect(Vector2 circlePivot, float circleRadius, Rect rect)
    {
        int xp = circlePivot.x < rect.x ? 0 : (circlePivot.x > rect.x + rect.width ? 2 : 1);
        int yp = circlePivot.y < rect.y ? 0 : (circlePivot.y > rect.y + rect.height ? 2 : 1);

        if (yp == 1 && xp == 1) return true;  //在中间，则一定命中

        if (yp != 1 && xp == 1)
        {
            float halfRect = rect.height / 2;
            float toHeart = Mathf.Abs(circlePivot.y - (rect.y + halfRect));
            return (toHeart <= circleRadius + halfRect);
        }
        else
        if (yp == 1 && xp != 1)
        {
            float halfRect = rect.width / 2;
            float toHeart = Mathf.Abs(circlePivot.x - (rect.x + halfRect));
            return (toHeart <= circleRadius + halfRect);
        }
        else
        {
            return InRange(
                circlePivot.x, circlePivot.y,
                yp == 0 ? rect.x : (rect.x + rect.width),
                xp == 0 ? rect.y : (rect.y + rect.height),
                circleRadius
            );
        }
    }

    ///<summary>
    ///AABB的矩形之间是否有碰撞
    ///<param name="a">一个rect</param>
    ///<param name="b">另一个rect</param>
    ///<return>是否碰撞到了，true代表碰到了</return>
    ///</summary>
    public static bool RectCollide(Rect a, Rect b)
    {
        float ar = a.x + a.width;
        float br = b.x + b.width;
        float ab = a.y + a.height;
        float bb = b.y + b.height;
        return (
            (a.x >= b.x && a.x <= br) ||
            (b.x >= a.x && b.x <= ar)
        ) && (
            (a.y >= b.y && a.y <= bb) ||
            (b.y >= a.y && b.y <= ab)
        );
    }

    /// <summary>
    /// 基础版本
    /// 圆形Aoe范围检测
    /// </summary>
    /// <param name="x1">aoe的x坐标</param>
    /// <param name="y1">aoe的z坐标</param>
    /// <param name="x2">物体的x坐标</param>
    /// <param name="y2">物体的z左边</param>
    /// <param name="range">aoe的半径</param>
    /// <returns></returns>
    public static bool InRange(float x1, float y1, float x2, float y2, float range)
    {
        return Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) <= Mathf.Pow(range, 2);
    }

    public static bool InRange(AoeState aoeState, float x1, float y1)
    {
        // 判断 AoE 类型
        if (aoeState.type == AoEType.Sector)
        {
            // 计算点到扇形 AoE 中心的向量
            Vector3 toPoint = new Vector3(x1, aoeState.transform.position.y, y1) - aoeState.transform.position;

            // 计算点到扇形 AoE 中心的距离
            float distanceToCenter = toPoint.magnitude;

            // 如果距离超过外半径，则不在扇形 AoE 内
            if (distanceToCenter > aoeState.radius)
            {
                return false;
            }

            // 计算点到扇形 AoE 中心的角度（相对于正前方）
            float angleToCenter = Vector3.SignedAngle(aoeState.transform.forward, toPoint, Vector3.up);

            // 计算点在扇形 AoE 内的角度范围
            float halfRotationAngle = aoeState.rotationAngle / 2;

            // 如果角度在扇形 AoE 的范围内并且距离在内半径和外半径之间，就在扇形 AoE 内
            if (angleToCenter >= aoeState.startAngle - halfRotationAngle && angleToCenter <= aoeState.startAngle + halfRotationAngle && distanceToCenter >= aoeState.radius)
            {
                return true;
            }

            return false;
        }

        if (aoeState.type == AoEType.Rectangle)
        {
            // 计算世界坐标
            Vector3 worldPosition = new Vector3(x1, 0f, y1); // 世界坐标
            // 计算本地坐标
            Vector3 localPosition = aoeState.transform.InverseTransformPoint(worldPosition);
            // 如果坐标在矩形内，就在矩形内
            if (localPosition.x <= aoeState.xLength / 2 && localPosition.x >= aoeState.xLength / -2 && localPosition.z <= aoeState.zLength / 2 && localPosition.z >= aoeState.zLength / -2)
            {
                return true;
            }
        }

        if (aoeState.type == AoEType.Circle)
        {
            // 计算点到圆心的向量
            return InRange(aoeState.transform.position.x, aoeState.transform.position.z, x1, y1, aoeState.radius);
        }
        return false;
    }


    ///<summary>
    ///根据面向和移动方向得到一个资源名预订了规则的后缀名
    ///<param name="faceDegree">面向角度</param>
    ///<param name="moveDegree">移动角度</param>
    ///<return>约定好的关键字，比如"Forward","Back","Left","Right"，对应到角色动画的key</return>
    ///</summary>
    public static string GetTailStringByDegree(float faceDegree, float moveDegree)
    {
        float fd = faceDegree;
        float md = moveDegree;
        while (fd < 180) fd += 360;
        while (md < 180) md += 360;
        fd = fd % 360;
        md = md % 360;
        float dd = md - fd;
        if (dd > 180)
        {
            dd -= 360;
        }
        else if (dd < -180)
        {
            dd += 360;
        }
        //Debug.Log("degree:"+fd + " / " + md + " / " + dd);
        if (dd >= -45 && dd <= 45)
        {
            return "Forward";
        }
        else
        if (dd < -45 && dd >= -135)
        {
            return "Left";
        }
        else
        if (dd > 45 && dd <= 135)
        {
            return "Right";
        }
        else
        {
            return "Back";
        }
    }

}