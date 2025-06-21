using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WJS
{
    public static class CommonScripts
    {
        /// <summary>
        /// 伤害公式
        /// </summary>
        /// <param name="damageInfo"></param>
        /// <param name="asHeal"></param>
        /// <returns></returns>
        public static DamageInfo DamageValue(DamageInfo damageInfo, bool asHeal)
        {
            return damageInfo;
        }

        /// <summary>
        /// 修改参数
        /// </summary>
        /// <param name="eveParams"></param>
        /// <param name="modifier"></param>
        /// <typeparam name="T"></typeparam>
        public static void ModifyParameterOfType<T>(object[] eveParams, Func<T, T> modifier)
        {
            // 动态生成索引，只对当前的 eveParams 有效
            int[] indices = eveParams
                .Select((param, index) => (param, index))
                .Where(x => x.param is T)
                .Select(x => x.index)
                .ToArray();

            foreach (int index in indices)
            {
                if (eveParams[index] is T originalValue)
                {
                    eveParams[index] = modifier(originalValue);
                }
            }
        }
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
                if (CommonScripts.CircleHitRect(circlePivot, circleRadius, rects[i]) == true)
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
}

