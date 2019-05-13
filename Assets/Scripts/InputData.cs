using UnityEngine;
using System.Collections;

// Class for storing data from user input
public class InputData
{
    public float duration = 0.1f;
    public Vector3 initialPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero;
    public float speed = 0.0f;
    public float displacement = 0.0f;

    // Populate class data using known variables
    public void CalculateInput()
    {
        displacement = Vector3.Distance(initialPosition, endPosition);
        if (duration > 0.0f)
        {
            speed = displacement / duration;
        }
        else
        {
            speed = 0.0f;
        }
    }
}
