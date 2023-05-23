using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }
    
    public void LoadScene(string escene){
        StartCoroutine(LoadAsync(escene));
    }

    IEnumerator LoadAsync(string escene){
        AsyncOperation operation = SceneManager.LoadSceneAsync(escene);

        loadingScreen.SetActive(true);

        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            
            yield return null;
        }
    }
}
