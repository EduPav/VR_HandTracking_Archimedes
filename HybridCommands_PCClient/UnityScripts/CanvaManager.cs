using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvaManager : MonoBehaviour
{
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject canvas3;

    // Start is called before the first frame update
    void Start(){
        canvas1.SetActive(true);
        canvas2.SetActive(false);
        canvas3.SetActive(false);
    }

    public void SwitchToCanvas2() {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        canvas3.SetActive(false);
    }

    public void SwitchToCanvas3() {
        canvas1.SetActive(false);
        canvas2.SetActive(false);
        canvas3.SetActive(true);
    }
}


