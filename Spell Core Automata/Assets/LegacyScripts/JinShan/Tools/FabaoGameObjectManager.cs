using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FabaoGameObjectManager : MonoBehaviour
{
    private GameObject owner;
    private float radius;
    private float height;
    [ShowInInspector]
    private List<GameObject> faBaoObjects;
    [ShowInInspector]
    private Dictionary<GameObject, bool> faBaoVisibility;

    public void Init(GameObject owner, float radius = 1f, float height = 1f)
    {
        this.owner = owner;
        this.radius = radius;
        this.height = height;
        faBaoObjects = new List<GameObject>();
        faBaoVisibility = new Dictionary<GameObject, bool>();
    }

    public void FixedUpdate()
    {
        UpdateFaBaoPositions();
    }

    public void EnterFollowState(GameObject faBao)
    {
        if (!faBaoObjects.Contains(faBao))
        {
            faBaoObjects.Add(faBao);
            faBaoVisibility[faBao] = true;
        }
        UpdateFaBaoPositions();
    }

    public void ExitFollowState(GameObject faBao)
    {
        if (faBaoObjects.Contains(faBao))
        {
            faBaoObjects.Remove(faBao);
            faBaoVisibility.Remove(faBao);
        }
        UpdateFaBaoPositions();
    }

    public void HideFaBao(GameObject faBao)
    {
        if (faBaoObjects.Contains(faBao))
        {
            faBaoVisibility[faBao] = false;
            faBao.SetActive(false);
        }
    }

    public void ShowFaBao(GameObject faBao)
    {
        if (faBaoObjects.Contains(faBao))
        {
            faBaoVisibility[faBao] = true;
            faBao.SetActive(true);
        }
    }

    private void UpdateFaBaoPositions()
    {
        float angleIncrement = 360f / faBaoObjects.Count;
        for (int i = 0; i < faBaoObjects.Count; i++)
        {
            if (faBaoVisibility[faBaoObjects[i]])
            {
                float angle = angleIncrement * i;
                Vector3 positionOffset = new Vector3(
                    Mathf.Sin(angle * Mathf.Deg2Rad),
                    height,
                    Mathf.Cos(angle * Mathf.Deg2Rad)
                ) * radius;
                faBaoObjects[i].transform.position = owner.transform.position + positionOffset;
            }
        }
    }
}
