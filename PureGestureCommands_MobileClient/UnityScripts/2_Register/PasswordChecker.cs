using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PasswordChecker : MonoBehaviour
{
    public Button button;
    public TMP_InputField inputField;
    public string nextScene;
    public string PersonalPassword;

    void Start()
    {
        button.onClick.AddListener(ButtonClicked);
    }

    void ButtonClicked()
    {
        if (inputField.text == PersonalPassword)
        {
            StartCoroutine(ChangeButtonColor(Color.green));
        }
        else
        {
            StartCoroutine(ChangeButtonColor(Color.red));
        }
    }
    public void LoadScene(string scene){
        SceneManager.LoadSceneAsync(scene);
    }

    IEnumerator ChangeButtonColor(Color color)
    {
        button.colors=new ColorBlock{selectedColor=color, colorMultiplier=1f, fadeDuration=0.1f};
        yield return new WaitForSeconds(1);
        if (color == Color.green)
        {
            LoadScene(nextScene);
        }
        button.colors = ColorBlock.defaultColorBlock;
    }
}


