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


        new protected virtual IEnumerator Start()
        {
            var ret = base.Start();

            //InvokeRepeating("Algorithmus1",0.0f,0.5f);

            return ret;
        }

        new protected virtual void Update()
        {
            base.Update();

            Algorithmus1();
            Algorithmus2();

            if (grabPinchAction.GetStateDown(handType))
            {
                if (MainObject.Highlight != null && currentAttachedObject == null)
                {
                    AttachObject(MainObject.Highlight.gameObject, GrabTypes.Pinch);
                }
            }

            if (grabPinchAction.GetStateUp(handType))
            {
                if (MainObject.Highlight != null && currentAttachedObject != null)
                {
                    DetachObject(MainObject.Highlight.gameObject, true);
                }
            }
        }
        

        private List<VertexDummy> a2 = new List<VertexDummy>();
        private void Algorithmus1()
        {
            foreach (var mainObject in GameObject.FindObjectsOfType<MainObject>())
            {
                foreach (var vertex in mainObject.Mesh.Vertices)
                {
                    var vertexdummy = vertex.VertexDummy;


                    /*
                    if (vertex.Position.x <= (transform.position.x + MainObject.Radius) && vertex.Position.x >
                                                                                        (transform.position.x -
                                                                                         MainObject.Radius)
                                                                                        && vertex.Position.y <=
                                                                                        (transform.position.y +
                                                                                         MainObject.Radius) &&
                                                                                        vertex.Position.y >
                                                                                        (transform.position.y -
                                                                                         MainObject.Radius)
                                                                                        && vertex.Position.z <=
                                                                                        (transform.position.z +
                                                                                         MainObject.Radius) &&
                                                                                        vertex.Position.z >
                                                                                        (transform.position.z -
                                                                                         MainObject.Radius))
    */
                    var vertexPosition = vertexdummy.GetComponent<Transform>().position;

                    if ( Math.Abs(vertexPosition.x - transform.position.x) <= MainObject.Radius && Math.Abs(vertexPosition.y - transform.position.y) <= MainObject.Radius && Math.Abs(vertexPosition.z - transform.position.z) <= MainObject.Radius)
                    {
                        if (!a2.Contains(vertexdummy))
                            a2.Add(vertexdummy);
                    }
                    else
                    {
                        if (a2.Contains(vertexdummy))
                            a2.Remove(vertexdummy);
                    }
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
