﻿using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    //--UI
    public GameObject playerUi;
    public GameObject UiClassement;
    QuestionParser qp;
    public static List<Question> questions;
    public List<GameObject> chairs;
    public List<GameObject> persos;
    public Material[] materials;
    public GameObject AnswerPrefabs;
    public bool gameStarted = false;
    public List<string> responses;
    //--PLAYER
    private int playerId = 985;
    private string playerName;

    //--PARTIE
    public int nbPlayer = 10;
    private Dictionary<int, int> playersAnswer = new Dictionary<int, int>();
    [SerializeField]
    private Dictionary<int, int> playersHasAnswered = new Dictionary<int, int>();
    private Dictionary<int, string> players = new Dictionary<int, string>();
    private Dictionary<int, int> scores = new Dictionary<int, int>();
    private int currentAnswer;
    private int currentQuestion;
    private bool isCorrectAnswer = false;
    private int nbQuestion = 0;
    private Text QuestionsUI;
    private List<GameObject> ListButtonAnswers;

    private bool inQuestion = false;

    //--SERVEUR
    public SocketIOComponent socket;
    private bool initialized = false;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        players.Clear();
        chairs = new List<GameObject>();

        ResetPlayersAnswer();
        DisplayHasAnswered();

        questions = new List<Question>();
        ListButtonAnswers = new List<GameObject>();
        //--SERVEUR-------
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        responses = new List<string>();
        //socket.On("setReponse", getIsCorrectAnswer);
       socket.On("respondedd", aPlayerResponded);
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "MainStage")
        {
            if (!initialized) //one ne met dans ce if que les choses qu'on ne veut faire qu'une seule fois
            {
                Init(nbPlayer);
                socket.Emit("getCurrentQuestion");
                DisplayHasAnswered();
                initialized = true;
                        InitialisePerso();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            socket.Emit("getCurrentQuestion");
        }
    }

    public void DisplayQuestion()
    {
        int nbAnswer = questions[currentQuestion].GetReponses().Count;
        List<string> Answers = questions[currentQuestion].GetReponses();
        int rightAnswer = questions[currentQuestion].GetBonneReponse();
        for (int i = 0; i < nbAnswer; i++)
        {
            Vector3 pos = new Vector3(647, 450 - (i * 70), 0);

            GameObject AnswerInstantiate = Instantiate(AnswerPrefabs, pos, Quaternion.identity, GameObject.Find("GridLayout").transform);
            AnswerInstantiate.GetComponent<AnswerButton>().index = i;
            // AnswerInstantiate.transform.SetParent(GameObject.Find("Canvas").transform);

            ListButtonAnswers.Add(AnswerInstantiate);
            AnswerInstantiate.GetComponent<AnswerButton>().goodAnswer = RightAnswer(rightAnswer, i);
            AnswerInstantiate.GetComponentInChildren<Text>().text = Answers[i];
        }
    }

    public void SetupServerOn()
    {
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        socket.On("respondedd", aPlayerResponded);
        socket.On("getCurrentQuestion", getCurrentQuestion);
        socket.On("setReponse", isGoodAnswer);
        socket.On("getScore", setupdicoScore);
        //socket.On("")
    }


    //-----------GETTERS-------------
    public SocketIOComponent GetSocket()
    {
        return GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    public int GetPlayerId()
    {
        return playerId;
    }
    //-----------/GETTERS-------------






    //-----------SETTERS-------------
    public void SetPlayerId(int i)
    {
        playerId = i;
    }

    public void SetPlayerAnswer(int i)
    {
        currentAnswer = i;
    }

    public void SetHasAnswered()
    {
        Debug.Log("ID : " + playerId);
        playersHasAnswered[playerId] = 1;
        DisplayHasAnswered();
    }
    //-----------/SETTERS-------------







    //-----------SERVER ON-------------
    public void isGoodAnswer(SocketIOEvent e)
    {
        int id = int.Parse(e.data.GetField("id").ToString());
        Debug.Log("le joueur "+id+" a répondu !");
        playersAnswer[id] = int.Parse(e.data.GetField("answer").ToString());
    }

    public void aPlayerResponded(SocketIOEvent e)
    {
        int pId = int.Parse(e.data.GetField("id").ToString());
        //Debug.Log("UN PLAYER A REPONDU id : " + pId);
        playersHasAnswered[pId] = 1;
        DisplayHasAnswered();
        //responses.Add(pId.ToString());
    }

    private void getCurrentQuestion(SocketIOEvent e)
    {
        ClearButton();
        //Debug.Log("je rentre dans le on de getCurrentQuestion");
        currentQuestion = int.Parse(e.data.GetField("question").ToString());
        Debug.Log("current question : "+ e.data);
        DisplayQuestion();
        QuestionsUI = GameObject.Find("Question").GetComponent<Text>();
        QuestionsUI.text = questions[currentQuestion].GetEnonce();
        
          StartCoroutine(StartQuestion());
    }
    //-----------/SERVER ON-------------







    //-----------SERVER EMIT-------------
    public void SendReponse(int i)
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("id", playerId);
        j.AddField("answer", i);
        socket.Emit("setReponse", j);
        ClearButton();
    }
    //-----------/SERVER EMIT-------------








    //-----------DISPLAY-------------
    private void DisplayIsCorrectAnswer()
    {
        foreach (int i in playersAnswer.Keys)
        {
            if (playersAnswer[i] == 0)
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[3]; //mauvaise reponse
            }
            else if (playersAnswer[i] == 1)
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[2]; //Bonne reponse
            }
        }
    }

    private void DisplayHasAnswered()
    {
        foreach (int i in playersHasAnswered.Keys)
        {
            if (playersHasAnswered[i] != -1)
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[1];
            }
            else
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[0];
            }
        }
    }
    //-----------/DISPLAY-------------









    public IEnumerator StartQuestion()
    {
        yield return new WaitForSeconds(20);
        StartCoroutine(StartInterQuestion());
    }

    public IEnumerator StartInterQuestion()
    {
        DisplayIsCorrectAnswer();
        yield return new WaitForSeconds(5);
        if (currentQuestion == nbQuestion - 1)
        {
            Debug.Log("plus de question");
            StartCoroutine(SetUpClassement());
            socket.Emit("resetVariables");
        }
        else
        {
            socket.Emit("getCurrentQuestion");
            ResetPlayersAnswer();
        }

    }

    public void InitialiseChairs()
    {
        for (int i = 0; i < 18; i++)
        {
            chairs.Add(GameObject.Find("Slot"+(i +1)));
            Debug.Log(GameObject.Find("Slot" + (i + 1)));
        }
    }

    public void InitialisePerso()
    {
        Debug.Log("nb persos : " + nbPlayer);
        for (int i = 0; i < 18; i++)
        {
            if (i < nbPlayer)
            {
                GameObject.Find("perso" + (i + 1)).active = true;
            }
            else
            {
                GameObject.Find("perso" + (i + 1)).active = false;
            }
        }

        //initialisation des player names
        GameObject playerNames = GameObject.Find("PlayerNames");
        for (int i = 0; i < playerNames.transform.childCount; i++)
        {
            if (i < nbPlayer)
            {
                playerNames.transform.GetChild(i).gameObject.active = true;
                playerNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = players[i].Substring(1, players[i].Length-2);
            }
            else
            {
                playerNames.transform.GetChild(i).gameObject.active = false;
            }
        }
    }


    /// <summary>
    /// Permet de reset le tableau des réponse de façon à de nouveau dire qu'aucun joueur a répondu
    /// </summary>
    private void ResetPlayersAnswer()
    {
        /*foreach (int i in playersHasAnswered.Keys)
        {
            Debug.Log(i);
            playersHasAnswered[i] = -1;
        }*/

        foreach (int i in playersHasAnswered.Keys)
        {
            chairs[i].GetComponent<MeshRenderer>().material = materials[0];
        }
    }

    public void Init(int nbP)
    {
        SetupServerOn();
        nbPlayer = nbP;

        nbQuestion = questions.Count;

        //initialisation de la listes des reponses
        foreach (int i in players.Keys)
        {
            playersAnswer.Add(i, -1);
            playersHasAnswered.Add(i, -1);
        }
        InitialiseChairs();
    }

    public void AddPlayer(int id, string n)
    {
        players.Add(id, n);
    }

    bool RightAnswer(int rightAnswer, int idAnswer)
    {
        return rightAnswer == idAnswer;
    }

    IEnumerator SetUpClassement()
    {
        socket.Emit("getScore");      
        yield return new WaitForSeconds(2);
        List<int> classementID = sortbyScore();
        Instantiate(UiClassement, GameObject.Find("Canvas").transform);
        //classement des joueurs dans une list
        foreach (int i in players.Keys)
         {
            GameObject UI = Instantiate(playerUi, transform.position, Quaternion.identity, GameObject.Find("Names").transform);
            Debug.Log(scores[i].ToString());
            UI.GetComponent<Text>().text = players[i].Substring(1, players[i].Length - 2) + " : " + scores[i].ToString();
         }
    }

    void setupdicoScore(SocketIOEvent e)
    {
        JSONObject jsonListScore = e.data.GetField("scoreJSON");
        for (int i = 0; i < jsonListScore.Count; i++)
        {
            int id = int.Parse(jsonListScore[i].GetField("id").ToString());
            int score = int.Parse(jsonListScore[i].GetField("score").ToString());
            scores.Add(id, score);
        }
        Debug.Log(scores.Keys.Count + " dfihggggggggggggggggggggggggggggggggg");
    }

    List<int> sortbyScore()
    {
        List<int> result = new List<int>();
        int scoremax = 0;
        Debug.Log(scores.Keys.Count + " scores.Keys");
        foreach (int id in scores.Keys)
        {
            if (scoremax < scores[id])
            {
                result.Insert(0, id);
            }
            else
            {
                result.Add(id);
            }
            Debug.Log(id);
        }
        return result;
    }

    void ClearButton()
    {
        foreach (GameObject g in ListButtonAnswers)
        {
            Destroy(g);
        }
        ListButtonAnswers.Clear();
    }
}
