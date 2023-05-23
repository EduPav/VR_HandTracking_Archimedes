using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;


    void Start()
    {
        mainCamera.enabled = true; 
    }
}