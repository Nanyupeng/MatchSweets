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

    public GameObject gridPrefab;
    void Start()
    {
        sweetsPrefabDic = new Dictionary<SweetsType, GameObject>();
        sweets = new GameSweet[xColumn, yRow];

        InstantiateGrid();
        SweetsLiveDic();
        InstantiateSweet();
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
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
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

}
