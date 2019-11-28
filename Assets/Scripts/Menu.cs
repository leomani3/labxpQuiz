using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private SocketIOComponent socket;

    private int nbPlayer;

    private string namePlayer;
    private int idPlayer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        nbPlayer = 0;

        socket.On("join", Join);
        socket.On("joinAll", JoinAll);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnclickButtonJoin()
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);

        string namePlayerInput = GameObject.Find("text_namePlayer").GetComponent<Text>().text;

        namePlayer = namePlayerInput;
        j.AddField("name", namePlayer);
        socket.Emit("join", j);
        
    }

    private void Join(SocketIOEvent e)
    {
        string nbPlayerSt = e.data.GetField("nbPlayer").Print();
        string idPlayerSt = e.data.GetField("id").Print();
        nbPlayer = int.Parse(nbPlayerSt);
        idPlayer = int.Parse(idPlayerSt);
        PlaceTextElement(nbPlayer, idPlayer);
    }

    private void JoinAll(SocketIOEvent e)
    {
        Debug.Log(e.data);
    }


    private void PlaceTextElement(int nbPlayer , int idPlayer )
    {
        /*        Debug.Log("sa passe");

                GameObject textObject = new GameObject("new text " + nbPlayer);
                Text myText = textObject.AddComponent<Text>();
                myText.text = ;
                myText.transform.position = new Vector2(0, 0);


                textObject.transform.SetParent(canvas.transform);*/

        GameObject canvas = GameObject.Find("Text_player");
        string newText = "Player " + nbPlayer + " " + namePlayer;
        CreateText(canvas.transform, 0, 0, newText, 14, Color.black, idPlayer);


    }

    GameObject CreateText(Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color, int idPlayer)
    {
        GameObject UItextGO = new GameObject("Text2");
        UItextGO.transform.SetParent(canvas_transform);
        UItextGO.transform.position = new Vector2(0, -idPlayer + 10);
        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(x, y);

        Text text = UItextGO.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = font_size;
        text.color = text_color;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        return UItextGO;
    }
}
