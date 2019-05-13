using UnityEngine;
using System.Collections;

public class GameInput : MonoBehaviour
{
    // The accepted directions of a swipe input
    public enum Direction { NW, N, NE, W, E, SW, S, SE };

    // Available mousebuttons and the total thereof
    private enum MouseButtons { Left, Right, Middle, NumMouseButtons };

    // Delegate functions for input
    public delegate void OnTapCallback(Vector3 position);
    public delegate void OnDragCallback(Vector3 position);
    public delegate void OnEndDragCallback(Vector3 position);
    public delegate void OnSwipeCallback(Direction direction);
    public delegate void OnBeginDragCallback(Vector3 position);

    // Callback events for received input
    public static event OnTapCallback OnTap;
    public static event OnDragCallback OnDrag;
    public static event OnEndDragCallback OnEndDrag;
    public static event OnBeginDragCallback OnBeginDrag;
    public static event OnSwipeCallback OnSwipe;
  
    // How many mousebuttons are available
    private const int NUM_MOUSE_BUTTONS = (int)MouseButtons.NumMouseButtons;
    // Used for determining if a swipe was straight or diagonal
    private const float DIAGONAL_THRESHOLD_LOW = 0.462f;
    private const float DIAGONAL_THRESHOLD_HIGH = 0.887f;
    // how long the screen is pressed before it is a hold
    private const float HOLD_THRESHOLD = 0.75f;

    // Start time of an input
    private float m_startTime;
    private float m_clickStart;
    // If the mousebuttons are down
    private bool[] m_mouseButtons;
    // If the mousebuttons wer down last frame
    private bool[] m_prevMouseButtons;
    // The data received this frame about user input
    private bool mTouch = false;
    private bool mTouchLast = false;
    private bool mClick = false;
    private bool mClickLast = false;
    private InputData m_currentInput;
    private InputData m_currentClick;
    // if the player is currently holding down on the screen
    private bool m_isHolding = false;
    private bool m_isPotentiallyDragging = false;

    // Use this for initialization
    void Start()
    {
        // Create arrays for available mouse buttons and initialise to false
        m_mouseButtons = new bool[NUM_MOUSE_BUTTONS];
        m_prevMouseButtons = new bool[NUM_MOUSE_BUTTONS];
        for (int count = 0; count < NUM_MOUSE_BUTTONS; count++)
        {
            m_mouseButtons[count] = false;
            m_prevMouseButtons[count] = false;
        }
        // Interpret touch input as mouse input for calculations
        Input.simulateMouseWithTouches = true;
        m_isHolding = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetNewInput();
    }

    
    private void GetNewInput()
    {
        // Cache the last frame mouse status and read in the current mouse status 
        for (int i = 0; i < NUM_MOUSE_BUTTONS; i++)
        {
            m_prevMouseButtons[i] = m_mouseButtons[i];
            m_mouseButtons[i] = Input.GetMouseButton(i);
        }

        // Initialise tap, swipe and direction status
        var tap = false;
        var swipe = false;
        var direction = Direction.W;

        // If the screen was just touched, create a new input container and store starting information
        if (MouseButtonJustPressed(MouseButtons.Left))
        {
            m_startTime = Time.time;

            m_currentInput = new InputData();
            m_currentInput.duration = 0.0f;
            m_currentInput.initialPosition = Input.mousePosition;
            m_currentInput.endPosition = Input.mousePosition;

            // This could be a drag
            m_isPotentiallyDragging = true;
        }
        // If the screen is still being touched, record it's latest position and the duration it has been held for
        else if (MouseButtonHeld(MouseButtons.Left))
        {
            m_currentInput.duration = Time.time - m_startTime;
            m_currentInput.endPosition = Input.mousePosition;

            if (m_currentInput.duration > HOLD_THRESHOLD)
            {
                // the player is pressing the screen
                if (!m_isHolding)
                {
                    if (OnBeginDrag != null)
                    {
                        OnBeginDrag(Input.mousePosition);
                    }
                    m_isHolding = true;
                }
            }

            // If this could still be a drag, send input information
            if (m_isPotentiallyDragging)
            {
               // Debug.Log("Dragging Object");
                //OnDrag(Input.mousePosition);
            }
        }
        // If the screen is no longer being touched, and the player was not holding calculate whether it was a tap or a swipe
        else if (MouseButtonJustReleased(MouseButtons.Left))
        {
            if (!m_isHolding)
            {
                if(m_currentInput == null)
                {
                    m_currentInput = new InputData();
                    Debug.Log("InputData Exception Caught");
                }
                m_currentInput.endPosition = Input.mousePosition;
                m_currentInput.CalculateInput();

                // If this could still be a drag, send final position
                if (m_isPotentiallyDragging)
                {
                    OnEndDrag(Input.mousePosition);
                }

                // If the displacement was under the sensitivity threshold, it is a tap
                if (m_currentInput.speed < (GameSettings.sensitivity - m_currentInput.displacement))
                {
                    tap = true;
                }
                // If it was not a tap, calculate the direction of swipe based on vector between start and end point
                else
                {
                    swipe = true;
                    Vector3 difference = m_currentInput.initialPosition - m_currentInput.endPosition;
                    direction = CalculateMobileDirection(-difference);
                }
            }
        }

        // If a tap or a swipe was completed this frame, notify Callback Event
        if (tap || swipe)
        {
            // If it was a tap, send the position of the tap
            if (tap && OnTap != null)
            {
                OnTap(Input.mousePosition);
            }
            // If it was a swipe send the direction of the swipe
            else if (swipe && OnSwipe != null)
            {
                OnSwipe(direction);
            }
        }
    }

