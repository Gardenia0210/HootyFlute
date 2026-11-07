using UnityEngine;
using UnityEngine.SceneManagement;
public class StartSongController : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
