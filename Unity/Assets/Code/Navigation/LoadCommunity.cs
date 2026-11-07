using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadCommunity : MonoBehaviour
{
    public void ToCommunity()
    {
        SceneManager.LoadScene("CommunityShare");
    }
}
