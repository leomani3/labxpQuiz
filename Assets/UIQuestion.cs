using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestion : MonoBehaviour
{
    public GameObject AnswerPrefabs;
    public Text QuestionsUI;
    public int idQuestion = 0;

    QuestionParser qp;
    List<Question> questions;
    List<GameObject> ListButtonAnswers;
    // Start is called before the first frame update
    void Start()
    {
        //generate first questions
        ListButtonAnswers = new List<GameObject>();
        qp = new QuestionParser();
        questions = qp.ParseTxt();
        GenerateAnswer(0);
        ListenerButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateAnswer(int idQuestions)
    {
        QuestionsUI.text = questions[idQuestion].GetEnonce();
        int nbAnswer = questions[idQuestions].GetReponses().Count;
        List<string> Answers = questions[idQuestions].GetReponses();

        for (int i = 0; i < nbAnswer; i++)
        {
            Vector3 pos = new Vector3(647, 450 - (i * 70), 0);
            GameObject AnswerInstantiate = Instantiate(AnswerPrefabs, pos, Quaternion.identity);
            ListButtonAnswers.Add(AnswerInstantiate);
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


}
