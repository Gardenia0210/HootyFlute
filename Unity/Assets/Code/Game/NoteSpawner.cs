using UnityEngine;
public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnPoint;
    public Transform judgeLeft;
    public Transform judgeRight;
    public Transform destroyPointBeforeHit;
    public Transform destroyPointAfterHit;
    public OwlController owl;
    public float spawnInterval = 1.4f;
    public Sprite[] numberSprites;
    public Sprite[] musicNoteSprites;
    public ChargeBarController chargeBarController;
    public SongDefinition song;
    private int index;
    private int resolvedCount;
    private bool ending;
    void Start()
    {
        if (song == null || song.notes == null || song.notes.Length == 0 ||
            notePrefab == null || notePrefab.GetComponent<Note>() == null ||
            notePrefab.GetComponent<SpriteRenderer>() == null || spawnPoint == null ||
            judgeLeft == null || judgeRight == null || destroyPointBeforeHit == null ||
            destroyPointAfterHit == null || chargeBarController == null)
        {
            Debug.LogError("Song or gameplay references are incomplete.", this);
            enabled = false;
            return;
        }
        foreach (int number in song.notes)
            if (number < 1 || number > 8 || numberSprites == null ||
                number >= numberSprites.Length || numberSprites[number] == null)
            {
                Debug.LogError("Song contains an unsupported note sprite.", this);
                enabled = false;
                return;
            }
        chargeBarController.totalNotes = song.notes.Length;
        InvokeRepeating(nameof(SpawnNote), 1f, Mathf.Max(0.1f, song.interval));
    }
    void SpawnNote()
    {
        if (index >= song.notes.Length) { CancelInvoke(); return; }
        int number = song.notes[index++];
        GameObject instance = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        instance.GetComponent<SpriteRenderer>().sprite = numberSprites[number];
        Note note = instance.GetComponent<Note>();
        note.noteNumber = number;
        note.judgeLeft = judgeLeft;
        note.judgeRight = judgeRight;
        note.destroyPointBeforeHit = destroyPointBeforeHit;
        note.destroyPointAfterHit = destroyPointAfterHit;
        note.noteSprites = musicNoteSprites;
        note.owl = owl;
        note.Resolved += OnNoteResolved;
    }
    void OnNoteResolved()
    {
        resolvedCount++;
        if (!ending && resolvedCount == song.notes.Length)
        {
            ending = true;
            chargeBarController.EndGame();
        }
    }
    void OnDisable() { CancelInvoke(); }
}
