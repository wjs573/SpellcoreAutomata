using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;

public class ItemPickup : MonoBehaviour
{
    public Item Item;

    public Material WhiteMaterial;
    public Material BlueMaterial;
    public Material PurpleMaterial;
    public Material GoldenMaterial;
    public Material RedMaterial;

    public MeshRenderer MeshRenderer;
    public float attractionRadius = 3.0f;
    public float attractionSpeed = 100.0f;
    private bool isAttracted = false;

    private void Awake()
    {

        MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    public void Init(Item item)
    {
        this.Item = item;
        switch (item.Rank)
        {
            case ItemRank.默认:
                MeshRenderer.material = WhiteMaterial;
                break;

            case ItemRank.凡品:
                MeshRenderer.material = BlueMaterial;
                break;

            case ItemRank.稀有:
                MeshRenderer.material = PurpleMaterial;
                break;

            case ItemRank.绝世:
                MeshRenderer.material = GoldenMaterial;
                break;

            case ItemRank.罕见:
                MeshRenderer.material = RedMaterial;
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (isAttracted)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractionRadius);
        foreach (Collider collider in hitColliders)
        {
            UnitItemCollector collector = collider.GetComponent<UnitItemCollector>();
            if (collector != null)
            {
                StartCoroutine(AttractToCollector(collector));
                break;
            }
        }
    }

    private IEnumerator AttractToCollector(UnitItemCollector collector)
    {
        isAttracted = true;

        while (Vector3.Distance(transform.position, collector.transform.position) > 0.5f)
        {
            Vector3 direction = (collector.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position = transform.position + direction * attractionSpeed * Time.fixedDeltaTime;
            yield return null;
        }

        collector.AddItem(this);
        gameObject.SetActive(false);
    }
}