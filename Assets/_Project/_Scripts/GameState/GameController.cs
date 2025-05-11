using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Core Systems")]
    public GameStateManager gameStateManager;
    public MemoryProgressTracker memoryProgressTracker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ValidateDependencies();
    }

    private void ValidateDependencies()
    {
        if (gameStateManager == null)
            gameStateManager = GetComponent<GameStateManager>();

        if (memoryProgressTracker == null)
            memoryProgressTracker = GetComponent<MemoryProgressTracker>();
    }
}
