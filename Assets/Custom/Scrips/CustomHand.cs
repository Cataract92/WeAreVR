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

        public SteamVR_Action_Boolean triggerAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Trigger");

        public ManipulationTool.ToolType CurrentToolType = ManipulationTool.ToolType.HAND;

        private VertexDummy _highlight;
        public VertexDummy Highlight
        {
            get { return _highlight; }
            set
            {
                if (_highlight != null)
                    _highlight.UndoHighlight();
                _highlight = value;
                if (_highlight != null)
                    _highlight.DoHighlight();
            }
        }


        private Transform _playerTransform;

        protected virtual new IEnumerator Start()
        {
            var ret = base.Start();
            _playerTransform = GameObject.Find("Player").transform;
            return ret;
        }

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

            if (triggerAction.GetStateDown(handType))
            {
                if (Highlight != null && currentAttachedObject == null)
                {
                    if (CurrentToolType == ManipulationTool.ToolType.HAND || CurrentToolType == ManipulationTool.ToolType.PLUNGER)
                    {
                        if (Highlight.Vertex != null || CurrentToolType == ManipulationTool.ToolType.PLUNGER)
                        {
                            Highlight.InitialFaceRotation = Quaternion.Inverse(transform.rotation);
                            AttachObject(Highlight.gameObject, GrabTypes.Trigger);
                            Highlight.IsAttached = true;
                        }
                        else
                        {
                            var v = new CustomMesh.Vertex(Highlight.transform.localPosition, Highlight.Face.Mesh, Highlight.Face, Highlight);
                            AttachObject(v.VertexDummy.gameObject, GrabTypes.Trigger);
                            Highlight.IsAttached = true;
                        }

                    }

                    if (CurrentToolType == ManipulationTool.ToolType.ERASER)
                    {
                        if (Highlight.Face != null)
                        {
                            Destroy(Highlight.Face.Mesh.MainObject);
                        }
                        else
                        {
                            Highlight.Vertex.Remove();

                        }
                        Highlight = null;
                    }
                }
            }

            if (triggerAction.GetStateUp(handType))
            {
                if (CurrentToolType == ManipulationTool.ToolType.CUBE)
                {
                    ManipulationTool go = null;
                    var objects = GameObject.FindObjectsOfType<ManipulationTool>();
                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].Type == ManipulationTool.ToolType.CUBE)
                            go = objects[i];
                    }
                    var o = GameObject.Instantiate(go.MainObjectPrefab);

                    o.transform.position = transform.position;
                    o.transform.rotation = transform.rotation;
                    o.transform.localScale = FindObjectOfType<CustomPlayer>().transform.localScale * 0.1f;

                }
                else if (Highlight != null && currentAttachedObject != null)
                {
                    DetachObject(Highlight.gameObject, true);
                    Highlight.transform.rotation *= Quaternion.Inverse(Highlight.InitialFaceRotation);
                    Highlight.IsAttached = false;
                }
            }


            if (currentAttachedObject != null)
                return;

            List<VertexDummy> dummies = new List<VertexDummy>();

            switch (CurrentToolType)
            {
                case ManipulationTool.ToolType.ERASER:
                case ManipulationTool.ToolType.HAND:
                {
                    foreach (var mainObject in GameObject.FindObjectsOfType<MainObject>())
                    {
                        foreach (var vertex in mainObject.Mesh.Vertices)
                        {
                            if(vertex != null && vertex.VertexDummy != null)
                                dummies.Add(vertex.VertexDummy);
                        }
                        
                        foreach (var face in mainObject.Mesh.Faces)
                        {
                            if (face != null && face.FaceDummy != null)
                                dummies.Add(face.FaceDummy);
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
                            if (face != null && face.FaceDummy != null)
                                dummies.Add(face.FaceDummy);
                        }

                            foreach (var edge in mainObject.Mesh.Edges)
                            {
                                if (edge != null && edge.EdgeDummy != null)
                                    dummies.Add(edge.EdgeDummy);
                            }
                        }
                    break;
                }
            }

            Algorithmus2(Algorithmus1(dummies));

            
        }
        

        private List<VertexDummy> Algorithmus1(List<VertexDummy> dummies)
        {
            List<VertexDummy> list = new List<VertexDummy>();
            foreach (var dummy in dummies)
            {
                var vertexPosition = dummy.GetComponent<Transform>().position;

                if ( Math.Abs(vertexPosition.x - transform.position.x) <= MainObject.Radius * _playerTransform.localScale.x && Math.Abs(vertexPosition.y - transform.position.y) <= MainObject.Radius * _playerTransform.localScale.x && Math.Abs(vertexPosition.z - transform.position.z) <= MainObject.Radius * _playerTransform.localScale.x)
                {
                    if (!list.Contains(dummy))
                        list.Add(dummy);
                }
                else
                {
                    if (list.Contains(dummy))
                        list.Remove(dummy);
                }
            }
            return list;
        }


        private void Algorithmus2(List<VertexDummy> list)
        {
            float supercount = (Math.Abs(transform.position.x) + Math.Abs(transform.position.y) + Math.Abs(transform.position.z)) +
                               (3 * MainObject.Radius * _playerTransform.localScale.x) + 1;

            foreach (var dummy in list)
            {
                float count = 0f;
                count += Math.Abs(dummy.transform.position.x - transform.position.x);
                count += Math.Abs(dummy.transform.position.y - transform.position.y);
                count += Math.Abs(dummy.transform.position.z - transform.position.z);

                if (count < supercount)
                {
                    supercount = count;
                    Highlight = dummy;
                }
            }

            if (list.Count == 0)
                Highlight = null;
        }
    }
 }
