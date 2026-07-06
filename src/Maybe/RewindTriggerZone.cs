using UnityEngine;

public class RewindTriggerZone : MonoBehaviour
{
    public int triggerID;

    private bool hasBeenTripped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTripped) return;

        if (other.CompareTag("MainCamera") || other.gameObject.layer == 0 || other.GetComponentInChildren<Camera>() != null)
        {
            hasBeenTripped = true;

            RewindManager manager = FindFirstObjectByType<RewindManager>();
            manager.PlayerEnteredTrigger(triggerID);
        }
    }
}