using UnityEngine;
using UnityEngine.SceneManagement;
public class StartButtonController : MonoBehaviour
{

    public void LoadChooseSong()
    {
        SceneManager.LoadScene("ChooseSong");
    }
}
