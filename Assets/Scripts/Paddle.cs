using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleManager : MonoBehaviour
{

    [SerializeField]
    private BallController m_ball;

    private Rect m_rect;

    private bool m_previouslyTouching;


    // Start is called before the first frame update
    void Start()
    {
        m_previouslyTouching = false;
        var pos = transform.position;
        var size = transform.localScale;
        m_rect = new Rect(pos.x, pos.y, size.x, size.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    
}
