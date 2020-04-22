using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum ColorType
{
    YELLOW,
    PURPLE,
    RED,
    BLUE,
    GREEN,
    PINK,
    ANY,
    COUNT
}

public class ColorSweet : MonoBehaviour
{
    [Serializable]
    public struct ColorSprite
    {
        public ColorType colorType;
        public Sprite sprite;
    }

    public ColorSprite[] colorSprites;
    private Dictionary<ColorType, Sprite> colorspriteDic;

    [HideInInspector]
    public SpriteRenderer sprite;
    public int colorNum
    {
        get { return colorSprites.Length; }
    }

    [SerializeField]
    private ColorType colorType;
    public ColorType ColorType
    {
        get
        {
            return colorType;
        }

        set
        {
            SetColor(value);
        }
    }
    void Awake()
    {
        colorspriteDic = new Dictionary<ColorType, Sprite>();
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        ColorLiveDic();
    }

    /// <summary>
    /// 将图片存入字典
    /// </summary>
    void ColorLiveDic()
    {
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorspriteDic.ContainsKey(colorSprites[i].colorType))
                colorspriteDic.Add(colorSprites[i].colorType, colorSprites[i].sprite);
        }
    }

    /// <summary>
    /// 设置图片
    /// </summary>
    /// <param name="_colorType"></param>
   public void SetColor(ColorType _colorType)
    {
        colorType = _colorType;
        if (colorspriteDic.ContainsKey(_colorType))
        {
            sprite.sprite = colorspriteDic[_colorType];
        }
    }
}
