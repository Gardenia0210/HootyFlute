using UnityEngine;
public class FingerHintLights : MonoBehaviour
{
    public Animator[] owls;
    private static readonly int[] masks = { 0, 255, 254, 252, 248, 240, 224, 192, 160 };
    public void ShowByNoteNumber(int noteNumber)
    {
        Clear();
        if (noteNumber < 1 || noteNumber > 8 || owls == null) return;
        for (int i = 0; i < Mathf.Min(8, owls.Length); i++)
            if (owls[i] != null && (masks[noteNumber] & (1 << i)) != 0)
                owls[i].SetTrigger("Jump");
    }
    public void Clear()
    {
        if (owls == null) return;
        foreach (Animator owl in owls)
            if (owl != null) owl.ResetTrigger("Jump");
    }
}
