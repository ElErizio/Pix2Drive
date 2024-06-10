using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maps : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject cubito;
    public int actualpos;
    bool reverse;

    public void Restart()
    {
        /*int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            reverse = false;
        }
        else
        {
            reverse = true;
        }*/
        reverse = false;
        actualpos = -1;
    }
    public void NextObjective()
    {
        if (reverse)
        {
            actualpos--;
        }
        else
        {
            actualpos++;
        }

        if (actualpos >= spawnPoints.Length || actualpos < 0)
        {
            reverse = !reverse;
            //actualpos = 0;
            if (reverse)
            {
                actualpos--;
            }
            else
            {
                actualpos++;
            }
            NextObjective();
            return;
        }
        cubito.transform.position = spawnPoints[actualpos].transform.position;
        
    }
}
