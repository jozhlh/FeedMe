using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float distFromCameraToScene = 1000.0f;

    [Header("References")]
    private Ray touchRay;
  
    void OnDrawGizmos()
    {
        Gizmos.DrawRay(touchRay);
    }

    // Use this for initialization
    void Start () 
    {
        GameInput.ResetTap();
        if (GameInput.CanAddToTap())
        {
            GameInput.OnTap += PlayerTap;
        }
    }

    public void PlayerTap(Vector3 position)
    {

        RaycastHit hit;
        touchRay = Camera.main.ScreenPointToRay(position);

        // Bit shift the index of the layer (9) to get a bit mask
        int layerMask = 1 << 9;

        // use {layerMask = ~layerMask;} to invert the selection
        if (Physics.Raycast(touchRay, out hit, distFromCameraToScene, layerMask))
        {
            // The touch intersected something in the scene
            if (hit.collider.tag == "Tile")
            {

            }
            else if (hit.collider.tag == "Toy")
            {

            }
        }
    }

    public void DisableTaps()
    {
        GameInput.ResetTap();
    }

    public void EnableTaps()
    {
        GameInput.ResetTap();
        GameInput.OnTap += PlayerTap;
    }

    private bool IsOverUi()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
        return EventSystem.current.IsPointerOverGameObject();
#endif
    }
}
