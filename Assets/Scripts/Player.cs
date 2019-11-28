using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public int id;
    public string name;
    public int score;


    public Player(int id, string name)
    {
        this.id = id;
        this.name = name;
        score = 0;
    }


}
