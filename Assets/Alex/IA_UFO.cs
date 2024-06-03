using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AlexInputSystem))]
public class IA_UFO : Agent
{
    Rigidbody rBody;
    AlexInputSystem alexInputSystem;



    public int fuerza = 100;
    int mult = 1;

    public int objs = 0;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        alexInputSystem = GetComponent<AlexInputSystem>();
    }

    public Transform Objetivo;
    public override void OnEpisodeBegin()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        //si te caes este va a ser tu punto de inicio
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);

        }




        //FASE 1 Linea Recta
        if(objs < 20)
        {
            transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);
            Objetivo.localPosition = new Vector3(Random.Range(-2, 2), 0.5f, 15); ;

        }else if(objs < 40)
        {
            transform.localPosition = new Vector3(0, 0.5f, 0);
            Objetivo.localPosition = new Vector3(0, 0.5f, Random.Range(10 * mult, 25 * mult));
        }
        else
        {
            mult *= -1;
            //mover el objetivo dentro del plano de manera aleatoria
            Objetivo.localPosition = new Vector3(Random.Range(0, -20), 0.5f, Random.Range(10 * mult, 25 * mult));
        }
    }

    //funcion para programar los sensores
    public override void CollectObservations(VectorSensor sensor)
    {
        //el agente sepa la posicion del objetivo
        sensor.AddObservation(Objetivo.localPosition); //3 observaciones
        sensor.AddObservation(this.transform.localPosition); //3 observaciones

        //la velocidad del agente
        sensor.AddObservation(rBody.velocity.x);//1 observacion
        sensor.AddObservation(rBody.velocity.z);//1 observacion
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        print(controlSignal);
        rBody.AddForce(controlSignal * fuerza);


        if (this.transform.localPosition.y < -0.3f)
        {
            SetReward(-2.0f);
            EndEpisode();
        }

        SetReward(-0.01f);

        if(GetCumulativeReward() < -3f)
        {
            objs = 0;
            SetReward(-2f);
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var conti = actionsOut.ContinuousActions;
        conti[0] = Input.GetAxis("Horizontal");
        conti[1] = Input.GetAxis("Vertical");
    }

    public void CollisionConObjetivo()
    {
        SetReward(2.0f);
        objs++;
        EndEpisode();
    }
}
