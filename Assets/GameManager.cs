using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //--SINGLETON
    private static GameManager Instance;

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
    private int currentQuestion;
    private bool isCorrectAnswer = false;
    private int nbQuestion = 0;
    private Text QuestionsUI;
    private List<GameObject> ListButtonAnswers;

    private bool inQuestion = false;

    //--SERVEUR
    public SocketIOComponent socket;
    private bool initialized = false;


    //AWAKE : On gère le singleton
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
        //SINGLETON
        DontDestroyOnLoad(gameObject);

        //--SERVEUR-------
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

        players.Clear();
        chairs = new List<GameObject>();

        //Initialisation de l'affiche des sièges (status : non répondu) pour le début du jeu
        ResetPlayersAnswer();
        DisplayHasAnswered();

        //UI
        questions = new List<Question>();
        ListButtonAnswers = new List<GameObject>();
        responses = new List<string>();
    }



    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "MainStage") //Si nous sommes sur la scène de jeu
        {
            if (!initialized) //Ce if permet de n'executer qu'une seule fois toute la partie d'initialisation du jeu
            {
                Init(nbPlayer);
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

    /// <summary>
    /// Permet d'initialiser tous les écoutes du client sur le serveur
    /// </summary>
    public void SetupServerOn()
    {
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

        socket.On("getCurrentQuestion", getCurrentQuestion);
        socket.On("setReponse", isGoodAnswer);
        socket.On("getScore", setupdicoScore);
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

    public void SetHasAnswered()
    {
        Debug.Log("ID : " + playerId);
        playersHasAnswered[playerId] = 1;
        DisplayHasAnswered();
    }
    //-----------/SETTERS-------------







    //-----------SERVER ON-------------

    /// <summary>
    /// Permet de dire si la réponse donnée est la bonne ou non
    /// </summary>
    /// <param name="e"></param>
    public void isGoodAnswer(SocketIOEvent e)
    {
        int id = int.Parse(e.data.GetField("id").ToString());
        playersAnswer[id] = int.Parse(e.data.GetField("answer").ToString());
    }

    /// <summary>
    /// Permet de récupérer lorsqu'un joueur répond à une question
    /// </summary>
    /// <param name="e"></param>
    public void aPlayerResponded(SocketIOEvent e)
    {
        int pId = int.Parse(e.data.GetField("id").ToString());
        playersHasAnswered[pId] = 1;
        DisplayHasAnswered();
    }

    /// <summary>
    /// Permet de récupérer la question courante que le serveur nous renvoie
    /// </summary>
    /// <param name="e"></param>
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

    /// <summary>
    /// Le client envoie sa réponse au serveur. Le serveur nous répondra si c'est une bonne réponse ou non.
    /// </summary>
    /// <param name="i"></param>
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

     /// <summary>
     /// permet de le colorer les sièges en fonction de si la réponse est bonne ou non.
     /// </summary>
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

    /// <summary>
    /// Permet d'allumer le siège lorsque qu'un joueur répond
    /// </summary>
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



    /// <summary>
    /// Lance le timer de la question (temps que le joueur a pour répondre à la question)
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartQuestion()
    {
        yield return new WaitForSeconds(20); //stop le code pendant 20 secondes
        StartCoroutine(StartInterQuestion());
    }

    /// <summary>
    /// Temps entre les question pendant lequel les résultats sont affichés
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartInterQuestion()
    {
        DisplayIsCorrectAnswer();

        yield return new WaitForSeconds(5); //stop le code pendant 5 secondes

        if (currentQuestion == nbQuestion - 1)
        {
            StartCoroutine(SetUpClassement());
            socket.Emit("resetVariables");
        }
        else
        {
            socket.Emit("getCurrentQuestion");
            ResetPlayersAnswer();
        }

    }

    /// <summary>
    /// Permet de remplir la Liste de sièges avec les objet populant la scène
    /// </summary>
    public void InitialiseChairs()
    {
        for (int i = 0; i < 18; i++)
        {
            chairs.Add(GameObject.Find("Slot"+(i +1)));
        }
    }

    /// <summary>
    /// permet d'initialiser les avatar en fonction du nombre de joueur
    /// </summary>
    public void InitialisePerso()
    {
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
        foreach (int i in playersHasAnswered.Keys)
        {
            chairs[i].GetComponent<MeshRenderer>().material = materials[0];
        }
    }

    /// <summary>
    /// Initialisation avant de commencer la partie
    /// </summary>
    /// <param name="nbP"></param>
    public void Init(int nbP)
    {
        SetupServerOn(); //Permet de gérer les écoutes du serveur
        DisplayHasAnswered();

        nbPlayer = nbP;

        nbQuestion = questions.Count;

        socket.Emit("getCurrentQuestion"); //permet de récupérer la question actuellement en cours

        //initialisation de la listes des reponses
        foreach (int i in players.Keys)
        {
            playersAnswer.Add(i, -1);
            playersHasAnswered.Add(i, -1);
        }

        InitialiseChairs(); //initialisation des sièges
        InitialisePerso();  //initialisation des avatars

        initialized = true;
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
    }

    List<int> sortbyScore()
    {
        List<int> result = new List<int>();
        int scoremax = 0;
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
