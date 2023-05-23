using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputController : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public static string ipServer;

    public void SetVariable()
    {
        ipServer = inputField.text;
    }

}
