using System.Collections;
using UnityEngine;

public class AtticSceneBoot : MonoBehaviour
{
    // Delays when the attic boots up after returning from a memory to ensure that everything has properly rendered
    private IEnumerator Start()
    {
        yield return null;
        GameManager.Instance.RefreshTable();
    }
}