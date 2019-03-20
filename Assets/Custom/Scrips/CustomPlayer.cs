//=============================================================================
//
// Purpose: Hopefully to scale the player on button press
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class CustomPlayer : MonoBehaviour
{
    // TODO: Change Object declarations
    private GameObject rightHand;
    private GameObject leftHand;

    private Vector3 initialDirection;
    private float initialHandsDistance = 0f;

    private GameObject Player;

    public SteamVR_Action_Boolean ScaleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Scale");
    public float MaxScale = 10f;
    public float MinScale = 0.01f;

    private bool _isScaling = false;


    // Start is called before the first frame update
    //Set values for the hands
    public void Start()
    {
        rightHand = GameObject.Find("RightHand");
        leftHand = GameObject.Find("LeftHand");

        // TODO: Change to proper object
        Player = GameObject.Find("Player");
    }

    protected virtual void FixedUpdate()
    {
        // TODO: Change button name
        if (ScaleAction.GetStateDown(SteamVR_Input_Sources.RightHand) || ScaleAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            initialDirection = rightHand.transform.position -
                    leftHand.transform.position;
            initialHandsDistance = initialDirection.magnitude;
            _isScaling = true;
            return;
        }

        if (ScaleAction.GetStateUp(SteamVR_Input_Sources.RightHand) || ScaleAction.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            _isScaling = false;
        }

        if (_isScaling)
        {
            var newDirection = rightHand.transform.position -
                leftHand.transform.position;

            if (newDirection.x == initialDirection.x && newDirection.y == initialDirection.y && newDirection.z == initialDirection.z)
                return;

            var newDistance = newDirection.magnitude;
            Player.transform.localScale *= ((newDistance / initialHandsDistance) + 1)/2;

            

            if (Player.transform.localScale.x <= MinScale)
                Player.transform.localScale = new Vector3(MinScale, MinScale, MinScale);

            if (Player.transform.localScale.x >= MaxScale)
                Player.transform.localScale = new Vector3(MaxScale, MaxScale, MaxScale);

            initialHandsDistance = newDistance;
            initialDirection = newDirection;
            
        
    }
    }
}