using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadnessPuzzleRoomManager : MonoBehaviour
{
    public static SadnessPuzzleRoomManager Instance { get; private set; }

    private static readonly string[] finalMessageSequence = { "I", "Give", "You", "This", "Tree", "To", "Remember"};

    [SerializeField] private SadnessPuzzleRoom currentRoom;

    [Header("All Rooms")]
    [SerializeField] private List<SadnessPuzzleRoom> allRooms;

    [Header("Final Room Unlock")]
    [SerializeField] private GameObject finalUnlockObject;

    private bool puzzleCompleted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void EnterRoom(SadnessPuzzleRoom newRoom)
    {
        if (currentRoom != null)
            currentRoom.gameObject.SetActive(false);

        currentRoom = newRoom;

        if (currentRoom != null)
        {
            currentRoom.gameObject.SetActive(true);
            currentRoom.RefreshState(); // Optional: reapply frame/phrase visuals
            Debug.Log($"[SadnessPuzzleRoomManager] Entered room: {currentRoom.name}");
        }
    }

    public SadnessPuzzleRoom GetCurrentRoom() => currentRoom;

    public void CheckGlobalPuzzleState()
    {
        if (puzzleCompleted)
            return;

        foreach (var room in allRooms)
        {
            string phrase = room.GetAssignedPhrase();
            int imageIndex = room.GetCurrentImageIndex();

            int expectedIndex = System.Array.IndexOf(finalMessageSequence, phrase);
            if (expectedIndex == -1)
            {
                Debug.LogWarning($"[SadnessPuzzleManager] Unexpected phrase '{phrase}' in room {room.name}");
                return;
            }

            if (imageIndex != expectedIndex)
            {
                Debug.Log($"[SadnessPuzzleManager] Room {room.name}: phrase '{phrase}' is set to index {imageIndex} — needs {expectedIndex}");
                return; // early exit if any are wrong
            }
        }

        Debug.Log("[SadnessPuzzleManager] Puzzle solved: full sentence aligned!");
        puzzleCompleted = true;

        if (finalUnlockObject != null)
            finalUnlockObject.SetActive(true);

        // Optional: play echo, trigger quip, etc.
    }
}
