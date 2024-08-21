using UnityEngine.SceneManagement;
using UnityEngine;

public class returnMenu : MonoBehaviour
{
    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
