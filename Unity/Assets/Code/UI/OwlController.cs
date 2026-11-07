using UnityEngine;
public class OwlController : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite openMouthSprite;
    private SpriteRenderer sr;
    private bool isPlaying = false;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
    }
    public void PlayHitReaction()
    {
        if (isPlaying) return;
        StartCoroutine(OpenMouthCoroutine());
    }
    private System.Collections.IEnumerator OpenMouthCoroutine()
    {
        isPlaying = true;
        sr.sprite = openMouthSprite;
        yield return new WaitForSeconds(1f);
        sr.sprite = idleSprite;
        isPlaying = false;
    }
}
