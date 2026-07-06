using System.Collections;
using UnityEngine;

public class AtticInteractableObject : MonoBehaviour
{
    [Header("Item Configuration")]
    public string itemName;

    [Header("Audio Settings")]
    public AudioSource localAudioSource;
    public AudioClip immediateReflectionClip;

    private bool hasBeenClicked = false;

    // Evaluates memory records on load to lock out previously finished interactions
    private void Start()
    {
        bool isAlreadyCompleted = false;

        if (itemName == "WoodenSpoon") isAlreadyCompleted = GameManager.Instance.kitchenCompleted;
        else if (itemName == "Wrench") isAlreadyCompleted = GameManager.Instance.garageCompleted;
        else if (itemName == "Umbrella") isAlreadyCompleted = GameManager.Instance.porchCompleted;

        if (isAlreadyCompleted)
        {
            hasBeenClicked = true;

            if (TryGetComponent<Collider>(out Collider col))
            {
                col.enabled = false;
            }
        }
    }

    public void InteractWithObject()
    {
        if (hasBeenClicked) return;
        hasBeenClicked = true;

        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        StartCoroutine(PlayClipAndLoadScene());
    }

    // Plays object audio queues before executing the screen transition script
    private IEnumerator PlayClipAndLoadScene()
    {
        localAudioSource.PlayOneShot(immediateReflectionClip);
        yield return new WaitForSeconds(immediateReflectionClip.length + 0.5f);

        string targetSceneName = "";

        if (itemName == "WoodenSpoon") targetSceneName = "Kitchen";
        else if (itemName == "Wrench") targetSceneName = "Garage";
        else if (itemName == "Umbrella") targetSceneName = "Porch";

        VRSceneLoader.Instance.SwitchScene(targetSceneName);
    }
}