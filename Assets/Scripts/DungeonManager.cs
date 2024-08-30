using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject floorPrefab, wallPrefab, tilePrefab, exitPrefab;
    public GameObject[] randomItems;
    [Range(50, 1000)] public int totalFloorCount;
    [Range(0, 100)] public int itemSpawnPercent;

    [HideInInspector] public float minX, maxX, minY, maxY;

    List<Vector3> floorList = new List<Vector3>();
    Vector3 doorPos;
    LayerMask floorMask;
    LayerMask wallMask;

    void Start()
    {
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
        RandomWalker();
    }
    void Update()
    {
        // テスト用
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Map Generator
    /// </summary>
    void RandomWalker()
    {
        Vector3 curPos = Vector3.zero; // x: 0, y: 0, z: 0

        floorList.Add(curPos);

        // floorListを作成
        bool inFLoorList;
        while (floorList.Count < totalFloorCount)
        {
            switch (Random.Range(1, 5))
            {
                case 1:
                    curPos += Vector3.up;
                    break;
                case 2:
                    curPos += Vector3.right;
                    break;
                case 3:
                    curPos += Vector3.down;
                    break;
                case 4:
                    curPos += Vector3.left;
                    break;
            }

            inFLoorList = false;
            for (int i = 0; i < floorList.Count; i++)
            {
                if (Vector3.Equals(curPos, floorList[i]))
                {
                    inFLoorList = true;
                    break;
                }
            }
            if (!inFLoorList)
            {
                floorList.Add(curPos);
            }
        }

        // floorListを元にタイルを生成
        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity);
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }

        StartCoroutine(DelayProgress());
    }

    IEnumerator DelayProgress()
    {
        // タイルの生成が終了するのを待つ
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }

        // ExitDoorの作成
        ExitDoorway();

        // アイテムの生成
        Vector2 hitSize = Vector2.one * .8f;
        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; y++)
            {
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, floorMask);
                if (hitFloor)
                {
                    if (!Vector2.Equals(hitFloor.transform.position, doorPos))
                    {
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);

                        RandomItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                    }
                }
            }
        }

    }

    void RandomItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        if ((hitTop || hitRight || hitBottom || hitLeft) && !(hitTop && hitBottom) && !(hitRight && hitLeft))
        {
            int roll = Random.Range(0, 101);
            if (roll <= itemSpawnPercent)
            {
                int itemIndex = Random.Range(0, randomItems.Length);
                GameObject item = randomItems[itemIndex];
                GameObject goItem = Instantiate(item, hitFloor.transform.position, Quaternion.identity);
                goItem.name = item.name;
                goItem.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void ExitDoorway()
    {
        doorPos = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPos, Quaternion.identity);
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}
