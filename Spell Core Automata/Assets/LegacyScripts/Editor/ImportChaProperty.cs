using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ImportChaProperty : MonoBehaviour
{
    // ����һ�����ڱ���ChaPropertyʵ�����ֵ�
    private static Dictionary<string, ChaProperty> chaProperties = new Dictionary<string, ChaProperty>();

    private static void ImportChaPropertyFromCsv()
    {
        chaProperties = new Dictionary<string, ChaProperty>();

        //��ȡcsv�ļ�
        FileStream fs = new FileStream("Assets/Resources/Csv/ChaProperty.csv", FileMode.Open);
        StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("gb2312"));
        string tempText = "";
        while ((tempText = sr.ReadLine()) != null)
        {
            //����ǵ�һ�� ֱ������
            if (tempText == "id,name,moveSpeed,cd_speed,actionSpeed,hp,hp_recover,mp," +
                "mp_recover,shield,attack,defence,mind,mental_talent,wood_talent,water_talent," +
                "fire_talent,earth_talent,critic_multiplier,critic_rate,dodge_rate,bodyRadius," +
                "hitRadius,moveType")
            {
                continue;
            }

            string[] arr = tempText.Split(new char[] { ',' });

            if (arr.Length != 24)
            {
                Debug.Log($"����chaproperty�����У�{arr[0]}�����ݴ�������");
                continue;
            }

            ChaProperty chaProperty = GenerateChaPropertyFromString(arr);

            chaProperties.Add(arr[0], chaProperty);
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    /// <summary>
    /// ����csv�е�һ�ж�Ӧ���ַ������飬����һ��chaproperty
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    private static ChaProperty GenerateChaPropertyFromString(string[] arr)
    {
        string id = arr[0];
        string name = arr[1];
        int moveSpeed = int.Parse(arr[2]);
        int cd_speed = int.Parse(arr[3]);
        int actionSpeed = int.Parse(arr[4]);
        int hp = int.Parse(arr[5]);
        int hp_recover = int.Parse(arr[6]);
        int mp = int.Parse(arr[7]);
        int mp_recover = int.Parse(arr[8]);
        int shield = int.Parse(arr[9]);
        int attack = int.Parse(arr[10]);
        int defence = int.Parse(arr[11]);
        int mind = int.Parse(arr[12]);
        float critic_multiplier = float.Parse(arr[18]);
        float critic_rate = float.Parse(arr[19]);
        float dodge_rate = float.Parse(arr[20]);
        float bodyRadius = float.Parse(arr[21]);
        float hitRadius = float.Parse(arr[22]);
        MoveType moveType = (MoveType)Enum.Parse(typeof(MoveType), arr[23]);

        ChaProperty chaProperty = new ChaProperty(
            moveSpeed, cd_speed, actionSpeed,
            hp, hp_recover, mp, mp_recover, shield,
            attack, defence, mind,
            critic_multiplier, critic_rate, dodge_rate, bodyRadius, hitRadius, moveType);
        return chaProperty;
    }
}