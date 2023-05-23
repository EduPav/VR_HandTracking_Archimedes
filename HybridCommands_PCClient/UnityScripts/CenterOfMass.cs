using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Vector3 newPosition;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = newPosition;
    }
}
