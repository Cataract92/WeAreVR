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
        CUBE,
        BOMB
    }

    public ToolType Type;
    public GameObject ModelPrefab;
    public float Radius = 0.1f;

    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
    public Vector3 BelowCameraVector = new Vector3(0, -0.6f, 0);

    public GameObject MainObjectPrefab;

    private Dictionary<CustomHand,bool> _isInRangeDictionary = new Dictionary<CustomHand, bool>();
    private Camera _camera;
    private Transform _playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var hand in FindObjectsOfType<CustomHand>())
        {
            _isInRangeDictionary.Add(hand,false);
        }

        _camera = GameObject.FindObjectOfType<Camera>();
        _playerTransform = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (Type)
        {
            case ToolType.PLUNGER:
            {
                var eulers = _camera.transform.localRotation.eulerAngles;
                transform.position = _camera.transform.position + BelowCameraVector * _playerTransform.localScale.x + Quaternion.Euler(0,eulers.y,0) * new Vector3(1.2f,0, 0.3f) * 0.2f * _playerTransform.localScale.x;
                break;
            }
            case ToolType.ERASER:
            {
                var eulers = _camera.transform.localRotation.eulerAngles;
                transform.position = _camera.transform.position + BelowCameraVector * _playerTransform.localScale.x - Quaternion.Euler(0, eulers.y, 0) * new Vector3(1.2f, 0, 0.3f) * 0.2f * _playerTransform.localScale.x;
                break;
            }
            case ToolType.CUBE:
            {
                var eulers = _camera.transform.localRotation.eulerAngles;
                transform.position = _camera.transform.position + BelowCameraVector * _playerTransform.localScale.x + Quaternion.Euler(0, eulers.y, 0) * new Vector3(0.5f, 0, 0.5f) * 0.2f * _playerTransform.localScale.x;
                break;
            }
            case ToolType.BOMB:
            {
                var eulers = _camera.transform.localRotation.eulerAngles;
                transform.position = _camera.transform.position + BelowCameraVector * _playerTransform.localScale.x - Quaternion.Euler(0, eulers.y, 0) * new Vector3(0.5f, 0, -0.3f) * 0.2f * _playerTransform.localScale.x;
                break;
            }
        }
       

        foreach (var hand in _isInRangeDictionary.Keys)
        {
            if (Vector3.Distance(hand.transform.position, transform.position) < Radius * _playerTransform.localScale.x && _isInRangeDictionary[hand] == false)
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

                    GameObject tool = null;

                    if (Type == ToolType.CUBE)
                    {
                        tool = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        tool.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    } else
                    {
                        tool = Instantiate(ModelPrefab);
                        tool.transform.localScale = new Vector3(1f, 1f, 1f);
                    }

                    tool.AddComponent<DummyTool>();
                    tool.transform.parent = hand.transform;
                    tool.transform.localPosition = PositionOffset;
                    tool.transform.localRotation = Quaternion.Euler(RotationOffset);

                    tool.transform.localScale = new Vector3(tool.transform.localScale.x * FindObjectOfType<CustomPlayer>().transform.localScale.x, tool.transform.localScale.y * FindObjectOfType<CustomPlayer>().transform.localScale.y, tool.transform.localScale.z * FindObjectOfType<CustomPlayer>().transform.localScale.z);

                    hand.HideController();

                    hand.CurrentToolType = Type;
                }
                _isInRangeDictionary[hand] = true;
            }
            if (Vector3.Distance(hand.transform.position, transform.position) > Radius * _playerTransform.localScale.x && _isInRangeDictionary[hand] == true)
            {
                _isInRangeDictionary[hand] = false;
            }
        }


    }
}
