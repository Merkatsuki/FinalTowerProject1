using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Core Systems")]
    public GameStateManager gameStateManager;
    public SceneLoader sceneLoader;
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

        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();

        if (memoryProgressTracker == null)
            memoryProgressTracker = GetComponent<MemoryProgressTracker>();
    }
}
