using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HallwayManager : MonoBehaviour
{
    [Header("Sections")]
    public GameObject section1;
    public GameObject section2;

    [Header("Audio Sources")]
    public AudioSource primaryAudioSource;
    public AudioSource tableFlashbackAudioSource;
    public AudioSource doorFlashbackAudioSource;

    [Header("Scene Start")]
    public AudioClip initialSceneStartLine;
    public float delayBeforeStartLine = 2.0f;
    public float initialFadeInDuration = 2.0f;

    [Header("Section 1 Clips")]
    public AudioClip section1MemoryDialogue;
    public AudioClip s1Dialogue_Compassionate;
    public AudioClip s1Dialogue_Logical;
    public AudioClip s1Dialogue_Loyalty;

    [Header("Section 2 Clips")]
    public AudioClip section2MemoryDialogue;
    public AudioClip s2Dialogue_Compassionate;
    public AudioClip s2Dialogue_Logical;
    public AudioClip s2Dialogue_Loyalty;

    [Header("Rewind Stuff")]
    public AudioClip playerMaybeSpiralLine;
    public AudioClip rewindSFXClip;

    [Header("Scene Rewind UI")]
    public Image localWhiteFlashImage;
    public Image localGlitchTextureImage;
    public float rewindDuration = 3.0f;
    public float horizontalJitterRange = 45f;

    private int playerTrait = 0;
    private bool trigger1Activated = false;
    private bool trigger2Activated = false;
    private bool trigger3Activated = false;

    private RectTransform glitchRectTransform;
    private Vector2 originalGlitchPosition;

    private IEnumerator Start()
    {
        section1.SetActive(false);
        section2.SetActive(false);

        localWhiteFlashImage.color = new Color(1f, 1f, 1f, 0f);

        glitchRectTransform = localGlitchTextureImage.rectTransform;
        originalGlitchPosition = glitchRectTransform.anchoredPosition;

        Color clearColor = localGlitchTextureImage.color;
        clearColor.a = 0f;
        localGlitchTextureImage.color = clearColor;
        localGlitchTextureImage.gameObject.SetActive(true);

        yield return null;

        if (GameManager.Instance != null)
        {
            playerTrait = GameManager.Instance.personalityTrait;
        }

        StartCoroutine(ExecuteSceneOpeningSequence());
    }

    private IEnumerator ExecuteSceneOpeningSequence()
    {
        if (VRSceneLoader.Instance != null && VRSceneLoader.Instance.fadeImage != null)
        {
            VRSceneLoader.Instance.StopAllCoroutines();
            Image sharedFadeImage = VRSceneLoader.Instance.fadeImage;
            sharedFadeImage.gameObject.SetActive(true);
            sharedFadeImage.color = Color.black;

            float fadeTimer = 0f;
            while (fadeTimer < initialFadeInDuration)
            {
                fadeTimer += Time.deltaTime;
                float progress = fadeTimer / initialFadeInDuration;
                sharedFadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, progress));
                yield return null;
            }

            sharedFadeImage.color = new Color(0f, 0f, 0f, 0f);
        }
        else
        {
            yield return new WaitForSeconds(initialFadeInDuration);
        }

        yield return new WaitForSeconds(delayBeforeStartLine);

        primaryAudioSource.clip = initialSceneStartLine;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(initialSceneStartLine.length);
    }

    public void PlayerEnteredTrigger(int triggerID)
    {
        if (triggerID == 1 && !trigger1Activated)
        {
            trigger1Activated = true;
            StartCoroutine(ExecuteSection1Sequence());
        }
        else if (triggerID == 2 && !trigger2Activated)
        {
            trigger2Activated = true;
            StartCoroutine(ExecuteSection2Sequence());
        }
        else if (triggerID == 3 && !trigger3Activated)
        {
            trigger3Activated = true;
            StartCoroutine(ExecuteSection3RewindSequence());
        }
    }

    private IEnumerator ExecuteSection1Sequence()
    {
        section1.SetActive(true);

        tableFlashbackAudioSource.clip = section1MemoryDialogue;
        tableFlashbackAudioSource.Play();
        yield return new WaitForSeconds(section1MemoryDialogue.length + 0.5f);

        yield return new WaitForSeconds(1.5f);

        AudioClip selfDialogue = PersonalityDialogue(1);

        primaryAudioSource.clip = selfDialogue;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(selfDialogue.length + 1.0f);
    }

    private IEnumerator ExecuteSection2Sequence()
    {
        section2.SetActive(true);

        doorFlashbackAudioSource.clip = section2MemoryDialogue;
        doorFlashbackAudioSource.Play();
        yield return new WaitForSeconds(section2MemoryDialogue.length + 0.5f);

        yield return new WaitForSeconds(1.5f);

        AudioClip selfDialogue = PersonalityDialogue(2);

        primaryAudioSource.clip = selfDialogue;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(selfDialogue.length + 1.0f);
    }

    private IEnumerator ExecuteSection3RewindSequence()
    {
        primaryAudioSource.clip = playerMaybeSpiralLine;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(playerMaybeSpiralLine.length + 0.5f);

        primaryAudioSource.clip = rewindSFXClip;
        primaryAudioSource.Play();

        float timer = 0f;
        Color baseGlitchColor = localGlitchTextureImage.color;

        while (timer < rewindDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / rewindDuration;

            float randomXOffset = Random.Range(-horizontalJitterRange, horizontalJitterRange);
            glitchRectTransform.anchoredPosition = new Vector2(originalGlitchPosition.x + randomXOffset, originalGlitchPosition.y);

            float flashAlpha = (Random.value > 0.25f) ? 1.0f : 0.0f;
            localGlitchTextureImage.color = new Color(baseGlitchColor.r, baseGlitchColor.g, baseGlitchColor.b, flashAlpha);

            localWhiteFlashImage.color = new Color(1f, 1f, 1f, progress);

            yield return null;
        }

        localWhiteFlashImage.color = Color.white;

        glitchRectTransform.anchoredPosition = originalGlitchPosition;
        localGlitchTextureImage.color = new Color(baseGlitchColor.r, baseGlitchColor.g, baseGlitchColor.b, 0f);

        SceneManager.LoadScene("Rewind");
    }

    private AudioClip PersonalityDialogue(int sectionID)
    {
        if (sectionID == 1)
        {
            if (playerTrait == 1) return s1Dialogue_Compassionate;
            if (playerTrait == 2) return s1Dialogue_Logical;
            if (playerTrait == 3) return s1Dialogue_Loyalty;
        }
        else if (sectionID == 2)
        {
            if (playerTrait == 1) return s2Dialogue_Compassionate;
            if (playerTrait == 2) return s2Dialogue_Logical;
            if (playerTrait == 3) return s2Dialogue_Loyalty;
        }
        return null;
    }
}