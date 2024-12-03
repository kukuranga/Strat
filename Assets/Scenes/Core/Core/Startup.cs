using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Startup : MonoBehaviour
{
    public bool EnableMultiTouch = false;
    public SceneReference StartUpScene;

    void Start() {
        Input.multiTouchEnabled = EnableMultiTouch;

        Scene scene = SceneManager.GetSceneByPath(StartUpScene);
        if (scene != null && scene.isLoaded == false) {
            SceneLoader.Instance.LoadScene(StartUpScene);
            //SceneManager.LoadScene(StartUpScene, LoadSceneMode.Additive);
        }
    }
}
