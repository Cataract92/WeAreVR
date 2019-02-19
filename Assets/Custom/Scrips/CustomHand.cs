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

        [SteamVR_DefaultAction("Trigger")]
        public SteamVR_Action_Boolean triggerAction;

        [HideInInspector]
        public ManipulationTool.ToolType CurrentToolType = ManipulationTool.ToolType.HAND;

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

        new protected virtual void Update()
        {
            base.Update();

            List<VertexDummy> dummies = new List<VertexDummy>();


            switch (CurrentToolType)
            {
                case ManipulationTool.ToolType.HAND:
                {
                    foreach (var mainObject in GameObject.FindObjectsOfType<MainObject>())
                    {
                        foreach (var vertex in mainObject.Mesh.Vertices)
                        {
                            dummies.Add(vertex.VertexDummy);
                        }
                    }

                    break;
                }
                case ManipulationTool.ToolType.PLUNGER:
                {
                    foreach (var mainObject in GameObject.FindObjectsOfType<MainObject>())
                    {
                        foreach (var face in mainObject.Mesh.Faces)
                        {
                            dummies.Add(face.FaceDummy);
                        }
                    }

                    break;
                    }
            }

            Algorithmus1(dummies);
            Algorithmus2();

            if (triggerAction.GetStateDown(handType))
            {
                if (MainObject.Highlight != null && currentAttachedObject == null)
                {
                    MainObject.Highlight.InitialFaceRotation = Quaternion.FromToRotation( transform.forward, -MainObject.Highlight.transform.forward);
                    AttachObject(MainObject.Highlight.gameObject,GrabTypes.Trigger);
                    MainObject.Highlight.IsAttached = true;
                }
            }

            if (triggerAction.GetStateUp(handType))
            {
                if (MainObject.Highlight != null && currentAttachedObject != null)
                {
                    DetachObject(MainObject.Highlight.gameObject, true);
                    MainObject.Highlight.transform.rotation *= Quaternion.Inverse(MainObject.Highlight.InitialFaceRotation);
                    MainObject.Highlight.IsAttached = false;
                }
            }
        }
        
        private List<VertexDummy> a2 = new List<VertexDummy>();

        private void Algorithmus1(List<VertexDummy> dummies)
        {
                foreach (var dummy in dummies)
                {
                    var vertexPosition = dummy.GetComponent<Transform>().position;

                    if ( Math.Abs(vertexPosition.x - transform.position.x) <= MainObject.Radius && Math.Abs(vertexPosition.y - transform.position.y) <= MainObject.Radius && Math.Abs(vertexPosition.z - transform.position.z) <= MainObject.Radius)
                    {
                        if (!a2.Contains(dummy))
                            a2.Add(dummy);
                    }
                    else
                    {
                        if (a2.Contains(dummy))
                            a2.Remove(dummy);
                    }
                }
        }


        private void Algorithmus2()
        {
            float supercount = (Math.Abs(transform.position.x) + Math.Abs(transform.position.y) + Math.Abs(transform.position.z)) +
                               (3 * MainObject.Radius) + 1;

            foreach (var dummy in a2)
            {
                float count = 0f;
                count += Math.Abs(dummy.transform.position.x - transform.position.x);
                count += Math.Abs(dummy.transform.position.y - transform.position.y);
                count += Math.Abs(dummy.transform.position.z - transform.position.z);

                if (count < supercount)
                {
                    supercount = count;
                    MainObject.Highlight = dummy;
                }
            }

            if (a2.Count == 0)
                MainObject.Highlight = null;
        }
    }
 }
