using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private SocketIOComponent socket;

    private int nbPlayer;

    private string namePlayer;
    private int idPlayer;
    private List<Question> questions;

    private List<Player> listPlayer;
    private GameObject startButton;
    private GameManager gameManager;

    void Start()
    {
        questions = new List<Question>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startButton = GameObject.Find("startGame");
        startButton.SetActive(false);
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        nbPlayer = 0;

        socket.On("join", Join);
       
        socket.On("getQuestions", getQuestions);
        socket.On("respondedd", blbl);

        listPlayer = new List<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            j.AddField("id", 111);
            socket.Emit("responded", j);
        }
    }

    private void blbl(SocketIOEvent e)
    {
        Debug.Log("RESPONDEDD");
    }

    private void getQuestions(SocketIOEvent e)
    {
        int nbQuestion = e.data.GetField("questions").Count;
        for (int i = 0; i < nbQuestion; i++)
        {
            int nbReponse = e.data.GetField("questions")[i].GetField("answer").Count;
            List<string> reponses = new List<string>();
            Question q = new Question();

            q.SetEnonce(e.data.GetField("questions")[i].GetField("title").str);
            q.SetBonneReponse(int.Parse(e.data.GetField("questions")[i].GetField("goodAnswer").str));
            for (int j = 0; j < nbReponse; j++)
            {
                reponses.Add(e.data.GetField("questions")[i].GetField("answer").GetField(j.ToString()).str);
            }
            q.SetReponses(reponses);
            questions.Add(q);
        }

        GameManager.questions = questions;
        Debug.Log("QUESTIONS RECUES");
        gameManager.SetPlayerId(idPlayer);
        SceneManager.LoadScene("MainStage");
        gameManager.gameStarted = true;
        //Debug.Log("hhhhh" + questions.Count);
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
        //Debug.Log(e.data.GetField("namePlayerJson"));
        string nbPlayerSt = e.data.GetField("nbPlayer").Print();
        string idPlayerSt = e.data.GetField("id").Print();
        nbPlayer = int.Parse(nbPlayerSt);
        idPlayer = int.Parse(idPlayerSt);
        //PlaceTextElement(nbPlayer, idPlayer,"");

        GameObject inputNamePlayer = GameObject.Find("InputNamePlayer");
        GameObject joinButton = GameObject.Find("join_button");

        inputNamePlayer.SetActive(false);
        joinButton.SetActive(false);

        AddPlayers(e.data);
        PlaceTextOtherPlayers();

        if (nbPlayer == 1)
        {
            startButton.SetActive(true);
        }
        socket.On("joinAll", JoinAll);
    }

    public void StartGame()
    {
        socket.Emit("getQuestions");
        //passer toutes les variables necessaires au GameManager


    }

    private void JoinAll(SocketIOEvent e)
    {
   
        AddOtherPlayer(e.data);
        PlaceTextOtherPlayers();
    }


    private void PlaceTextElement(int idPlayer, string namePlayer )
    {

        GameObject canvas = GameObject.Find("Text_player");
        namePlayer = "Player " + idPlayer + " : " + namePlayer;
        CreateText(canvas.transform, 0, 0, namePlayer, 14, Color.black, idPlayer);
    }

    private void PlaceTextOtherPlayers()
    {
        //Debug.Log(nbPlayer);
        for (int i = 0; i < listPlayer.Count; i++)
        {
            //Debug.Log("sa passe");
            PlaceTextElement(i+1, listPlayer[i].name);
        }   
    }

    GameObject CreateText(Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color, int idPlayer)
    {
        GameObject UItextGO = new GameObject("Text" + idPlayer);
        UItextGO.transform.SetParent(canvas_transform);
       
        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(x, y);
        trans.sizeDelta = new Vector2(200, 20);

        Text text = UItextGO.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = font_size;
        text.color = text_color;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        text.transform.position = UItextGO.transform.position + new Vector3(0, -idPlayer * 50, 0);

        return UItextGO;
    }


    private void AddPlayers(JSONObject data)
    {
   
        for (int i = 0; i < nbPlayer; i++)
        {
            JSONObject playerElement = data.GetField("namePlayerJson")[i];
            Player player = new Player(int.Parse(playerElement.GetField("id").Print()), playerElement.GetField("name").Print()) as Player;
            listPlayer.Add(player);
            gameManager.AddPlayer(int.Parse(playerElement.GetField("id").Print()), playerElement.GetField("name").Print());
        }
      
    }

    private void AddOtherPlayer(JSONObject data)
    {
        string namePlayer = data.GetField("name").Print();
        string idPlayer = data.GetField("id").Print();
        Player player = new Player(int.Parse(idPlayer), namePlayer) as Player;
        listPlayer.Add(player);
        gameManager.AddPlayer(int.Parse(idPlayer), namePlayer);


    }
 }
