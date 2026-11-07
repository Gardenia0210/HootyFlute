using UnityEngine;
public class NextNoteHintController : MonoBehaviour
{
    public Transform judgeLeft;
    public Transform judgeRightRight;
    public FingerHintLights fingerLights;
    private Note lastNote;
    void Update()
    {
        Note next = Note.FindTarget();
        if (next == lastNote || fingerLights == null) return;
        lastNote = next;
        if (next != null) fingerLights.ShowByNoteNumber(next.noteNumber);
        else fingerLights.Clear();
    }
}
