//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: The hands used by the player in the vr interaction system
//
//=============================================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using System.Threading;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    // Links with an appropriate SteamVR controller and facilitates
    // interactions with objects in the virtual world.
    //-------------------------------------------------------------------------
    public class CustomHand : Hand
    {
        private new void FixedUpdate()
        {
            if (currentAttachedObject != null)
            {
                AttachedObject attachedInfo = currentAttachedObjectInfo.Value;
                if (attachedInfo.attachedObject != null)
                {
                    if (attachedInfo.HasAttachFlag(AttachmentFlags.VelocityMovement))
                    {
                        var customThrowable = attachedInfo.attachedObject.GetComponent<CustomThrowable>();
                        if (!customThrowable || !customThrowable.enableTwoHandScaling || !customThrowable.twoHandsAttached)
                            UpdateAttachedVelocity(attachedInfo);
                        else
                        {

                        }
                    }
                }
            }
        }
    }
 }
