using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
   
    public static SceneTransitionManager Instance { get; private set; }
    public string CurrentSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateCurrentScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            MoveManager.Instance.OnSceneIn();//记录位置
            //MoveManager.Instance.OnSceneOut();
            /*Scene currentScene = SceneManager.GetActiveScene();
            SceneTransitionManager.Instance.GoToScene(currentScene.name);*/
            GoToScene("New Scene");
           
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            MoveManager.Instance.OnSceneIn();//记录位置
            //MoveManager.Instance.OnSceneOut();
            /*Scene currentScene = SceneManager.GetActiveScene();
            SceneTransitionManager.Instance.GoToScene(currentScene.name);*/
            GoToScene("Climb_Test 1");

        }



    }
    private void UpdateCurrentScene()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Current Scene: {CurrentSceneName}");
    }
    // 同步场景加载
    public void GoToScene(string sceneIndex)
    {
      
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    private IEnumerator GoToSceneRoutine(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        yield return null;
        UpdateCurrentScene();

    }

    // 异步场景加载（带进度控制）
    public void GoToSceneAsync(string sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    private IEnumerator GoToSceneAsyncRoutine(string sceneIndex)
    {
     

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0;
     

        operation.allowSceneActivation = true;

        // 等待场景完全激活
        while (!operation.isDone)
        {
            yield return null;
        }

        UpdateCurrentScene();
    }
}