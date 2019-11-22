using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    QuestionParser qp;
    public static List<Question> questions;
    public List<GameObject> chairs;
    public Material[] materials;

    //--PLAYER
    private int playerId;
    private string playerName;

    //--PARTIE
    private int nbPlayer = 10;
    private int[] playersAnswer = new int[10];

    void Awake()
    {
        qp = new QuestionParser();
        questions = qp.ParseTxt();
        ResetPlayersAnswer();
        DisplayHasAnswered();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Permet à la fin d'une question d'afficher si les joueurs ont la bonne ou la mauvaise réponse
    /// </summary>
    private void DisplayIsCorrectAnswer()
    {
        for (int i = 0; i < nbPlayer; i++)
        {
            if (playersAnswer[i] == 0)
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[3];
            }
            else if(playersAnswer[i] == 1)
            {
                chairs[i].GetComponent<MeshRenderer>().material = materials[2];
            }
        }
    }

    /// <summary>
    /// Chnage la couleur du siège selon si le joueur a répondu ou non
    /// </summary>
    private void DisplayHasAnswered()
    {
        for (int i = 0; i < nbPlayer; i++)
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
        for (int i = 0; i < playersAnswer.Length; i++)
        {
            playersAnswer[i] = -1;
        }
    }
}
