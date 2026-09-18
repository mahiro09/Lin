using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public void OpenStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}