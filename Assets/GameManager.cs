﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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
    private bool gameStarted = false;
    private int nbQuestion = 0;

    //--SERVEUR
    private SocketIOComponent socket;

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

        ResetPlayersAnswer();
        DisplayHasAnswered();

        questions = new List<Question>();

        //--SERVEUR-------
        socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();

        socket.On("getCurrentQuestion", getCurrentQuestion);
        socket.On("setReponse", getIsCorrectAnswer);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            //TODO : faire ici la boucle de jeu
            //getCurrentQuestion
            //affichage de la question a l'écran
            //lancement du timer de la question
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

    public void Init(int nbP)
    {
        nbPlayer = nbP;

        nbQuestion = questions.Count;

        //initialisation de la liste des reponses
        foreach (int i in players.Keys)
        {
            playersAnswer.Add(i, -1);
        }
        gameStarted = true;
    }

    public void AddPlayer(int id, string n)
    {
        players.Add(id, n);
    }
}
