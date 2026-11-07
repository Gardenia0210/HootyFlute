 using UnityEngine;
public class GameResultData : MonoBehaviour
{
    public static GameResultData Instance;
    public int finalScore;
    public int starCount;
    public static void EnsureInstance()
    {
        if (Instance == null) new GameObject("GameResultData").AddComponent<GameResultData>();
    }
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnDestroy() { if (Instance == this) Instance = null; }
}
