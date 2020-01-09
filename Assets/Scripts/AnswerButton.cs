using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class AnswerButton : MonoBehaviour
{
    public bool goodAnswer = false;
    public int index;

    private SocketIOComponent socket;

    public void Answer()
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("id", GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayerId());
        GameObject.Find("GameManager").GetComponent<GameManager>().SetHasAnswered();
        GameObject.Find("GameManager").GetComponent<GameManager>().GetSocket().Emit("responded", j);
        GameObject.Find("GameManager").GetComponent<GameManager>().SetPlayerAnswer(index);
        GameObject.Find("GameManager").GetComponent<GameManager>().SendReponse(index);
    }
}
