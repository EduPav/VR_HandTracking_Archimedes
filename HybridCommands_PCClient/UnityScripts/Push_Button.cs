using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Push_Button : MonoBehaviour
{
    public Button buttonToPress;
    int i = 0;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Point (8)")
        {
        buttonToPress.onClick.Invoke();
        }
    }
}

