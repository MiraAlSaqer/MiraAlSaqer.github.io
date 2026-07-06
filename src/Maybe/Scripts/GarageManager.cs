using System.Collections;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance;

    [Header("Audio Sources")]
    public AudioSource voiceOverSource;
    public AudioSource radioMusicSource;

    [Header("Voice Lines")]
    public AudioClip vo1_TurnOnRadio;
    public AudioClip vo2_HelpWithWrench;
    public AudioClip vo3_FirstBoltDone;
    public AudioClip vo4_BothBoltsDone;

    private bool radioTaskDone = false;
    private int boltsTightenedCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence()
    {
        yield return new WaitForSeconds(2.0f);
        PlayVoiceOver(vo1_TurnOnRadio);
        radioTaskDone = true;
    }

    public void InteractWithRadio()
    {
        if (!radioTaskDone) return;
        radioTaskDone = false;

        radioMusicSource.loop = true;
        radioMusicSource.Play();

        StartCoroutine(RadioTurnedOnSequence());
    }

    private IEnumerator RadioTurnedOnSequence()
    {
        yield return new WaitForSeconds(1.5f);
        PlayVoiceOver(vo2_HelpWithWrench);
    }

    public void BoltCompleted()
    {
        boltsTightenedCount++;

        if (boltsTightenedCount == 1)
        {
            PlayVoiceOver(vo3_FirstBoltDone);
        }
        else if (boltsTightenedCount == 2)
        {
            StartCoroutine(FinishLevelSequence());
        }
    }

    private IEnumerator FinishLevelSequence()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterItemInteraction("Wrench");
        }

        PlayVoiceOver(vo4_BothBoltsDone);
        yield return new WaitForSeconds(vo4_BothBoltsDone.length + 1.0f);

        VRSceneLoader.Instance.SwitchScene("Attic");
    }

    private void PlayVoiceOver(AudioClip clip)
    {
        voiceOverSource.Stop();
        voiceOverSource.clip = clip;
        voiceOverSource.Play();
    }
}