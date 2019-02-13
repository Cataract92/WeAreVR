using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ManipulationTool : MonoBehaviour
{



    public enum ToolType
    {
        HAND,
        PLUNGER,
        ERASER,
        PLIER
    }

    public ToolType Type;
    public GameObject ModelPrefab;
    public float Radius = 0.1f;

    public Vector3 PositionOffset;
    public Vector3 RotationOffset;

    private Dictionary<CustomHand,bool> _isInRangeDictionary = new Dictionary<CustomHand, bool>();
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var hand in FindObjectsOfType<CustomHand>())
        {
            _isInRangeDictionary.Add(hand,false);
        }

        _camera = GameObject.FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (Type)
        {
            case ToolType.PLUNGER:
            {
                transform.position = _camera.transform.position + new Vector3(0, -0.6f, 0) + _camera.transform.right * 0.2f;
                    break;
            }
            case ToolType.ERASER:
            {
                transform.position = _camera.transform.position + new Vector3(0, -0.6f, 0) - _camera.transform.right * 0.2f;
                    break;
            }
            case ToolType.PLIER:
            {
                transform.position = _camera.transform.position + new Vector3(0, -0.6f, 0) + _camera.transform.forward * 0.2f;
                    break;
            }
        }
       

        foreach (var hand in _isInRangeDictionary.Keys)
        {
            if (Vector3.Distance(hand.transform.position, transform.position) < Radius && _isInRangeDictionary[hand] == false)
            {
                if (hand.CurrentToolType == Type)
                {
                    Destroy(hand.GetComponentInChildren<DummyTool>().gameObject);
                    hand.CurrentToolType = ToolType.HAND;
                    hand.ShowController();
                }
                else
                {
                    if (hand.CurrentToolType != ToolType.HAND)
                        Destroy(hand.GetComponentInChildren<DummyTool>().gameObject);

                    var tool = Instantiate(ModelPrefab);

                    tool.AddComponent<DummyTool>();
                    tool.transform.parent = hand.transform;
                    tool.transform.localPosition = PositionOffset;
                    tool.transform.localRotation = Quaternion.Euler(RotationOffset);

                    hand.HideController();

                    hand.CurrentToolType = Type;
                }
                _isInRangeDictionary[hand] = true;
            }
            if (Vector3.Distance(hand.transform.position, transform.position) > Radius && _isInRangeDictionary[hand] == true)
            {
                _isInRangeDictionary[hand] = false;
            }
        }


    }
}
