using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour 
{
    [SerializeField]
    private AnimationCurve m_curve;

    private Vector3 m_defaultPos;

    private Vector3 m_prevMove;

    private Vector3 m_prevPosition;

    private Vector3 m_targetPosition;

    private bool dragging;

    private float m_timeSinceDrag = 0.0f;

    [SerializeField]
    private float m_resetTime = 2.0f;

    private float m_moveSpeed = 0.05f;

    private BallController m_ball;

    private Rect m_rect;

    private bool m_previouslyTouching;

    private float m_sqrSpeed;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        m_targetPosition = transform.position;
        m_prevPosition = transform.position;
        m_defaultPos = m_prevPosition;
        dragging = false;
        m_previouslyTouching = false;
        var pos = transform.position;
        var size = transform.localScale;
        m_rect = new Rect(pos.x, pos.y, size.x, size.y);
        
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        m_prevPosition = transform.position;
        float distance_to_screen = Camera.main.WorldToScreenPoint(m_prevPosition).z;
        m_prevMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen ));
    }

    void OnMouseDrag()
    {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen ));
        var moveDelta = pos_move - m_prevMove;
        m_prevMove = pos_move;
        m_prevPosition.y += moveDelta.y;
        m_targetPosition = m_prevPosition;
        dragging = true;
    }

    /// <summary>
    /// OnMouseUp is called when the user has released the mouse button.
    /// </summary>
    void OnMouseUp()
    {
        dragging = false;
        m_timeSinceDrag = 0.0f;
    }

    /// <summary>
    /// Called when the mouse is not any longer over the GUIElement or Collider.
    /// </summary>
    void OnMouseExit()
    {
        if (dragging)
        {
            dragging = false;
            m_timeSinceDrag = 0.0f;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        MoveToDefaultPos();
    }

    void FixedUpdate()
    {
        if (transform.position == m_targetPosition)
        {
            return;
        }
        m_sqrSpeed = m_moveSpeed * m_moveSpeed;

        var pos = transform.position;
        var diff = m_targetPosition - pos;
        if (diff.sqrMagnitude > m_sqrSpeed)
        {
            diff *= m_moveSpeed;
        }
        pos += diff;
        transform.position = pos;

    }

    void MoveToDefaultPos()
    {
        if (dragging)
        {
            return;
        }
        m_timeSinceDrag += Time.deltaTime;
        var t = m_timeSinceDrag / (m_resetTime * Vector3.Distance(m_prevPosition, m_defaultPos));
        var pos = m_defaultPos;
        pos.y = Mathf.Lerp(m_prevPosition.y, m_defaultPos.y, t);
        m_targetPosition = pos;
    }

    void UpdateRect()
    {
        var pos = transform.position;
        m_rect.center = new Vector2(pos.x, pos.z);

        if (Intersects())
        {
            
            m_previouslyTouching = true;
        }
        else if (m_previouslyTouching)
        {
            // stopped touching ball this frame
            m_previouslyTouching = false;
        }
    }

    bool Intersects()
    {
        Vector2 circleDistance = new Vector2();
        circleDistance.x = Mathf.Abs(m_ball.x - m_rect.x);
        circleDistance.y = Mathf.Abs(m_ball.y - m_rect.y);

        if (circleDistance.x > (m_rect.width/2 + m_ball.r)) { return false; }
        if (circleDistance.y > (m_rect.height/2 + m_ball.r)) { return false; }

        if (circleDistance.x <= (m_rect.width/2)) { return true; } 
        if (circleDistance.y <= (m_rect.height/2)) { return true; }

        var cornerDistance_sq = Mathf.Pow((circleDistance.x - (m_rect.width * 0.5f)), 2.0f) + Mathf.Pow((circleDistance.y - (m_rect.height * 0.5f)) , 2.0f);

        return (cornerDistance_sq <= Mathf.Pow(m_ball.r, 2.0f));
    }
}
