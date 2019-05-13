using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TappableObject : MonoBehaviour 
{
    [Header("Settings")]
    [SerializeField]
    private float m_impulseForce = 1.0f;
    [SerializeField]
    private float m_gravity = -0.1f;

    [Header("Auto Generated")]
    [SerializeField]
    Rigidbody2D m_rigidBody = null;

    [Header("Internal Variables")]
    [SerializeField]
    private Vector3 m_defaultPos;  
    [SerializeField]
    private bool m_tapped = false;
    [SerializeField]
    private bool m_available = true;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        m_tapped = false;
        m_available = true;
        m_rigidBody.isKinematic = true;

        m_defaultPos = transform.position;
    }

    void OnValidate()
    {
        if (!m_rigidBody)
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        if (m_available)
        {
            m_tapped = true;
        }
    }

    void FixedUpdate()
    {
       BeginTap();
       TapUpdate();
    }

    void BeginTap()
    {
        if (!m_tapped)
        {
            return;
        }

        // TODO: preallocate this
        var force = new Vector2(0.0f, m_impulseForce);

        // Add impulse force
        m_rigidBody.isKinematic = false;
        m_rigidBody.AddForce(force, ForceMode2D.Impulse);

        m_tapped = false;
        m_available = false;
    }

    void TapUpdate()
    {
        if (m_available || m_tapped)
        {
            return;
        }

        // TODO: preallocate this
        var gravity = new Vector2(0.0f, m_gravity);

        // Add negative force
        m_rigidBody.AddForce(gravity, ForceMode2D.Force);

        if (transform.position.y < m_defaultPos.y)
        {
            m_available = true;
            m_rigidBody.isKinematic = true;
            m_rigidBody.velocity = Vector2.zero;
            transform.position = m_defaultPos;
        }
    }
}
