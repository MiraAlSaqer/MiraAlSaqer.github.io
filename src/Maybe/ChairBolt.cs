using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ChairBolt : MonoBehaviour
{
    [Header("Mesh References")]
    public Transform boltMeshTransform;
    public AudioSource boltAudioSource;

    [Header("SFX Clips")]
    public AudioClip wrenchScrapingLoop;
    public AudioClip boltClickSuccess;

    [Header("Bolt Interactions")]
    public float requiredTime = 5.0f;
    public Vector3 innerZOffset = new Vector3(0, 0, 0.05f);

    private XRSocketInteractor socket;
    private Coroutine tighteningCoroutine;
    private float progressTimer = 0f;
    private bool isFullyTightened = false;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void Start()
    {
        startPos = boltMeshTransform.localPosition;
        targetPos = startPos + innerZOffset;
    }

    private void OnEnable()
    {
        socket.hoverEntered.AddListener(OnWrenchHoverEnter);
        socket.hoverExited.AddListener(OnWrenchHoverExit);
    }

    private void OnDisable()
    {
        socket.hoverEntered.RemoveListener(OnWrenchHoverEnter);
        socket.hoverExited.RemoveListener(OnWrenchHoverExit);
    }

    private void OnWrenchHoverEnter(HoverEnterEventArgs args)
    {
        if (isFullyTightened) return;

        if (args.interactableObject.transform.name.Contains("Wrench") || args.interactableObject.transform.CompareTag("Wrench"))
        {
            boltAudioSource.clip = wrenchScrapingLoop;
            boltAudioSource.loop = true;
            boltAudioSource.Play();

            if (tighteningCoroutine != null) StopCoroutine(tighteningCoroutine);
            tighteningCoroutine = StartCoroutine(TightenSequence());
        }
    }

    private void OnWrenchHoverExit(HoverExitEventArgs args)
    {
        if (isFullyTightened) return;

        if (tighteningCoroutine != null)
        {
            StopCoroutine(tighteningCoroutine);
            boltAudioSource.Stop();
        }
    }

    private IEnumerator TightenSequence()
    {
        while (progressTimer < requiredTime)
        {
            progressTimer += Time.deltaTime;
            float progress = progressTimer / requiredTime;

            boltMeshTransform.localPosition = Vector3.Lerp(startPos, targetPos, progress);

            yield return null;
        }

        boltMeshTransform.localPosition = targetPos;
        isFullyTightened = true;

        boltAudioSource.Stop();
        boltAudioSource.loop = false;
        boltAudioSource.PlayOneShot(boltClickSuccess);

        socket.enabled = false;

        if (GarageManager.Instance != null)
        {
            GarageManager.Instance.BoltCompleted();
        }
    }
}