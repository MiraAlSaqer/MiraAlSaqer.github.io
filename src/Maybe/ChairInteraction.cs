using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChairInteraction : MonoBehaviour
{
    [Header("Sit UI Prompt")]
    public GameObject sitPromptCanvas;

    [Header("Player Stuff")]
    public Transform xrOrigin;
    public Transform vrCamera;
    public GameObject locomotionGameObject;

    [Header("Sitting Alignment Positions")]
    public Transform sitTargetTransform;
    public float sitHeightDrop = 0.4f;

    [Header("Fade Out Canvas")]
    public Image whiteFadeImage;

    private bool alreadySatDown = false;

    private void Start()
    {
        sitPromptCanvas.SetActive(false);
        whiteFadeImage.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnChairClicked()
    {
        if (alreadySatDown) return;
        alreadySatDown = true;

        sitPromptCanvas.SetActive(false);

        StartCoroutine(ExecuteSittingAndFadeSequence());
    }

    private IEnumerator ExecuteSittingAndFadeSequence()
    {
        locomotionGameObject.SetActive(false);

        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Vector3 cameraWorldPos = vrCamera.position;
        Vector3 targetWorldPos = sitTargetTransform.position;

        cameraWorldPos.y = xrOrigin.position.y;
        targetWorldPos.y = xrOrigin.position.y;

        Vector3 slideVector = targetWorldPos - cameraWorldPos;
        Vector3 startOriginPos = xrOrigin.position;
        Vector3 finalOriginPos = startOriginPos + slideVector;

        float elapsed = 0f;
        while (elapsed < 1.5f)
        {
            xrOrigin.position = Vector3.Lerp(startOriginPos, finalOriginPos, elapsed / 1.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        xrOrigin.position = finalOriginPos;

        Transform cameraOffset = xrOrigin.Find("Camera Offset");
        if (cameraOffset != null)
        {
            Vector3 startOffset = cameraOffset.localPosition;
            Vector3 targetOffset = startOffset - new Vector3(0, sitHeightDrop, 0);

            float dropElapsed = 0f;
            while (dropElapsed < 0.5f)
            {
                cameraOffset.localPosition = Vector3.Lerp(startOffset, targetOffset, dropElapsed / 0.5f);
                dropElapsed += Time.deltaTime;
                yield return null;
            }
            cameraOffset.localPosition = targetOffset;
        }

        yield return new WaitForSeconds(3.0f);

        float fadeElapsed = 0f;
        while (fadeElapsed < 2.0f)
        {
            fadeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Clamp01(fadeElapsed / 2.0f);
            whiteFadeImage.color = new Color(1f, 1f, 1f, newAlpha);
            yield return null;
        }
        whiteFadeImage.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Field");
    }
}