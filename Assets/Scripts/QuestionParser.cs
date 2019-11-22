using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestionParser
{
    public List<Question> ParseTxt()
    {
        List<Question> questions = new List<Question>();

        string filename = "Assets/Questions.txt";
        string line = "";
        StreamReader sr = new StreamReader(filename);

        int nbQuestion = int.Parse(sr.ReadLine()); //On lit le nombre de question

        for (int i = 0; i < nbQuestion; i++)
        {
            Question q = new Question();

            q.SetEnonce(sr.ReadLine()); //lit l'ennoncé

            int nbReponse = int.Parse(sr.ReadLine()); //lit de nombre de reponse
            for (int j = 0; j < nbReponse; j++)
            {
                q.AddReponse(sr.ReadLine()); //lit les reponses
            }

            q.SetBonneReponse(int.Parse(sr.ReadLine())); //lit la bonne reponse
            sr.ReadLine(); //on lit une ligne de plus pour passer le saut de ligne entre les questions

            questions.Add(q);
        }
        return questions;
    }
}
