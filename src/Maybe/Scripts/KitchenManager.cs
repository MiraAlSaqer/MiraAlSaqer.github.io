using System.Collections;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource motherVoiceSource;
    public AudioSource ambientMusicSource;
    public AudioSource stirSFXSource;

    [Header("Voice Clips")]
    public AudioClip motherIntroduction;
    public AudioClip motherThankYou;

    [Header("Stirring Interaction")]
    public float requiredStirTime = 8.0f;
    private float currentStirTimer = 0.0f;
    private bool isSpoonInPot = false;
    private bool sequenceCompleted = false;

    private void Start()
    {
        stirSFXSource.Stop();
        StartCoroutine(KitchenOpeningSequence());
    }

    private IEnumerator KitchenOpeningSequence()
    {
        yield return new WaitForSeconds(2.0f);

        motherVoiceSource.clip = motherIntroduction;
        motherVoiceSource.Play();
    }

    // Handles the sound effect that plays when the spoon is in the pot and its pausing when taken out
    public void SetSpoonInPot(bool inPot)
    {
        if (sequenceCompleted) return;

        isSpoonInPot = inPot;

        if (isSpoonInPot)
        {
            if (!stirSFXSource.isPlaying) stirSFXSource.Play();
        }
        else
        {
            stirSFXSource.Stop();
        }
    }

    private void Update()
    {
        if (isSpoonInPot && !sequenceCompleted)
        {
            currentStirTimer += Time.deltaTime;

            if (currentStirTimer >= requiredStirTime)
            {
                StartCoroutine(CompleteKitchenMemory());
            }
        }
    }

    // Sends to the game manager that the memory is completed before initiating the screen fade
    private IEnumerator CompleteKitchenMemory()
    {
        sequenceCompleted = true;
        isSpoonInPot = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterItemInteraction("WoodenSpoon");
        }

        stirSFXSource.Stop();

        motherVoiceSource.clip = motherThankYou;
        motherVoiceSource.Play();
        yield return new WaitForSeconds(motherThankYou.length + 1.0f);

        float startVolume = ambientMusicSource.volume;
        while (ambientMusicSource.volume > 0)
        {
            ambientMusicSource.volume -= startVolume * Time.deltaTime;
            yield return null;
        }

        VRSceneLoader.Instance.SwitchScene("Attic");
    }
}