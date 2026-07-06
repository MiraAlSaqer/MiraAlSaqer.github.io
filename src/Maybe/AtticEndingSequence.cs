using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AtticEndingSequence : MonoBehaviour
{
    [Header("Audio Component")]
    public AudioSource voiceSource;

    [Header("Final Personality Voice Lines")]
    public AudioClip endingPath1Compassionate;
    public AudioClip endingPath2Logic;
    public AudioClip endingPath3Loyalty;

    [Header("Narrative Twist Lines")]
    public AudioClip badMemoriesTwistClip;
    public AudioClip pitchBlackVoiceClip;

    private bool endingSequenceStarted = false;

    private IEnumerator Start()
    {
        yield return null;

        // Evaluates if all narrative memory branches are completely checked off
        if (GameManager.Instance.kitchenCompleted &&
            GameManager.Instance.garageCompleted &&
            GameManager.Instance.porchCompleted)
        {
            StartFinalAtticEnding();
        }
    }

    private void StartFinalAtticEnding()
    {
        if (endingSequenceStarted) return;
        endingSequenceStarted = true;

        StartCoroutine(ExecuteEndingSequence());
    }

    // Coordinates the branching voice overs and eventual transition timelines
    private IEnumerator ExecuteEndingSequence()
    {
        yield return new WaitForSeconds(3.0f);

        int lockedTrait = GameManager.Instance.personalityTrait;
        AudioClip finalVoiceLine = null;

        if (lockedTrait == 1) finalVoiceLine = endingPath1Compassionate;
        else if (lockedTrait == 2) finalVoiceLine = endingPath2Logic;
        else if (lockedTrait == 3) finalVoiceLine = endingPath3Loyalty;

        voiceSource.clip = finalVoiceLine;
        voiceSource.Play();
        yield return new WaitForSeconds(finalVoiceLine.length);

        yield return new WaitForSeconds(2.0f);

        voiceSource.clip = badMemoriesTwistClip;
        voiceSource.Play();
        yield return new WaitForSeconds(badMemoriesTwistClip.length);

        yield return StartCoroutine(FadeOutTransition());
    }

    // Handles screen fading out to the hallway scene
    private IEnumerator FadeOutTransition()
    {
        yield return StartCoroutine(VRSceneLoader.Instance.ManualFadeToBlack());

        voiceSource.clip = pitchBlackVoiceClip;
        voiceSource.Play();
        yield return new WaitForSeconds(pitchBlackVoiceClip.length + 1.0f);

        VRSceneLoader.Instance.fadeImage.gameObject.SetActive(true);
        VRSceneLoader.Instance.fadeImage.color = new Color(0f, 0f, 0f, 1f);

        SceneManager.LoadScene("Hallway");
    }
}