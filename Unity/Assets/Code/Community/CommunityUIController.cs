using UnityEngine;
using UnityEngine.UI;
public class CommunityUIController : MonoBehaviour
{
    public Text scoreText;
    public GameObject star1On;
    public GameObject star1Off;
    public GameObject star2On;
    public GameObject star2Off;
    public GameObject star3On;
    public GameObject star3Off;
    void Start()
    {
        GameResultData.EnsureInstance();
        int finalScore = GameResultData.Instance.finalScore;
        int starCount = GameResultData.Instance.starCount;
        if (scoreText != null)
            scoreText.text = finalScore + " ";

        SetStar(star1On, star1Off, starCount >= 1);
        SetStar(star2On, star2Off, starCount >= 2);
        SetStar(star3On, star3Off, starCount >= 3);
    }
    void SetStar(GameObject onObj, GameObject offObj, bool isOn)
    {
        if (onObj != null) onObj.SetActive(isOn);
        if (offObj != null) offObj.SetActive(!isOn);
    }
}
