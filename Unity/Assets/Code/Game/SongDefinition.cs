using UnityEngine;
[CreateAssetMenu(menuName = "Hooty Flute/Song")]
public class SongDefinition : ScriptableObject
{
    public string title;
    [Range(0.1f, 10f)] public float interval = 1.4f;
    public int[] notes;
}
