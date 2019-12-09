using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class AnswerButton : MonoBehaviour
{
    public bool goodAnswer = false;
    public int index;

    private SocketIOComponent socket;

    public void hasAnswer()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().GetSocket().Emit("respond");
    }
}
