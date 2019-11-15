using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question
{
    private string enonce;
    private List<string> reponses = new List<string>();
    private int bonneReponse;


    //--------GETTERS--------------
    public string GetEnonce()
    {
        return enonce;
    }

    public List<string> GetReponses()
    {
        return reponses;
    }

    public string GetReponseAt(int i)
    {
        return reponses[i];
    }

    public int GetBonneReponse()
    {
        return bonneReponse;
    }


    //------------SETTERS------------
    public void SetEnonce(string s)
    {
        enonce = s;
    }

    public void SetReponseAt(int i, string s)
    {
        reponses[i] = s;
    }

    public void SetReponses(List<string> sTab)
    {
        reponses = sTab;
    }

    public void SetBonneReponse(int i)
    {
        bonneReponse = i;
    }

    //-----------METHODES-------------
    public void AddReponse(string s)
    {
        reponses.Add(s);
    }

    public void DebugQuestion()
    {
        string debug = "";
        debug += "Question : "+this.GetEnonce() +"\nReponses : ";

        for (int i = 0; i < this.GetReponses().Count; i++)
        {
            debug += this.GetReponses()[i]+" ";
        }

        debug += "\nBonne reponse : " + this.GetBonneReponse();

        Debug.Log(debug);
    }

}
