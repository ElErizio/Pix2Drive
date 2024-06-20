using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semaforo : MonoBehaviour
{
    public bool stateStop;
    public Material green;
    public Material red;
    public GameObject checkPointSemaforo;
    public int timeToChange;
    //public string badCheckpointTag;

    public GameObject areaColl;

    GameObject player;

    public int minSecs, maxSecs;

    private void Start()
    {
        ChangeState();
    }
    private void Update()
    {
        timeToChange--;
        if(timeToChange <= 0)
        {
            ChangeState();

        }
    }

    private void ChangeState()
    {
        stateStop = !stateStop;
        areaColl.SetActive(stateStop);
        if (stateStop)
        {
            //checkPointSemaforo.tag = badCheckpointTag;
            checkPointSemaforo.GetComponent<Renderer>().material = red;
        }
        else
        {
            //checkPointSemaforo.tag = badCheckpointTag;
            checkPointSemaforo.GetComponent<Renderer>().material = green;
        }
        if (player)
        {
            player.GetComponent<IA_ManejoSemaforo>().SetStop(stateStop);

        }
        timeToChange = Random.Range(minSecs, maxSecs);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Entra el player");
            player = other.gameObject;
            player.GetComponent<IA_ManejoSemaforo>().SetStop(stateStop);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(stateStop == true)
            {

                other.GetComponent<IA_ManejoSemaforo>().SalirSemaforoReversa(false);
            }
            else
            {
                other.GetComponent<IA_ManejoSemaforo>().SetStop(false);

            }
            player = null;

        }
    }
}
