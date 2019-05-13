using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Vector3 startPos;

    Quaternion startRotation;

    Vector3 m_velocity;

    Vector3 m_gravity;

    bool m_touchedByPaddle;

    float m_radius;

    public float r { get => m_radius; }
    public float x { get => transform.position.x; }
    public float y { get => transform.position.y; }

    // Start is called before the first frame update
    void Start()
    {
        m_radius = transform.localScale.x * 0.5f;
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForReset();
        if (!m_touchedByPaddle)
        {
            AddGravity();
        }
        ApplyVelocity();
    }

    void CheckForReset()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetBall();
        } 
    }

    public void ResetBall()
    {
        transform.position = startPos;
        transform.rotation = startRotation;
    }

    void AddGravity()
    {
        m_velocity += m_gravity;
    }

    void ApplyVelocity()
    {
        var pos = transform.position;
        pos += m_velocity;
        transform.position = pos;
        m_touchedByPaddle = false;
    }

    public void PaddleTouching(Vector3 velocity)
    {
        if (m_touchedByPaddle)
        {
            if (velocity.y > m_velocity.y)
            {
                m_velocity.y = velocity.y;
            }
        }
        else
        {
            m_velocity.y = velocity.y;
        }
        m_touchedByPaddle = true;
    }
}
