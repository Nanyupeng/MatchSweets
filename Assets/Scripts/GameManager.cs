using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
/// <summary>
/// 甜品类型
/// </summary>
public enum SweetsType
{
    EMPTY,
    NORMAL,
    BARRIER,
    ROW_CLEAR,
    COLIMN_CLEAR,
    RAINBOWCANDY,
    COUNT//标记类型
}
public class GameManager : MonoBehaviour
{

    /// <summary>
    /// 单例
    /// </summary>
    private static GameManager _gameManager;
    public GameManager gameManager()
    {
        if (_gameManager == null)
        {
            _gameManager = GetComponent<GameManager>();
            return _gameManager;
        }
        return _gameManager;
    }

    /// <summary>
    /// 根据甜品种类获取甜品物体
    /// </summary>
    private Dictionary<SweetsType, GameObject> sweetsPrefabDic;
    [Serializable]
    public struct SweetsPrefab
    {
        public SweetsType sweetsType;
        public GameObject sweetsObj;
    }
    public SweetsPrefab[] sweetsPrefabs;
    private GameSweet[,] sweets;

    /// <summary>
    /// 生成的行列
    /// </summary>
    public int xColumn;
    public int yRow;

    public float moveSpeed;

    public GameObject gridPrefab;

    //需要交换的两个对象
    private GameSweet pressedSweet;
    private GameSweet enteredSweet;

    void Start()
    {
        sweetsPrefabDic = new Dictionary<SweetsType, GameObject>();
        sweets = new GameSweet[xColumn, yRow];

        InstantiateGrid();
        SweetsLiveDic();
        InstantiateSweet();

        Destroy(sweets[4, 4]);
        InstantiateGameSweet(4, 4, SweetsType.BARRIER);
        StartCoroutine(AllFill());
    }

    /// <summary>
    /// 生成格子
    /// </summary>
    void InstantiateGrid()
    {
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject grid = Instantiate(gridPrefab, CorrectPosition(x, y), Quaternion.identity);
                grid.transform.SetParent(transform);
            }
        }
    }

    /// <summary>
    /// 坐标修正
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - xColumn / 2 + x + 0.5f, transform.position.y + yRow / 2 - y + 0.5f);
    }

    /// <summary>
    /// 存入字典
    /// </summary>
    void SweetsLiveDic()
    {
        for (int i = 0; i < sweetsPrefabs.Length; i++)
        {
            if (!sweetsPrefabDic.ContainsKey(sweetsPrefabs[i].sweetsType))
                sweetsPrefabDic.Add(sweetsPrefabs[i].sweetsType, sweetsPrefabs[i].sweetsObj);
        }
    }

    /// <summary>
    /// 生成游戏道具
    /// </summary>
    void InstantiateSweet()
    {
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                InstantiateGameSweet(x, y, SweetsType.EMPTY);
            }
        }
    }

    /// <summary>
    /// 产生甜品的方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sweetsType"></param>
    /// <returns></returns>
    GameSweet InstantiateGameSweet(int x, int y, SweetsType sweetsType)
    {
        GameObject newSweet = Instantiate(sweetsPrefabDic[sweetsType], CorrectPosition(x, y), Quaternion.identity);
        newSweet.transform.SetParent(transform);

        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, sweetsType);
        return sweets[x, y];
    }


    IEnumerator AllFill()
    {
        while (OnFill())
        {
            yield return new WaitForSeconds(moveSpeed);
        }
    }

    bool OnFill()
    {
        bool fillNotFinished = false;
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                GameSweet gameSweet = sweets[x, y];
                if (gameSweet.CanMove())
                {
                    GameSweet sweetBelow = sweets[x, y + 1];
                    if (sweetBelow.SweetsType == SweetsType.EMPTY)//垂直填充
                    {
                        Destroy(sweetBelow.gameObject);
                        gameSweet.MoveSweet.Move(x, y + 1, moveSpeed);
                        sweets[x, y + 1] = gameSweet;
                        InstantiateGameSweet(x, y, SweetsType.EMPTY);
                        fillNotFinished = true;
                    }
                    else   //斜向填充
                    {
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = x + down;
                                if (downX >= 0 && downX < xColumn)
                                {
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.SweetsType == SweetsType.EMPTY)
                                    {
                                        bool canfill = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.SweetsType != SweetsType.EMPTY)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }

                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            gameSweet.MoveSweet.Move(downX, y + 1, moveSpeed); ;
                                            sweets[downX, y + 1] = gameSweet;
                                            InstantiateGameSweet(x, y, SweetsType.EMPTY); ;
                                            fillNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xColumn; x++)
        {
            GameSweet sweet = sweets[x, 0];
            if (sweet.SweetsType == SweetsType.EMPTY)
            {
                GameObject newSweet = Instantiate(sweetsPrefabDic[SweetsType.NORMAL], CorrectPosition(x + 1, 0), Quaternion.identity);
                newSweet.transform.SetParent(transform);

                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, 0, this, SweetsType.NORMAL);
                sweets[x, 0].MoveSweet.Move(x, 0, moveSpeed);
                sweets[x, 0].ColorComponet.SetColor((ColorType)Random.Range(0, sweets[x, 0].ColorComponet.colorNum));
                fillNotFinished = true;
            }
        }
        return fillNotFinished;
    }


    private bool IsFriend(GameSweet sweet1, GameSweet sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    private void ExchangeSweets(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            int tempx = sweet1.X;
            int tempy = sweet1.Y;

            sweet1.MoveSweet.Move(sweet2.X, sweet2.Y, moveSpeed);
            sweet2.MoveSweet.Move(tempx, tempy, moveSpeed);

        }
    }

    public void PressSweet(GameSweet sweet)
    {
        pressedSweet = sweet;
    }

    public void EnterSweet(GameSweet sweet)
    {
        enteredSweet = sweet;
    }

    public void UpSweet()
    {
        if (IsFriend(pressedSweet, enteredSweet))
            ExchangeSweets(pressedSweet, enteredSweet);
    }
}
