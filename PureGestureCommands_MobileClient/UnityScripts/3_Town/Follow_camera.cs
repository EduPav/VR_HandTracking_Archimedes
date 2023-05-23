using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_camera : MonoBehaviour
{
    public Transform camera2follow; // The camera hand will follow

    public Vector3 offset; // camera to hand distance

    private Transform[] hands; // Array containing the "Hand" objects

    // Start is called before the first frame update
    void Start()
    {
        // Get the "Hand" objects inside "Hands"
        hands = transform.GetChild(0).GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move each "Hand" object individually
        foreach(Transform hand in hands)
        {
            if(hand != transform.GetChild(0))
            {
                Matrix4x4 cameraTransform = camera2follow.transform.localToWorldMatrix;
                Vector3 newPosition = cameraTransform.MultiplyPoint3x4(hand.localPosition + offset);
                Quaternion newRotation = cameraTransform.rotation;
                hand.SetPositionAndRotation(newPosition, newRotation);
            }
        }
    }
}


