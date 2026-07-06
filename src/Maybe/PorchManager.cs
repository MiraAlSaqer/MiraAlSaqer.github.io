using System.Collections;
using UnityEngine;

public class PorchManager : MonoBehaviour
{
    public static PorchManager Instance;

    [Header("Audio Sources")]
    public AudioSource friendVoiceSource;
    public AudioSource backgroundMusicSource;

    [Header("Voice Lines")]
    public AudioClip vo1_ComeOnRain;
    public AudioClip vo2_PickUpUmbrella;
    public AudioClip vo3_FinalLine;

    [Header("Porch Stuff")]
    public Transform playerTransform;
    public Transform destinationTarget;
    public float arrivalDistance = 1.5f;

    private bool isUmbrellaOpened = false;
    private bool sequenceFinished = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();

        StartCoroutine(PorchOpeningSequence());
    }

    private IEnumerator PorchOpeningSequence()
    {
        yield return new WaitForSeconds(3.0f);
        PlayFriendVO(vo1_ComeOnRain);

        yield return new WaitForSeconds(vo1_ComeOnRain.length + 2.0f);
        PlayFriendVO(vo2_PickUpUmbrella);
    }

    public void ConfirmUmbrellaDeployment()
    {
        isUmbrellaOpened = true;
    }

    private void Update()
    {
        if (!isUmbrellaOpened || sequenceFinished) return;

        // Tracks player distance on the floor
        float distance = Vector3.Distance(
            new Vector3(playerTransform.position.x, 0, playerTransform.position.z),
            new Vector3(destinationTarget.position.x, 0, destinationTarget.position.z)
        );

        if (distance <= arrivalDistance)
        {
            StartCoroutine(FinishPorchSequence());
        }
    }

    private IEnumerator FinishPorchSequence()
    {
        sequenceFinished = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterItemInteraction("Umbrella");
        }

        PlayFriendVO(vo3_FinalLine);
        yield return new WaitForSeconds(vo3_FinalLine.length + 1.0f);

        VRSceneLoader.Instance.SwitchScene("Attic");
    }

    private void PlayFriendVO(AudioClip clip)
    {
        friendVoiceSource.Stop();
        friendVoiceSource.clip = clip;
        friendVoiceSource.Play();
    }
}