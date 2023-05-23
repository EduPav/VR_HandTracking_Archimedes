using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_camera : MonoBehaviour
{
    public Transform camera; // Camera hand will follow

    public Vector3 offset; // Distance from hand to camera

    private Transform[] hands; // Array of hand objects

    // Start is called before the first frame update
    void Start()
    {
        // Get "Hand" objects from inside "hands" 
        hands = transform.GetChild(0).GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move each hand object individually
        foreach(Transform hand in hands)
        {
            if(hand != transform.GetChild(0))
            {
                Matrix4x4 cameraTransform = camera.transform.localToWorldMatrix;
                Vector3 newPosition = cameraTransform.MultiplyPoint3x4(hand.localPosition + offset);
                Quaternion newRotation = cameraTransform.rotation;
                hand.SetPositionAndRotation(newPosition, newRotation);
            }
        }
    }
}


