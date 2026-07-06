using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRSceneLoader : MonoBehaviour
{
    public static VRSceneLoader Instance { get; private set; }

    [Header("Canvas Prefab")]
    public GameObject fadeCanvasPrefab;

    [Header("Fade Speed")]
    public float fadeDuration = 2f;

    public Image fadeImage;
    private Canvas activeFadeCanvas;
    private bool isFirstTimeBoot = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        SetupFadeCanvas();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetupFadeCanvas()
    {
        GameObject canvasInstance = Instantiate(fadeCanvasPrefab);
        DontDestroyOnLoad(canvasInstance);

        activeFadeCanvas = canvasInstance.GetComponent<Canvas>();
        fadeImage = canvasInstance.GetComponentInChildren<Image>();

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (activeFadeCanvas == null)
        {
            SetupFadeCanvas();
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        if (scene.name == "Attic" && isFirstTimeBoot)
        {
            isFirstTimeBoot = false;
            return;
        }

        StopAllCoroutines();
        StartCoroutine(StartFade(1f, 0f));
    }

    public void TriggerManualFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(StartFade(1f, 0f));
    }

    public void SwitchScene(string sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(LoadSceneSequence(sceneName));
    }

    private IEnumerator LoadSceneSequence(string sceneName)
    {
        yield return StartCoroutine(StartFade(0f, 1f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator StartFade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color originalColor = Color.black;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

            yield return null;
        }

        fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    public IEnumerator ManualFadeToBlack()
    {
        StopAllCoroutines();
        yield return StartCoroutine(StartFade(0f, 1f));
    }
}