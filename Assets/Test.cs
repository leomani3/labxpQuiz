using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QuestionParser qp = new QuestionParser();

        List<Question> questions = qp.ParseTxt();
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].DebugQuestion();
        }
    }
}
