using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWidthFollow : MonoBehaviour
{
    public RectTransform ImageToFollow;
    public RectTransform ImageFollowing;
    public float offest;

    // Update is called once per frame
    void Update()
    {
        ImageFollowing.sizeDelta = new Vector2(ImageToFollow.sizeDelta.x + offest, ImageFollowing.sizeDelta.y);
    }
}
