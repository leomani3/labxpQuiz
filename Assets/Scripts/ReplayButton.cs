using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().socket.Emit("resetVariables");
        SceneManager.LoadScene(0);
    }
}
