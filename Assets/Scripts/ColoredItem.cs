using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_sprite;
    
    public void SetColor(Color color)
    {
        m_sprite.color = color;
    }
}
