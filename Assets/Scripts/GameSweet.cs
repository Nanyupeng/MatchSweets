using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{

    private int x;
    public int X
    {
        get
        {
            return x;
        }

        set
        {
            if (CanMove())
                x = value;
        }
    }

    private int y;
    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            if (CanMove())
                y = value;
        }
    }

    private SweetsType sweetsType;
    public SweetsType SweetsType
    {
        get
        {
            return sweetsType;
        }
    }

    private GameManager gameManager;
    public GameManager GameManager
    {
        get
        {
            return gameManager;
        }

        set
        {
            gameManager = value;
        }
    }

    private MoveSweet moveSweet;
    public MoveSweet MoveSweet
    {
        get
        {
            if (moveSweet == null)
            {
                moveSweet = GetComponent<MoveSweet>();
                return moveSweet;
            }
            return moveSweet;
        }
    }

    private ColorSweet colorComponet;
    public ColorSweet ColorComponet
    {
        get
        {
            if (colorComponet == null)
            {
                colorComponet = GetComponent<ColorSweet>();
                return colorComponet;
            }
            return colorComponet;
        }
    }

    private ClearedSweet clearedSweet;
    public ClearedSweet ClearedSweet
    {
        get
        {
            if (clearedSweet == null)
            {
                clearedSweet = GetComponent<ClearedSweet>();
                return clearedSweet;
            }
            return clearedSweet;
        }


    }

    /// <summary>
    /// 是否可以移动
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        return MoveSweet != null;
    }

    /// <summary>
    /// 改变图片的脚本是否存在
    /// </summary>
    /// <returns></returns>
    public bool CanColor()
    {
        return ColorComponet != null;
    }

    /// <summary>
    /// 是否可以清除
    /// </summary>
    /// <returns></returns>
    public bool CanClear()
    {
        return ClearedSweet != null;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param x="_x"></param>
    /// <param y="_y"></param>
    /// <param manager="manager"></param>
    public void Init(int _x, int _y, GameManager _manager, SweetsType _type)
    {
        X = _x;
        Y = _y;
        GameManager = _manager;
        sweetsType = _type;
    }

    void OnMouseEnter()
    {
        GameManager.EnterSweet(this);
    }

    void OnMouseDown()
    {
        GameManager.PressSweet(this);
    }

    void OnMouseUp()
    {
        GameManager.UpSweet();
    }

}
