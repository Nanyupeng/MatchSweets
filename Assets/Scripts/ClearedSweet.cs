using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearedSweet : MonoBehaviour
{

    public AnimationClip clearAnim;

    private bool isClearing;
    public bool IsClearing
    {
        get
        {
            return isClearing;
        }
    }

    protected GameSweet sweet;

    public virtual void Clear()
    {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    IEnumerator ClearCoroutine()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play(clearAnim.name);
            yield return new WaitForSeconds(clearAnim.length);
            Destroy(gameObject);
        }
    }
}
