using UnityEngine;

public class HallwayTriggerZone : MonoBehaviour
{
    public int triggerID;
    private bool hasBeenTripped = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTripped) return;
        hasBeenTripped = true;

        HallwayManager manager = FindFirstObjectByType<HallwayManager>();
        manager.PlayerEnteredTrigger(triggerID);
    }
}