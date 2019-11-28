using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    QuestionParser qp;
    public static List<Question> questions;
    public List<GameObject> chairs;
    public Material[] materials;

    //--PLAYER
    private int playerId = 985;
    private string playerName;

    //--PARTIE
    private int nbPlayer = 10;
    private Dictionary<int, int> playersAnswer = new Dictionary<int, int>();
    private Dictionary<int, string> players = new Dictionary<int, string>();
    private int currentQuestion;
    private bool isCorrectAnswer = false;

    //--SERVEUR
    private SocketIOComponent socket;

    void Awake()
    {
        qp = new QuestionParser();
        questions = qp.ParseTxt();
        ResetPlayersAnswer();
        DisplayHasAnswered();


        //--SERVEUR-------
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

        socket.On("getQuestions", getQuestions);
        socket.On("getCurrentQuestion", getCurrentQuestion);
        socket.On("setReponse", getIsCorrectAnswer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            socket.Emit("getQuestions");
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

    private void getIsCorrectAnswer(SocketIOEvent e)
    {
        int pId = int.Parse(e.data.GetField("id").ToString());
        int pAnswer = int.Parse(e.data.GetField("answer").ToString());
        Debug.Log(pId+" "+pAnswer);
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

    private void getQuestions(SocketIOEvent e)
    {
        int nbQuestion = e.data.GetField("questions").Count;
        Debug.Log(e.data.GetField("questions"));
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
        foreach (int i in playersAnswer.Keys)
        {
            if (playersAnswer[i] != -1)
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
        }
    }
}
