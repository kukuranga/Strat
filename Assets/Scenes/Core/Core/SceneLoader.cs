using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
	[System.Serializable]
	public class SceneLoadedEvent : UnityEvent<List<string>> { }
	[HideInInspector] public SceneLoadedEvent onSceneLoadedEvent = new SceneLoadedEvent();

	public float delayTime = 1.0f;
	public MenuClassifier loadingMenuClassifier;
	[SerializeField] Animator loadingAnimator;
	[SerializeField] GameObject loadingScreen;
 
	private List<string> loadedScenes = new List<string>();

	// When loading just add a flag for persistence. If true don't add to the loadedScenes
	// Only remove the scenes when you unload

	public void LoadScene(string scene, bool showLoadingScreen = true)
	{
		StartCoroutine(loadScene(scene, showLoadingScreen, true));
	}

	public void LoadSceneAsync(string scene, bool showLoadingScreen = true)
	{
		StartCoroutine(loadSceneAsync(scene, showLoadingScreen, true));
	}
	IEnumerator loadSceneAsync(string scene, bool showLoadingScreen, bool raiseEvent, float scenes = 1f)
	{
		if (SceneManager.GetSceneByPath(scene).isLoaded == false)
		{
			if (showLoadingScreen)
			{
				MenuManagerCore.Instance.ShowMenu(loadingMenuClassifier);
			}

			AsyncOperation sync;

			Application.backgroundLoadingPriority = ThreadPriority.Low;

			sync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			while (sync.isDone == false) { yield return null; }

			Application.backgroundLoadingPriority = ThreadPriority.Normal;

			if (showLoadingScreen)
			{
				MenuManagerCore.Instance.HideMenu(loadingMenuClassifier);
			}
		}

		if (raiseEvent)
		{
			loadedScenes.Add(scene);
			onSceneLoadedEvent.Invoke(loadedScenes);
		}
	}


	public void LoadScenes(List<string> scenes, bool showLoadingScreen = true)
	{
		StartCoroutine(loadScenes(scenes, showLoadingScreen));
	}

	IEnumerator loadScenes(List<string> scenes, bool showLoadingScreen)
	{
		if (showLoadingScreen)
		{
			MenuManagerCore.Instance.ShowMenu(loadingMenuClassifier);
			loadingAnimator.speed = 1f / scenes.Count;
		}

		foreach (string scene in scenes)
		{
			yield return StartCoroutine(loadScene(scene, false, false, scenes.Count));
		}

		if (showLoadingScreen)
		{
			MenuManagerCore.Instance.HideMenu(loadingMenuClassifier);
			loadingAnimator.speed = 1f;
		}

		loadedScenes.AddRange(scenes);
		onSceneLoadedEvent.Invoke(loadedScenes);
	}

	IEnumerator loadScene(string scene, bool showLoadingScreen, bool raiseEvent, float scenes = 1f)
	{
		if (SceneManager.GetSceneByPath(scene).isLoaded == false)
		{
			if (showLoadingScreen)
			{
				MenuManagerCore.Instance.ShowMenu(loadingMenuClassifier);
			}

			yield return new WaitForSeconds(delayTime / scenes);

			AsyncOperation sync;

			Application.backgroundLoadingPriority = ThreadPriority.Low;

			sync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
			while (sync.isDone == false) { yield return null; }

			Application.backgroundLoadingPriority = ThreadPriority.Normal;

			yield return new WaitForSeconds(delayTime / scenes);

			if (showLoadingScreen)
			{
				MenuManagerCore.Instance.HideMenu(loadingMenuClassifier);
			}
		}

		if (raiseEvent)
		{
			loadedScenes.Add(scene);
			onSceneLoadedEvent.Invoke(loadedScenes);
		}
	}

	// 4 Methods:
	//	- Unload single scene
	//	- Unload list of scenes
	//		- Support to unload multiple (Coroutine)
	//	- Actual Unload of scenes.

	public void UnloadScene(string scene)
	{
		StartCoroutine(unloadScene(scene));
	}

	public void UnloadScenes(List<string> scenes)
	{
		StartCoroutine(unloadScenes(scenes));
	}

    public void UnloadAllScenes() {
        List<string> allScenes = new List<string>();
        allScenes.AddRange(loadedScenes);
        StartCoroutine(unloadScenes(allScenes));
    }

	IEnumerator unloadScenes(List<string> scenes)
	{
		foreach(string scene in scenes)
		{
			yield return StartCoroutine(unloadScene(scene));
		}
	}

	IEnumerator unloadScene(string scene)
	{
		AsyncOperation sync = null;

		try
		{
			sync = SceneManager.UnloadSceneAsync(scene);	
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}

		if (sync != null)
		{
			while(sync.isDone == false) { yield return null; }
		}

		sync = Resources.UnloadUnusedAssets();
		while(sync.isDone == false) { yield return null; }

		// Remove scene from loadedScenes
		//loadedScenes.Remove(scene);
  //      Debug.Log(scene);
  //      foreach (var k in loadedScenes) {
  //          Debug.Log(k);
  //      }
    }

	public List<string> GetLoadedScenes() {
		return loadedScenes;
    }

	public GameObject GetLoadingScreen() {
		return loadingScreen;
    }
}
