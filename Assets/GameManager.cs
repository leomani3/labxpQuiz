using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    QuestionParser qp;
    public static List<Question> questions;
    public List<GameObject> chairs;
    public Material[] materials;
    public GameObject AnswerPrefabs;
    public bool gameStarted = false;

    //--PLAYER
    private int playerId = 985;
    private string playerName;

    //--PARTIE
    private int nbPlayer = 10;
    private Dictionary<int, int> playersAnswer = new Dictionary<int, int>();
    private Dictionary<int, int> playersHasAnswered = new Dictionary<int, int>();
    private Dictionary<int, string> players = new Dictionary<int, string>();
    private int currentQuestion;
    private bool isCorrectAnswer = false;
    private int nbQuestion = 0;
    private Text QuestionsUI;
    private List<GameObject> ListButtonAnswers;

    //--SERVEUR
    private SocketIOComponent socket;
    private bool initialized = false;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        chairs = new List<GameObject>();

        ResetPlayersAnswer();
        DisplayHasAnswered();

        questions = new List<Question>();
        ListButtonAnswers = new List<GameObject>();
        //--SERVEUR-------
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

        socket.On("getCurrentQuestion", getCurrentQuestion);
        socket.On("setReponse", getIsCorrectAnswer);
        socket.On("responded", aPlayerResponded);
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "MainStage")
        {
            if (!initialized) //one ne met dans ce if que les choses qu'on ne veut faire qu'une seule fois
            {
                InitialiseChairs();
                Init(nbPlayer);
                DisplayHasAnswered();
                //TODO : faire ici la boucle de jeu
                socket.On("getCurrentQuestion", getCurrentQuestion);
                Debug.Log(SceneManager.GetActiveScene().name);
                QuestionsUI = GameObject.Find("Question").GetComponent<Text>();
                QuestionsUI.text = questions[currentQuestion].GetEnonce();

                //lancement du timer de la question
                //les differentes questions 
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
                initialized = true;
            }
            //envoyer SendReponse(int i) à chaque fois que le joueur appuis sur un bouton de reponse
            //pendant que les joueurs répondent Lancer DisplayHasAnswered() à chaque update
            //à la fin du timer afficher les reponses de jouer = DisplayIsCorrectAnswer()
            //lancement du timer inter-question
            //à la fin du timer inter-question (environ 5s) appeler ResetPlayersAnswer()
            //tester si c'est la fin du jeu
            //affichage des score = La fonction est pas encore faite. Ni l'écran d'affichage
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            socket.Emit("getCurrentQuestion");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SendReponse(1);
        }
    }

    public SocketIOComponent GetSocket()
    {
        return GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    }

    public void SetPlayerId(int i)
    {
        playerId = i;
    }

    public void InitialiseChairs()
    {
        Debug.Log("Chairs initalisées");
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Chair");
        Debug.Log("LENGTH : " + tmp.Length);
        for (int i = 0; i < tmp.Length; i++)
        {
            chairs.Add(tmp[i]);
        }
    }

    private void aPlayerResponded(SocketIOEvent e)
    {
        int pId = int.Parse(e.data.GetField("id").ToString());
        Debug.Log("UN PLAYER A REPONDU id : " +pId);
        playersHasAnswered[pId] = 1;
    }

    public void SetHasAnswered()
    {
        Debug.Log("ID : "+playerId);
        playersHasAnswered[playerId] = 1;
        DisplayHasAnswered();
    }

    private void getIsCorrectAnswer(SocketIOEvent e)
    {
        int pId = int.Parse(e.data.GetField("id").ToString());
        int pAnswer = int.Parse(e.data.GetField("answer").ToString());
        playersAnswer[pId] = pAnswer;
    }

    private void SendReponse(int i)
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("id", playerId);
        j.AddField("answer", i);

        socket.Emit("setReponse", j);
    }

    private void getCurrentQuestion(SocketIOEvent e)
    {
        currentQuestion = int.Parse(e.data.GetField("question").ToString());
    }

    /// <summary>
    /// Permet à la fin d'une question d'afficher si les joueurs ont la bonne ou la mauvaise réponse
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
    /// Chnage la couleur du siège selon si le joueur a répondu ou non
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

    /// <summary>
    /// Permet de reset le tableau des réponse de façon à de nouveau dire qu'aucun joueur a répondu
    /// </summary>
    private void ResetPlayersAnswer()
    {
        foreach (int i in playersAnswer.Keys)
        {
            playersAnswer[i] = -1;
            playersHasAnswered[i] = -1;
        }
    }

    public void Init(int nbP)
    {
        nbPlayer = nbP;

        nbQuestion = questions.Count;

        //initialisation de la listes des reponses
        foreach (int i in players.Keys)
        {
            playersAnswer.Add(i, -1);
            playersHasAnswered.Add(i, -1);
        }
    }

    public void AddPlayer(int id, string n)
    {
        players.Add(id, n);
    }

    bool RightAnswer(int rightAnswer, int idAnswer)
    {
        return rightAnswer == idAnswer;
    }

    public int GetPlayerId()
    {
        return playerId;
    }
}
