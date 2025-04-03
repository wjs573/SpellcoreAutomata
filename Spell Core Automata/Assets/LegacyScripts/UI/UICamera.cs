using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    public Camera uiCamera;

    private void Start()
    {
        uiCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
    }
}
