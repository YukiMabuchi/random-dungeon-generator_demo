using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    Transform GFX;
    float flipx;

    void Start()
    {
        GFX = GetComponentInChildren<SpriteRenderer>().transform;
        flipx = GFX.localScale.x;
    }

    void Update()
    {
        // キー入力で方向転換
        float horizontal = Math.Sign(Input.GetAxisRaw("Horizontal"));
        if (Mathf.Abs(horizontal) > 0)
        {
            GFX.localScale = new Vector2(flipx * horizontal, GFX.localScale.y);
        }
    }
}
