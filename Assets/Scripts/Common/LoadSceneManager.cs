using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    public string currentSceneName;

    private string nextSceneName;

    private void Start()
    {
        this.currentSceneName = Env.MENU_SCENE;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        this.currentSceneName = sceneName;
    }

    public AsyncOperation LoadSceneAsync(string sceneName)
    {
        this.nextSceneName = sceneName;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += ChangeCurrentScene;
        return operation;
    }

    private void ChangeCurrentScene(AsyncOperation operation)
    {
        this.currentSceneName = this.nextSceneName;
    }
}
