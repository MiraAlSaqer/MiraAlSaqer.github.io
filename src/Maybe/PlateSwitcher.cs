using UnityEngine;

public class PlateSwitcher : MonoBehaviour
{
    [Header("Plate Meshes")]
    public GameObject brokenPlate;
    public GameObject fixedPlate;

    private void Awake()
    {
        brokenPlate.SetActive(true);
        fixedPlate.SetActive(false);
    }

    public void OnPlateSocketed()
    {
        brokenPlate.SetActive(false);
        fixedPlate.SetActive(true);

        RewindManager manager = FindFirstObjectByType<RewindManager>();
        manager.PlacedPlateInSocket();
    }
}