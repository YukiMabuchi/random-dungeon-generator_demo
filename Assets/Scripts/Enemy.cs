using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector2 curPos;
    LayerMask obstacleMask;
    List<Vector2> availableMovementList = new List<Vector2>();

    bool isMoving;

    void Start()
    {
        obstacleMask = LayerMask.GetMask("Wall", "Enemy", "Player");
        curPos = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        availableMovementList.Clear();

        Vector2 hitSize = Vector2.one * .8f;

        // 進行可能マスの生成
        Collider2D hitUp = Physics2D.OverlapBox(curPos + Vector2.up, hitSize, 0, obstacleMask);
        if (!hitUp)
        {
            availableMovementList.Add(Vector2.up);
        }

        Collider2D hitRight = Physics2D.OverlapBox(curPos + Vector2.right, hitSize, 0, obstacleMask);
        if (!hitRight)
        {
            availableMovementList.Add(Vector2.right);
        }

        Collider2D hitDown = Physics2D.OverlapBox(curPos + Vector2.down, hitSize, 0, obstacleMask);
        if (!hitDown)
        {
            availableMovementList.Add(Vector2.down);
        }

        Collider2D hitLeft = Physics2D.OverlapBox(curPos + Vector2.left, hitSize, 0, obstacleMask);
        if (!hitLeft)
        {
            availableMovementList.Add(Vector2.left);
        }

        // 進行方向を進行可能なマスからランダムに1マス選ぶ
        if (availableMovementList.Count > 0)
        {
            int randomIndex = Random.Range(0, availableMovementList.Count);
            curPos += availableMovementList[randomIndex];
        }

        StartCoroutine(SmoothMove());
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, curPos) > .01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, curPos, 5f * Time.deltaTime);
            yield return null; // 1フレーム待つ
        }
        transform.position = curPos;

        yield return new WaitForSeconds(Random.Range(1f, 5f)); // TODO: プレイヤーが動くまで？

        isMoving = false;
    }
}
