using UnityEngine;
using UnityEngine.UI;

public class ChargeBarController : MonoBehaviour
{
    public int totalNotes;
    public Text scoreText;
    public GameObject resultScreen;
    public GameObject defeatSprite;
    public GameObject victorySprite;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public int starCount;
    public SpriteRenderer fillSprite;
    public Transform flag;
    public GameObject Confetti;
    public float maxWidth = 6f;
    public float smoothSpeed = 5f;
    private int hitNotes;
    private bool gameEnded;
    private float currentWidth;
    private Vector3 flagInitialPos;
    private Vector3 fillInitialScale;
    private float fillInitialLength;

    void Awake()
    {
        if (fillSprite != null)
        {
            fillInitialLength = fillSprite.bounds.size.x;
            fillInitialScale = fillSprite.transform.localScale;
        }
        if (flag != null) flagInitialPos = flag.localPosition;
        SetActive(resultScreen, false);
        SetActive(defeatSprite, false);
        SetActive(victorySprite, false);
        SetActive(star1, false);
        SetActive(star2, false);
        SetActive(star3, false);
        GameResultData.EnsureInstance();
        UpdateVisual();
    }

    public void AddHit()
    {
        if (gameEnded || totalNotes <= 0) return;
        hitNotes = Mathf.Min(hitNotes + 1, totalNotes);
        SetActive(Confetti, false);
        SetActive(Confetti, true);
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        int score = totalNotes > 0 ? Mathf.RoundToInt(100f * hitNotes / totalNotes) : 0;
        starCount = score >= 90 ? 3 : score >= 70 ? 2 : score >= 50 ? 1 : 0;
        if (scoreText != null) scoreText.text = score + "%";
        SetActive(defeatSprite, score < 50);
        SetActive(victorySprite, score >= 50);
        SetActive(star1, starCount >= 1);
        SetActive(star2, starCount >= 2);
        SetActive(star3, starCount >= 3);
        GameResultData.EnsureInstance();
        GameResultData.Instance.finalScore = score;
        GameResultData.Instance.starCount = starCount;
        SetActive(resultScreen, true);
    }

    void Update()
    {
        float target = totalNotes > 0 ? maxWidth * hitNotes / totalNotes : 0f;
        currentWidth = Mathf.Lerp(currentWidth, target, Time.deltaTime * smoothSpeed);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (fillSprite != null && fillInitialLength > 0f)
        {
            Vector3 scale = fillInitialScale;
            scale.x *= currentWidth / fillInitialLength;
            fillSprite.transform.localScale = scale;
        }
        if (flag != null) flag.localPosition = flagInitialPos + new Vector3(5f * currentWidth, 0f, 0f);
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null) target.SetActive(active);
    }
}
