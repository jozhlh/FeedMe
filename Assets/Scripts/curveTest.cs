using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurveTest : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float t = 0.0f;

    [SerializeField]
    private float duration = 5.0f;

    [SerializeField]
    private Vector3 startPos = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField]
    private Vector3 endPos = new Vector3(0.0f, 0.0f, 1.0f);

    [SerializeField]
    private Transform m_transform;

    [SerializeField]
    private AnimationCurve m_curve;

    private float count = 0.0f;

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;

        if (count > duration)
        {
            count = 0.0f;
        }

        t = count / duration;

        var curvePoint = m_curve.Evaluate(t);

        m_transform.position = Vector3.LerpUnclamped(startPos, endPos, curvePoint);
    }


}
