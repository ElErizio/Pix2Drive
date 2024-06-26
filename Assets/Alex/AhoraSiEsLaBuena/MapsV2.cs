using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsV2 : MonoBehaviour
{
    public GameObject player;
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    public int nextCorrectCheckPoint;
    public string checkpointTag;
    public string badCheckpointTag;

    public bool reverse;
    private void Start()
    {
        if (reverse)
        {
            ReverseCheckpoints();
        }
        else
        {
            Restart();
        }
    }

    public void ReverseCheckpoints()
    {
        List<CheckPoint> tempList = new List<CheckPoint>();
        for (int i = 0; i < checkPoints.Count; i++)
        {
            tempList.Insert(0, checkPoints[i]);
        }
        checkPoints = tempList;
        Restart();
    }

    public void Restart()
    {
        nextCorrectCheckPoint = 0;
        foreach (CheckPoint c in checkPoints)
        {
            c.SetMaps(this);
        }
        NextObjective();
    }
    public GameObject GetNextCheckPoint()
    {
        return checkPoints[nextCorrectCheckPoint].gameObject;
    }
    public void NextObjective()
    {
        foreach(CheckPoint c in checkPoints)
        {
            c.gameObject.SetActive(true);
            c.tag = badCheckpointTag;
        }
        checkPoints[nextCorrectCheckPoint].tag = checkpointTag;

        int lastCheckPoint = nextCorrectCheckPoint-1 < 0 ? checkPoints.Count-1 : nextCorrectCheckPoint - 1;
        checkPoints[lastCheckPoint].gameObject.SetActive(false);
    }

    public void CheckPointCollected(CheckPoint checkPoint)
    {
        if(nextCorrectCheckPoint == checkPoints.IndexOf(checkPoint))
        {
            nextCorrectCheckPoint = (nextCorrectCheckPoint+1)%checkPoints.Count;
            try
            {

                player.GetComponent<IA_ManejoV3>().CorrectCheckpoint();
            }
            catch
            {
                try
                {

                    player.GetComponent<IA_ManejoSemaforo>().CorrectCheckpoint();
                }
                catch
                {
                    print("nomas no es un player");
                }
            }
            NextObjective();
            print("Checkpoint");
        }
        else
        {
            try
            {

                player.GetComponent<IA_ManejoV3>().WrongCheckpoint();
            }
            catch
            {
                try
                {

                    player.GetComponent<IA_ManejoSemaforo>().WrongCheckpoint();
                }
                catch
                {
                    print("nomas no es un player");
                }
            }
            print("Sentido Contrario");
        }
    }
}
