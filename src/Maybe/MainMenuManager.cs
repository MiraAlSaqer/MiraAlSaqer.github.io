using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource menuMusicSource;

    [Header("UI Panels")]
    public GameObject menuUiPanel;
    public TextMeshProUGUI warningText;

    [Header("Fade Transitions")]
    public Image fadeInImage;
    public float fadeInDuration = 3.0f;
    public Image fadeOutImage;
    public float fadeOutDuration = 2.0f;

    private bool isStarting = false;
    private float initialMusicVolume = 1.0f;

    private void Start()
    {
        initialMusicVolume = menuMusicSource.volume;

        // Initialize screen transition
        fadeInImage.gameObject.SetActive(true);
        fadeInImage.color = new Color(0f, 0f, 0f, 1f);

        fadeOutImage.gameObject.SetActive(true);
        fadeOutImage.color = new Color(0f, 0f, 0f, 0f);

        menuUiPanel.SetActive(false);
        warningText.gameObject.SetActive(false);

        menuMusicSource.Play();

        // Check if returning from gameplay to skip warning text sequence
        bool isReturningFromGame = false;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.kitchenCompleted ||
                GameManager.Instance.garageCompleted ||
                GameManager.Instance.porchCompleted)
            {
                isReturningFromGame = true;
            }
        }

        if (isReturningFromGame)
        {
            StartCoroutine(SkipToMenuFadeIn());
        }
        else
        {
            StartCoroutine(PlayWarning());
        }
    }

    // Handles warning text sequence for first time boot sequence
    private IEnumerator PlayWarning()
    {
        warningText.color = new Color(1f, 1f, 1f, 0f);
        warningText.gameObject.SetActive(true);

        float tIn = 0f;
        while (tIn < 1.5f)
        {
            tIn += Time.deltaTime;
            warningText.color = new Color(1f, 1f, 1f, Mathf.Clamp01(tIn / 1.5f));
            yield return null;
        }
        warningText.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(5.0f);

        float tOut = 0f;
        while (tOut < 1.5f)
        {
            tOut += Time.deltaTime;
            warningText.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1f - (tOut / 1.5f)));
            yield return null;
        }
        warningText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        menuUiPanel.SetActive(true);
        yield return StartCoroutine(FadeInScene());
    }

    private IEnumerator SkipToMenuFadeIn()
    {
        menuUiPanel.SetActive(true);
        yield return StartCoroutine(FadeInScene());
    }

    private IEnumerator FadeInScene()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Clamp01(1f - (elapsed / fadeInDuration));
            fadeInImage.color = new Color(0f, 0f, 0f, currentAlpha);
            yield return null;
        }

        fadeInImage.gameObject.SetActive(false);
    }

    public void OnStartButtonPressed()
    {
        if (isStarting) return;
        isStarting = true;

        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        menuUiPanel.SetActive(false);

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeOutDuration;

            float currentAlpha = Mathf.Clamp01(progress);
            fadeOutImage.color = new Color(0f, 0f, 0f, currentAlpha);

            menuMusicSource.volume = Mathf.Lerp(initialMusicVolume, 0f, progress);

            yield return null;
        }

        fadeOutImage.color = new Color(0f, 0f, 0f, 1f);
        menuMusicSource.volume = 0f;

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Attic");
    }
}