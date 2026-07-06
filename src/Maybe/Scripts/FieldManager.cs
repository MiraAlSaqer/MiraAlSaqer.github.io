using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FieldSceneManager : MonoBehaviour
{
    [Header("Lock Movement")]
    public GameObject locomotion;
    public CharacterController characterController;

    [Header("Personality Objects")]
    public GameObject spoonObject;
    public GameObject wrenchObject;
    public GameObject umbrellaObject;

    [Header("Audio")]
    public AudioSource monologueAudioSource;
    public AudioSource backgroundMusicSource;
    [Range(0f, 1f)] public float musicInitialVolume = 0.2f;
    [Range(0f, 1f)] public float musicSwellVolume = 0.6f;

    [Header("UI Canvas and Transitions")]
    public Image whiteFadeImage;
    public Image blackFadeImage;
    public TextMeshProUGUI finalQuoteTextMesh;
    public TextMeshProUGUI finalPathTextMesh;

    private string pathStringName = "Logical";

    private void Start()
    {
        locomotion.SetActive(false);
        characterController.enabled = false;

        int trait = 0;

        if (GameManager.Instance != null)
        {
            trait = GameManager.Instance.personalityTrait;
        }

        SetupPersonalityPath(trait);

        whiteFadeImage.gameObject.SetActive(true);
        whiteFadeImage.color = new Color(1f, 1f, 1f, 1f);

        blackFadeImage.gameObject.SetActive(true);
        blackFadeImage.color = new Color(0f, 0f, 0f, 0f);

        finalQuoteTextMesh.gameObject.SetActive(false);
        finalPathTextMesh.gameObject.SetActive(false);

        StartCoroutine(ExecuteFinalTimelineSequence());
    }

    private void SetupPersonalityPath(int trait)
    {
        spoonObject.SetActive(false);
        wrenchObject.SetActive(false);
        umbrellaObject.SetActive(false);

        if (trait == 1)
        {
            pathStringName = "Compassionate";
            spoonObject.SetActive(true);
        }
        else if (trait == 2)
        {
            pathStringName = "Logical";
            wrenchObject.SetActive(true);
        }
        else if (trait == 3)
        {
            pathStringName = "Loyal";
            umbrellaObject.SetActive(true);
        }
    }

    private IEnumerator ExecuteFinalTimelineSequence()
    {
        backgroundMusicSource.volume = musicInitialVolume;

        float elapsed = 0f;
        while (elapsed < 2.0f)
        {
            elapsed += Time.deltaTime;
            whiteFadeImage.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.0f - (elapsed / 2.0f)));
            yield return null;
        }
        whiteFadeImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(3.0f);

        if (monologueAudioSource.clip != null)
        {
            monologueAudioSource.Play();
            yield return new WaitForSeconds(monologueAudioSource.clip.length);
        }
        else
        {
            yield return new WaitForSeconds(5.0f);
        }

        float swellElapsed = 0f;
        float currentVol = backgroundMusicSource.volume;
        while (swellElapsed < 2.0f)
        {
            swellElapsed += Time.deltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(currentVol, musicSwellVolume, swellElapsed / 2.0f);
            yield return null;
        }

        yield return new WaitForSeconds(8.0f);

        float fadeOutElapsed = 0f;
        while (fadeOutElapsed < 2.0f)
        {
            fadeOutElapsed += Time.deltaTime;
            blackFadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fadeOutElapsed / 2.0f));
            yield return null;
        }
        blackFadeImage.color = new Color(0f, 0f, 0f, 1f);

        yield return new WaitForSeconds(1.0f);

        finalQuoteTextMesh.color = new Color(1f, 1f, 1f, 0f);
        finalQuoteTextMesh.gameObject.SetActive(true);

        float qFadeIn = 0f;
        while (qFadeIn < 1.0f)
        {
            qFadeIn += Time.deltaTime;
            finalQuoteTextMesh.color = new Color(1f, 1f, 1f, Mathf.Clamp01(qFadeIn));
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);

        float qFadeOut = 0f;
        while (qFadeOut < 1.0f)
        {
            qFadeOut += Time.deltaTime;
            finalQuoteTextMesh.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.0f - qFadeOut));
            yield return null;
        }
        finalQuoteTextMesh.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        finalPathTextMesh.text = $"{pathStringName.ToUpper()} PATH COMPLETED";
        finalPathTextMesh.color = new Color(1f, 1f, 1f, 0f);
        finalPathTextMesh.gameObject.SetActive(true);

        float pFadeIn = 0f;
        while (pFadeIn < 1.0f)
        {
            pFadeIn += Time.deltaTime;
            finalPathTextMesh.color = new Color(1f, 1f, 1f, Mathf.Clamp01(pFadeIn));
            yield return null;
        }

        yield return new WaitForSeconds(5.0f);

        float dynamicOutElapsed = 0f;
        float musicVolBeforeFade = backgroundMusicSource.volume;

        while (dynamicOutElapsed < 3.0f)
        {
            dynamicOutElapsed += Time.deltaTime;
            float normalizedProgress = dynamicOutElapsed / 3.0f;

            finalPathTextMesh.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.0f - normalizedProgress));
            backgroundMusicSource.volume = Mathf.Lerp(musicVolBeforeFade, 0f, normalizedProgress);

            yield return null;
        }

        finalPathTextMesh.gameObject.SetActive(false);
        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = 0f;

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Main Menu");
    }
}