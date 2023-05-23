using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveData : MonoBehaviour
{
    public int x;
    public int y;
    public int ButtonCamera;
    public Button buttonToPress;
    public CanvaManager CanvaManager;

    // Start is called before the first frame update
    void Start()
    {
        SerialManagerScript.WhenReceiveDataCall += Receive;
    }


    private void Receive(string incomingString)
    {
        string[] values = incomingString.Split(',');
        int.TryParse(values[0], out x);
        int.TryParse(values[1], out y);
        int.TryParse(values[2], out ButtonCamera);
        

        if (values[3] == " 6A B5 EA 81\r")
        {
            CanvaManager.SwitchToCanvas3();
            buttonToPress.onClick.Invoke();
        }

        else if (values[3] == " 47 46 70 B4\r")
        {
            CanvaManager.SwitchToCanvas2();
        }

        Debug.Log(values);
    }

}
