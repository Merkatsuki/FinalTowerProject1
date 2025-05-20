
using Momentum;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    [Header("Core References")]
    public CompanionController Companion;
    public Player Player;
    public GameObject DialogueUI;
    public InventoryManager Inventory;
    public FlagManager FlagManager;
    public CameraController Camera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
