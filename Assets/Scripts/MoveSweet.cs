using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    private GameSweet gameSweet;
    public GameSweet GameSweet
    {
        get
        {
            if (gameSweet == null)
            {
                gameSweet = GetComponent<GameSweet>();
                return gameSweet;
            }
            return gameSweet;
        }
    }

    /// <summary>
    /// 糖果移动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(int x, int y)
    {
        GameSweet.X = x;
        GameSweet.Y = y;
        GameSweet.transform.position = GameSweet.GameManager.CorrectPosition(x, y);
    }
}
