using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Umbrella : MonoBehaviour
{
    [Header("Umbrella Meshes")]
    public GameObject closedUmbrellaMesh;
    public GameObject openUmbrellaMesh;

    [Header("UI Prompt")]
    public GameObject triggerPromptCanvas;

    [Header("SFX Stuff")]
    public AudioSource umbrellaAudioSource;
    public AudioClip umbrellaOpenSFX;

    private XRGrabInteractable grabInteractable;
    private bool isOpened = false;
    private bool isBeingHeld = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        closedUmbrellaMesh.SetActive(true);
        openUmbrellaMesh.SetActive(false);
        triggerPromptCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnPickedUp);
        grabInteractable.selectExited.AddListener(OnDropped);
        grabInteractable.activated.AddListener(OnTriggerPulled);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnPickedUp);
        grabInteractable.selectExited.RemoveListener(OnDropped);
        grabInteractable.activated.RemoveListener(OnTriggerPulled);
    }

    private void OnPickedUp(SelectEnterEventArgs args)
    {
        isBeingHeld = true;

        if (!isOpened)
        {
            triggerPromptCanvas.SetActive(true);
        }
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        isBeingHeld = false;
        triggerPromptCanvas.SetActive(false);
    }

    private void OnTriggerPulled(ActivateEventArgs args)
    {
        if (isOpened || !isBeingHeld) return;

        isOpened = true;

        triggerPromptCanvas.SetActive(false);
        closedUmbrellaMesh.SetActive(false);
        openUmbrellaMesh.SetActive(true);

        umbrellaAudioSource.PlayOneShot(umbrellaOpenSFX);
        PorchManager.Instance.ConfirmUmbrellaDeployment();
        
    }
}