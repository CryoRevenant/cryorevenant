using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCutSprite : MonoBehaviour
{
    [SerializeField] Sprite changeSprite;
    [SerializeField] SpriteRenderer spriteTitle;

    public void ChangeSprite()
    {
        spriteTitle.gameObject.GetComponent<Animator>().enabled = false;
        spriteTitle.sprite = changeSprite;

        Invoke("AnimBack", 1);
    }

    void AnimBack()
    {
        spriteTitle.gameObject.GetComponent<Animator>().enabled = true;
    }
}
