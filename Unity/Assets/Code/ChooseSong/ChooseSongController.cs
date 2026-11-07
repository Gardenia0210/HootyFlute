using UnityEngine;
public class ChooseSongController : MonoBehaviour
{
    public GameObject songActive;

    public void OnSongButtonClick(GameObject buttonSelf)
    {
        if (songActive != null)
            songActive.SetActive(true);
        if (buttonSelf != null)
            buttonSelf.SetActive(false);
    }
}
