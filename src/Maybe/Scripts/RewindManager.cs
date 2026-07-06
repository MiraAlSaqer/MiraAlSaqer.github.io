using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewindManager : MonoBehaviour
{
    [Header("Sections")]
    public GameObject section1;
    public GameObject section2;
    public GameObject section3;

    [Header("Audio Sources")]
    public AudioSource primaryAudioSource;
    public AudioSource tableReconciliationSource;
    public AudioSource doorReconciliationSource;

    [Header("Table Clips")]
    public AudioClip playerTableApproachLine;
    public AudioClip tableFixedReconciliationMemory;

    [Header("Door Clips")]
    public AudioClip playerDoorApproachLine;
    public AudioClip doorFixedReconciliationMemory;
    public AudioClip playerDoorRegretLine;

    [Header("Chair Clips")]
    public AudioClip chairRevealedMonologue;
    public AudioClip chairApproachVoiceLine;

    [Header("Chair UI Prompt")]
    public GameObject sitPromptCanvas;

    private int currentStep = 0;
    private bool trigger1Activated = false;
    private bool trigger2Activated = false;
    private bool chairTriggerActivated = false;
    private bool doorMonologueFinished = false;

    private void Start()
    {
        if (VRSceneLoader.Instance != null && VRSceneLoader.Instance.fadeImage != null)
        {
            VRSceneLoader.Instance.StopAllCoroutines();
            VRSceneLoader.Instance.fadeImage.gameObject.SetActive(false);
        }

        section3.SetActive(false);
        sitPromptCanvas.SetActive(false);

        currentStep = 1;
    }

    public void PlayerEnteredTrigger(int triggerID)
    {
        if (triggerID == 1 && !trigger1Activated && currentStep == 1)
        {
            trigger1Activated = true;
            StartCoroutine(ExecuteTableApproachSequence());
        }
        else if (triggerID == 2 && !trigger2Activated && currentStep >= 2)
        {
            trigger2Activated = true;
            StartCoroutine(ExecuteDoorApproachSequence());
        }
        else if (triggerID == 3 && !chairTriggerActivated && doorMonologueFinished)
        {
            chairTriggerActivated = true;
            StartCoroutine(ExecuteChairApproachSequence());
        }
    }

    private IEnumerator ExecuteTableApproachSequence()
    {
        primaryAudioSource.clip = playerTableApproachLine;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(playerTableApproachLine.length);

        currentStep = 2;
    }

    public void PlacedPlateInSocket()
    {
        currentStep = 3;
        StartCoroutine(ExecuteTableReconciliationMemory());
    }

    private IEnumerator ExecuteTableReconciliationMemory()
    {
        tableReconciliationSource.clip = tableFixedReconciliationMemory;
        tableReconciliationSource.Play();
        yield return new WaitForSeconds(tableFixedReconciliationMemory.length + 0.5f);
    }

    private IEnumerator ExecuteDoorApproachSequence()
    {
        currentStep = 4;

        primaryAudioSource.clip = playerDoorApproachLine;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(playerDoorApproachLine.length + 0.5f);

        doorReconciliationSource.clip = doorFixedReconciliationMemory;
        doorReconciliationSource.Play();
        yield return new WaitForSeconds(doorFixedReconciliationMemory.length);

        yield return new WaitForSeconds(1.0f);

        primaryAudioSource.clip = playerDoorRegretLine;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(playerDoorRegretLine.length);

        currentStep = 5;
    }

    public void BedroomDoorClosedSuccessfully()
    {
        currentStep = 6;
        StartCoroutine(ExecuteSection3RevealSequence());
    }

    private IEnumerator ExecuteSection3RevealSequence()
    {
        yield return new WaitForSeconds(1.0f);

        section3.SetActive(true);

        primaryAudioSource.clip = chairRevealedMonologue;
        primaryAudioSource.Play();
        yield return new WaitForSeconds(chairRevealedMonologue.length);

        doorMonologueFinished = true;
    }

    private IEnumerator ExecuteChairApproachSequence()
    {
        sitPromptCanvas.SetActive(true);

        primaryAudioSource.clip = chairApproachVoiceLine;
        primaryAudioSource.Play();
        yield return null;
    }
}