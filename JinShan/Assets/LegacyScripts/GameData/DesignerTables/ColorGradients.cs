using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorGradients
{
    public static Dictionary<string, Gradient> data = new Dictionary<string, Gradient>()
    {
        {"Green",CreateGreenColorGradient() },
        {"Red",CreateRedColorGradient() },
        {"Yellow",CreateYellowColorGradient() }
    };

    private static Gradient CreateGreenColorGradient()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        // 第一个颜色关键帧
        colorKeys[0].color = Color.green;
        colorKeys[0].time = 0f;

        // 第二个颜色关键帧
        colorKeys[1].color = Color.green;
        colorKeys[1].time = 1f;

        // 透明度关键帧
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 1f;

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }

    // 创建红色渐变色
    private static Gradient CreateRedColorGradient()
    {
        Gradient gradient = new Gradient();

        // 创建渐变色关键点
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.red, 0f);
        colorKeys[1] = new GradientColorKey(Color.red, 1f);

        // 创建透明度关键点
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        // 设置渐变的颜色和透明度关键点
        gradient.colorKeys = colorKeys;
        gradient.alphaKeys = alphaKeys;

        return gradient;
    }

    // 创建黄色渐变色
    private static Gradient CreateYellowColorGradient()
    {
        Gradient gradient = new Gradient();

        // 创建渐变色关键点
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.yellow, 0f);
        colorKeys[1] = new GradientColorKey(Color.yellow, 1f);

        // 创建透明度关键点
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        // 设置渐变的颜色和透明度关键点
        gradient.colorKeys = colorKeys;
        gradient.alphaKeys = alphaKeys;

        return gradient;
    }
}
