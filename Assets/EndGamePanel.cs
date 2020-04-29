using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(0);
    }
}
