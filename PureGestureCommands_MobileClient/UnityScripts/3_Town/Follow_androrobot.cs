using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_androrobot : MonoBehaviour
{
    public Transform androRobotTransform;
    public Transform[] handTransforms;
    private Vector3[] handOffsets;

    // Start is called before the first frame update
    void Start()
    {
        // Saves the relative position of each hand with respect to androrobot
        handOffsets = new Vector3[handTransforms.Length];
        for (int i = 0; i < handTransforms.Length; i++)
        {
            handOffsets[i] = handTransforms[i].position - androRobotTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Updates each hand's position based on androrobot position and its relative position
        for (int i = 0; i < handTransforms.Length; i++)
        {
            handTransforms[i].position = androRobotTransform.position + handOffsets[i];
        }
    }
}

