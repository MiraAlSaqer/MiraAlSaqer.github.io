using UnityEngine;

public class AtticTableManager : MonoBehaviour
{
    [Header("Table Visual Models")]
    public GameObject tableSpoonVisual;
    public GameObject tableWrenchVisual;
    public GameObject tableUmbrellaVisual;

    private void OnEnable()
    {
        GameManager.OnItemRegistered += HandleItemVisuals;
    }

    private void OnDisable()
    {
        GameManager.OnItemRegistered -= HandleItemVisuals;
    }

    private void Start()
    {
        tableSpoonVisual.SetActive(false);
        tableWrenchVisual.SetActive(false);
        tableUmbrellaVisual.SetActive(false);
    }

    // Show the objects on the table
    private void HandleItemVisuals(string itemName)
    {
        if (itemName == "WoodenSpoon") tableSpoonVisual.SetActive(true);
        if (itemName == "Wrench") tableWrenchVisual.SetActive(true);
        if (itemName == "Umbrella") tableUmbrellaVisual.SetActive(true);
    }
}