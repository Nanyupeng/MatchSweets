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
        bool needRefill = true;
        while (needRefill)
        {
            yield return new WaitForSeconds(moveSpeed);
            while (OnFill())
            {
                yield return new WaitForSeconds(moveSpeed);
            }
            needRefill = ClearAllMatchSweet();
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

            if (MatchSweets(sweet1, sweet2.X, sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null)
            {
                int tempx = sweet1.X;
                int tempy = sweet1.Y;

                sweet1.MoveSweet.Move(sweet2.X, sweet2.Y, moveSpeed);
                sweet2.MoveSweet.Move(tempx, tempy, moveSpeed);
                ClearAllMatchSweet();
                StartCoroutine(AllFill());
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
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


    /// <summary>
    /// 行遍历思路
    /// 如果匹配到的甜品有3个或以上，那么我们就应该加入 行匹配 列表。
    /// 如果我们 行匹配 列表中存在3个或以上甜品，那么我们就应该依次对每一个甜品进行匹配，
    /// 如果有存在 列匹配 达到的甜品在2个或以上的时候（需要清除 列匹配 列表），我们就把我们的这几甜品加入我们的 列匹配 列表，
    /// 最后呢我们把全部满足条件的甜品，放在我们的 完成匹配 的列表中。
    /// 如果 行匹配 之后，我们 完成匹配 列表中的元素数达不到3个，
    /// 把三个 行匹配  列匹配 完全匹配 列表全部清空一下
    /// </summary>
    /// 列遍历思路
    /// 原理相同
    /// <param name="gameSweet"></param>
    /// <param name="newx"></param>
    /// <param name="newy"></param>
    /// <returns></returns>
    public List<GameSweet> MatchSweets(GameSweet gameSweet, int newx, int newy)
    {
        if (gameSweet.CanColor())
        {
            ColorType colorType = gameSweet.ColorComponet.ColorType;
            List<GameSweet> matchRowSweets = new List<GameSweet>();//行匹配
            List<GameSweet> matchLineSweets = new List<GameSweet>();//列匹配
            List<GameSweet> finishedMacthLineSweets = new List<GameSweet>();//完全匹配

            matchRowSweets.Add(gameSweet);

            #region 行遍历+L+T行遍历

            for (int i = 0; i <= 1; i++)
            {
                for (int xDistance = 1; xDistance < xColumn; xDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newx - xDistance;
                    }
                    else
                    {
                        x = newx + xDistance;
                    }
                    if (x < 0 || x >= xColumn)
                    {
                        break;
                    }

                    if (sweets[x, newy].CanColor() && sweets[x, newy].ColorComponet.ColorType == colorType)
                    {
                        matchRowSweets.Add(sweets[x, newy]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMacthLineSweets.Add(matchRowSweets[i]);
                }
            }

            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int yDistance = 1; yDistance < yRow; yDistance++)
                        {
                            int y;
                            if (j == 0)
                            {
                                y = newy - yDistance;
                            }
                            else
                            {
                                y = newy + yDistance;
                            }
                            if (y < 0 || y >= yRow)
                            {
                                break;
                            }
                            if (sweets[matchRowSweets[i].X, y].CanColor() && sweets[matchRowSweets[i].X, y].ColorComponet.ColorType == colorType)
                            {
                                matchLineSweets.Add(sweets[matchRowSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchLineSweets.Count < 2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMacthLineSweets.Add(matchLineSweets[j]);
                        }
                        break;
                    }
                }
            }

            if (finishedMacthLineSweets.Count >= 3)
            {
                return finishedMacthLineSweets;
            }

            #endregion
            matchLineSweets.Clear();
            matchRowSweets.Clear();

            matchLineSweets.Add(gameSweet);

            #region 列遍历+L+T行遍历
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        y = newy - yDistance;
                    }
                    else
                    {
                        y = newy + yDistance;
                    }
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }

                    if (sweets[newx, y].CanColor() && sweets[newx, y].ColorComponet.ColorType == colorType)
                    {
                        matchLineSweets.Add(sweets[newx, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMacthLineSweets.Add(matchLineSweets[i]);
                }
            }

            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < xColumn; xDistance++)
                        {
                            int x;
                            if (j == 0)
                            {
                                x = newx - xDistance;
                            }
                            else
                            {
                                x = newx + xDistance;
                            }
                            if (x < 0 || x >= xColumn)
                            {
                                break;
                            }
                            if (sweets[x, matchLineSweets[i].Y].CanColor() && sweets[x, matchLineSweets[i].Y].ColorComponet.ColorType == colorType)
                            {
                                matchRowSweets.Add(sweets[x, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMacthLineSweets.Add(matchRowSweets[j]);
                        }
                        break;
                    }
                }
            }

            if (finishedMacthLineSweets.Count >= 3)
            {
                return finishedMacthLineSweets;
            }
            #endregion
        }

        return null;
    }


    /// <summary>
    /// 清除糖果
    /// </summary>
    /// <returns></returns>
    public bool ClearSweet(int x, int y)
    {
        if (sweets[x, y].CanClear() && !sweets[x, y].ClearedSweet.IsClearing)
        {
            sweets[x, y].ClearedSweet.Clear();
            InstantiateGameSweet(x, y, SweetsType.EMPTY);
            return true;
        }
        return false;
    }

    //清除全部完成匹配列表
    private bool ClearAllMatchSweet()
    {
        bool needRefill = false;
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<GameSweet> matchList = MatchSweets(sweets[x, y], x, y);
                    if (matchList != null)
                    {
                        for (int i = 0; i < matchList.Count; i++)
                        {
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
}
