using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    QuestionParser qp;
    public static List<Question> questions;

    void Awake()
    {
        qp = new QuestionParser();
        questions = qp.ParseTxt();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
