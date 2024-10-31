using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    public void PlayGameAgain()
    {
        SceneManager.LoadScene(0);
    }
}
