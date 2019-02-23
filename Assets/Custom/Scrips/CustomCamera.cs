//=============================================================================
//
// Purpose: Hopefully to scale the player on button press
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class CustomCamera : MonoBehaviour
{
    // TODO: Change Object declarations
    private GameObject rightHand;
    private GameObject leftHand;

    private Vector3 initialDirection;
    private float initialHandsDistance = 0f;

    private GameObject camera;

    // Start is called before the first frame update
    //Set values for the hands
    public void Start()
    {
        rightHand = GameObject.Find("RightHand");
        leftHand = GameObject.Find("LeftHand");

        // TODO: Change to proper object
        camera = GameObject.Find("Camera");
    }

    protected virtual void FixedUpdate()
    {
        // TODO: Change button name
        if (Input.GetButtonDown("0"))
        {
            var newDirection = rightHand.transform.position -
                leftHand.transform.position;

            if (newDirection.x == initialDirection.x && newDirection.y == initialDirection.y && newDirection.z == initialDirection.z)
                return;

            var newDistance = newDirection.sqrMagnitude;
            camera.transform.localScale *= newDistance / initialHandsDistance;

            initialHandsDistance = newDistance;
            initialDirection = newDirection;
        }
    }
}
