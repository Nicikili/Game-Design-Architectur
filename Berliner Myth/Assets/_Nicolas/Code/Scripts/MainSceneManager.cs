using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public string targetScenenName = "Game";
    public FullScreenPassRendererFeature fullscreenPassRendererFeatrue;

    private bool isFullscreenActive = false;


    void Awake()
    {
        if (SceneManager.GetActiveScene().name != targetScenenName)
        {
            fullscreenPassRendererFeatrue.SetActive(false);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetScenenName && !isFullscreenActive)
        {
            fullscreenPassRendererFeatrue.SetActive(true);
            isFullscreenActive = true;

            Debug.Log("shader on");
        }
        else if(scene.name != targetScenenName && isFullscreenActive)
        {
            fullscreenPassRendererFeatrue.SetActive(false);
            isFullscreenActive = false;

            Debug.Log("shader off");
        }
    }
}