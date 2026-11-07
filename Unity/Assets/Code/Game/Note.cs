using System.Collections.Generic;
using UnityEngine;
public enum NoteState { Number, Hit }
public class Note : MonoBehaviour
{
    public int noteNumber;
    public float moveSpeed = 2f;
    public Transform judgeLeft;
    public Transform judgeRight;
    public Transform destroyPointBeforeHit;
    public Transform destroyPointAfterHit;
    public Sprite[] noteSprites;
    public OwlController owl;
    private static readonly List<Note> activeNotes = new List<Note>();
    private SpriteRenderer spriteRenderer;
    private NoteState state;
    private bool resolved;
    public event System.Action Resolved;
    public bool CanJudge => !resolved && state == NoteState.Number && judgeLeft != null &&
        judgeRight != null && transform.position.x >= judgeLeft.position.x &&
        transform.position.x <= judgeRight.position.x;
    void Awake() { spriteRenderer = GetComponent<SpriteRenderer>(); }
    void OnEnable() { activeNotes.Add(this); }
    void OnDisable() { activeNotes.Remove(this); }
    public static Note FindTarget()
    {
        Note closest = null;
        foreach (Note note in activeNotes)
            if (note != null && note.CanJudge &&
                (closest == null || note.transform.position.x < closest.transform.position.x))
                closest = note;
        return closest;
    }
    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (state == NoteState.Number)
        {
            if (judgeLeft != null && transform.position.x < judgeLeft.position.x) Resolve();
            if (destroyPointBeforeHit != null && transform.position.x <= destroyPointBeforeHit.position.x)
            {
                Resolve();
                Destroy(gameObject);
            }
        }
        else if (destroyPointAfterHit != null && transform.position.x <= destroyPointAfterHit.position.x)
            Destroy(gameObject);
    }
    public void TryHit(int inputNumber)
    {
        if (!CanJudge || inputNumber != noteNumber) return;
        state = NoteState.Hit;
        if (spriteRenderer != null && noteSprites != null && noteSprites.Length > 0)
            spriteRenderer.sprite = noteSprites[Random.Range(0, noteSprites.Length)];
        if (owl != null) owl.PlayHitReaction();
        ChargeBarController charge = FindObjectOfType<ChargeBarController>();
        if (charge != null) charge.AddHit();
        Resolve();
    }
    private void Resolve()
    {
        if (resolved) return;
        resolved = true;
        Resolved?.Invoke();
    }
}
