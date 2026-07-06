using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorSensor : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource doorAudioSource;
    public AudioClip doorCloseSFX;

    private XRGrabInteractable grabInteractable;
    private Rigidbody doorRigidbody;
    private bool isClosed = false;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        doorRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isClosed) return;
        if (other.CompareTag("MainCamera") || other.name.Contains("Hand")) return;

        if (other.gameObject.name == "DoorLockTargetZone")
        {
            LockAndSlamDoor();
        }
    }

    private void LockAndSlamDoor()
    {
        isClosed = true;

        doorAudioSource.clip = doorCloseSFX;
        doorAudioSource.Play();

        grabInteractable.enabled = false;

        doorRigidbody.linearVelocity = Vector3.zero;
        doorRigidbody.angularVelocity = Vector3.zero;
        doorRigidbody.isKinematic = true;

        RewindManager manager = FindFirstObjectByType<RewindManager>();
        manager.BedroomDoorClosedSuccessfully();
    }
}