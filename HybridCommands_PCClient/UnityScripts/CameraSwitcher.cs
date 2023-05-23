using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera secondCamera;
    public Camera thirdCamera;
    public ReceiveData receiveData;

    private bool isMainCameraActive = true;
    private int cameraCount = 1;

    void Start()
    {
        
        secondCamera.enabled = false;
        thirdCamera.enabled = false;
        mainCamera.enabled = true;
        
    }

    void Update()
    {
//        int ChangeCamera = receiveData.ButtonCamera;

        if (Input.GetKeyDown(KeyCode.Space) )//|| ChangeCamera == 0)
        {
            cameraCount++;

            if (cameraCount > 3)
            {
                cameraCount = 1;
            }

            switch (cameraCount)
            {
                case 1:
                    mainCamera.enabled = true;
                    secondCamera.enabled = false;
                    thirdCamera.enabled = false;
                    break;

                case 2:
                    mainCamera.enabled = false;
                    secondCamera.enabled = true;
                    thirdCamera.enabled = false;
                    break;

                case 3:
                    mainCamera.enabled = false;
                    secondCamera.enabled = false;
                    thirdCamera.enabled = true;
                    break;
            }
        }
    }
}
