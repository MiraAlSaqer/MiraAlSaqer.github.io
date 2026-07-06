using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Personality Tracking")]
    public int personalityTrait { get; private set; } = 0;
    private bool hasMadeFirstChoice = false;

    [Header("Track Memory Completion")]
    public bool kitchenCompleted = false;
    public bool garageCompleted = false;
    public bool porchCompleted = false;

    public static event Action<string> OnItemRegistered;
    public static event Action OnAllItemsCollected;

    private int totalItemsCollected = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Handles progression data storage, path locking, and event triggers
    public void RegisterItemInteraction(string itemName)
    {
        if (!hasMadeFirstChoice)
        {
            hasMadeFirstChoice = true;
            if (itemName == "WoodenSpoon") personalityTrait = 1;
            else if (itemName == "Wrench") personalityTrait = 2;
            else if (itemName == "Umbrella") personalityTrait = 3;
        }

        if (itemName == "WoodenSpoon") kitchenCompleted = true;
        if (itemName == "Wrench") garageCompleted = true;
        if (itemName == "Umbrella") porchCompleted = true;

        totalItemsCollected++;

        RefreshTable();

        OnItemRegistered?.Invoke(itemName);

        if (totalItemsCollected == 3)
        {
            OnAllItemsCollected?.Invoke();
        }
    }

    // Synchronizes the physical item states displayed on the attic table layout
    public void RefreshTable()
    {
        if (SceneManager.GetActiveScene().name != "Attic") return;

        GameObject tableParent = GameObject.Find("Table");

        Transform wrenchTransform = tableParent.transform.Find("Wrench Table");
        Transform umbrellaTransform = tableParent.transform.Find("Umbrella Table");
        Transform spoonTransform = tableParent.transform.Find("Spoon Table");

        spoonTransform.gameObject.SetActive(kitchenCompleted);
        wrenchTransform.gameObject.SetActive(garageCompleted);
        umbrellaTransform.gameObject.SetActive(porchCompleted);
    }
}