    public static void ResetSwipe()
    {
        OnSwipe = null;
    }

    public static void ResetTap()
    {
        if (OnTap != null)
        {
            OnTap = null;
        }
    }

    public static void ResetNewHold()
    {
        if (OnBeginDrag != null)
        {
            OnBeginDrag = null;
        }
    }

    
    public static void ResetRotateHold()
    {
        if (OnEndDrag != null)
        {
            OnEndDrag = null;
        }
    }

    public static void ResetHold()
    {
        if (OnDrag != null)
        {
            OnDrag = null;
        }
    }

    // Check if tap already has a delegate assigned
    public static bool CanAddToTap()
    {
        if (OnTap == null)
        {
            return true;
        }
        return false;
    }

    // Check if tap already has a delegate assigned
    public static bool CanAddToHold()
    {
        if (OnDrag == null)
        {
            return true;
        }
        return false;
    }


    private Direction CalculateMobileDirection(Vector3 input)
    {
        Direction swipeDirection = Direction.W;

        input.Normalize();
        if ((input.x > -DIAGONAL_THRESHOLD_LOW) & (input.x < DIAGONAL_THRESHOLD_LOW))
        {
            if (input.y > 0)
            {
                // N
                swipeDirection = Direction.N;
            }
            else
            {
                // S
                swipeDirection = Direction.S;
            }
        }
        else if ((input.x > DIAGONAL_THRESHOLD_LOW) & (input.x < DIAGONAL_THRESHOLD_HIGH))
        {
            if (input.y > 0)
            {
                // NE
                swipeDirection = Direction.NE;
            }
            else
            {
                // SE
                swipeDirection = Direction.SE;
            }
        }
        else if ((input.x > -DIAGONAL_THRESHOLD_HIGH) & (input.x < -DIAGONAL_THRESHOLD_LOW))
        {
            if (input.y > 0)
            {
                // NW
                swipeDirection = Direction.NW;
            }
            else
            {
                // SW
                swipeDirection = Direction.SW;
            }
        }
        else if (input.x > DIAGONAL_THRESHOLD_HIGH)
        {
            // E
            swipeDirection = Direction.E;
        }
        else if (input.x < -DIAGONAL_THRESHOLD_HIGH)
        {
            // W
            swipeDirection = Direction.W;
        }

        switch(swipeDirection)
        {
            case Direction.NW:
            case Direction.SW:
                swipeDirection = Direction.W;
                break;
            case Direction.NE:
            case Direction.SE:
                swipeDirection = Direction.E;
                break;

        }

        return swipeDirection;
    }

    private bool NewClick()
    {
        return mClick && !mClickLast;
    }

    private bool ReleasedClick()
    {
        return !mClick && mClickLast;
    }

    private bool HeldClick()
    {
        return mClick && mClickLast;
    }

    private bool NewTouch()
    {
        return mTouch && !mTouchLast;
    }

    private bool ReleasedTouch()
    {
        return !mTouch && mTouchLast;
    }

    private bool HeldTouch()
    {
        return mTouch && mTouchLast;
    }

    // Check the status of the mousebuttons against their cached data from last frame
    private bool MouseButtonJustPressed(MouseButtons button)
    {
        return m_mouseButtons[(int)button] && !m_prevMouseButtons[(int)button];
    }

    private bool MouseButtonJustReleased(MouseButtons button)
    {
        return !m_mouseButtons[(int)button] && m_prevMouseButtons[(int)button];
    }

    private bool MouseButtonHeld(MouseButtons button)
    {
        return m_mouseButtons[(int)button] && m_prevMouseButtons[(int)button];
    }

    public void DragSuccess()
    {
        m_isPotentiallyDragging = true;
    }

    public void DragFailed()
    {
        m_isPotentiallyDragging = false;
    }
}