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

    private IEnumerator moveCoroutine;

    /// <summary>
    /// 糖果移动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(int x, int y, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(x, y, time);
        StartCoroutine(moveCoroutine);
    }

    IEnumerator MoveCoroutine(int x, int y, float time)
    {
        GameSweet.X = x;
        GameSweet.Y = y;

        Vector3 startPos = transform.position;
        Vector3 endPos = GameSweet.GameManager.CorrectPosition(x, y);

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            GameSweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        GameSweet.transform.position = endPos;
    }
}
