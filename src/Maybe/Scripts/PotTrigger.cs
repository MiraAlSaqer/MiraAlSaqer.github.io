using UnityEngine;

public class PotTrigger : MonoBehaviour
{
    public KitchenManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spoon") || other.name.Contains("spoon"))
        {
            manager.SetSpoonInPot(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spoon") || other.name.Contains("spoon"))
        {
            manager.SetSpoonInPot(false);
        }
    }
}