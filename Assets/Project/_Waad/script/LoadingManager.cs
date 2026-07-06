using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text loadingText;

    [Header("Loading")]
    [SerializeField] private string nextSceneName;   // اسم المشهد اللي بنروح له
    [SerializeField] private float fakeLoadDelay = 2f;

    void Start()
    {
        StartCoroutine(LoadMainSceneAsync());
    }

    IEnumerator LoadMainSceneAsync()
    {
        yield return new WaitForSeconds(fakeLoadDelay);

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            progressBar.value = progress;
            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100) + "%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}