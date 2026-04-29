using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    protected int nowSceneIndex = GameManager.currentSceneIndex;

    public void LoadScene(int _sceneIndex)
    {
        StartCoroutine(LoadScenes(_sceneIndex));
    }

    public IEnumerator LoadScenes(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        GameManager.currentSceneIndex = sceneIndex;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}