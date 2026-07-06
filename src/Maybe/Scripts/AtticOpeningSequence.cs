using System.Collections;
using UnityEngine;

public class AtticOpeningSequence : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource voiceSource;

    [Header("Voice Clips")]
    public AudioClip audio1Void;
    public AudioClip audio2Revealed;
    public AudioClip audio3Objective;

    [Header("Timing Adjustments")]
    public float delayBeforeFadeIn = 1.0f;

    // Handles all the introductory voiceovers and camera fade timelines
    private IEnumerator Start()
    {
        yield return null;

        // Skip narrative voice lines if returning from a memory layer
        if (GameManager.Instance.kitchenCompleted || GameManager.Instance.garageCompleted || GameManager.Instance.porchCompleted)
        {
            yield return StartCoroutine(FadeInScene());
            yield break;
        }

        yield return new WaitForSeconds(1.5f);
        voiceSource.clip = audio1Void;
        voiceSource.Play();
        yield return new WaitForSeconds(audio1Void.length);

        yield return new WaitForSeconds(delayBeforeFadeIn);

        yield return StartCoroutine(FadeInScene());

        voiceSource.clip = audio2Revealed;
        voiceSource.Play();
        yield return new WaitForSeconds(audio2Revealed.length);

        yield return new WaitForSeconds(2.0f);

        voiceSource.clip = audio3Objective;
        voiceSource.Play();
    }

    private IEnumerator FadeInScene()
    {
        VRSceneLoader.Instance.TriggerManualFadeIn();
        yield return new WaitForSeconds(VRSceneLoader.Instance.fadeDuration);
    }
}