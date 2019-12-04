using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestion : MonoBehaviour
{
    public GameObject AnswerPrefabs;
    public Text QuestionsUI;
    public int idQuestion = 0;

    List<GameObject> ListButtonAnswers;
    // Start is called before the first frame update
    void Start()
    {
        //generate first questions
        ListButtonAnswers = new List<GameObject>();
        GenerateAnswer(0);
        ListenerButtons();
    }

    void GenerateAnswer(int idQuestions)
    {
        Debug.Log(GameManager.questions.Count);
        QuestionsUI.text = GameManager.questions[idQuestion].GetEnonce();
        int nbAnswer = GameManager.questions[idQuestion].GetReponses().Count;
        List<string> Answers = GameManager.questions[idQuestion].GetReponses();
        int rightAnswer = GameManager.questions[idQuestions].GetBonneReponse();

        for (int i = 0; i < nbAnswer; i++)
        {
            Vector3 pos = new Vector3(647, 450 - (i * 70), 0);
            GameObject AnswerInstantiate = Instantiate(AnswerPrefabs, pos, Quaternion.identity);
            ListButtonAnswers.Add(AnswerInstantiate);
            AnswerInstantiate.GetComponent<AnswerButton>().goodAnswer = RightAnswer(rightAnswer,i);
            AnswerInstantiate.GetComponentInChildren<Text>().text = Answers[i];
            AnswerInstantiate.transform.parent = this.transform;
        }
    }
    void ListenerButtons()
    {
        foreach (GameObject g in ListButtonAnswers)
        {
            g.GetComponent<Button>().onClick.AddListener(
               () => ChangeQuestions());
        }
    }

    void ChangeQuestions()
    {
        //Destruction des buttons
        for (int i = 0; i < ListButtonAnswers.Count; i++)
            Destroy(ListButtonAnswers[i]);

        idQuestion++;

        GenerateAnswer(idQuestion);
    }

    bool RightAnswer(int rightAnswer, int idAnswer)
    {
        return rightAnswer == idAnswer;
    }
}
