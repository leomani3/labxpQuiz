using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Menu : MonoBehaviour
{
    private SocketIOComponent socket;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("getQuestions", Join);
        socket.On("joinAll", JoinAll);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnclickButtonJoin()
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("name", "Homertimes");
        socket.Emit("getQuestions", j);
        
    }

    private void Join(SocketIOEvent e)
    {
        Debug.Log(e.data.GetField("questions")[1].GetField("title"));
    }

    private void JoinAll(SocketIOEvent e)
    {
        Debug.Log(e.data);
    }
}
