using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(AlexInputSystem))]
public class IA_Manejo : Agent
{
    Rigidbody rBody;
    AlexInputSystem alexInputSystem;
    CarController carController;

    Vector3 currentRotation;
    int mult = 1;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        alexInputSystem = GetComponent<AlexInputSystem>();
        carController = GetComponent<CarController>();
    }

    public Transform Objetivo;
    public override void OnEpisodeBegin()
    {
        //si te caes este va a ser tu punto de inicio
        if (this.transform.localPosition.y < 0 || Mathf.Abs(currentRotation.x) > 80 || Mathf.Abs(currentRotation.z) > 80)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.eulerAngles = new Vector3(0, currentRotation.y, 0);

        }

        mult *= -1;
        //mover el objetivo dentro del plano de manera aleatoria
        Objetivo.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.Range(10*mult,25*mult));
    }

    //funcion para programar los sensores
    public override void CollectObservations(VectorSensor sensor)
    {
        //el agente sepa la posicion del objetivo
        sensor.AddObservation(Objetivo.localPosition); //3 observaciones
        sensor.AddObservation(this.transform.localPosition); //3 observaciones
        sensor.AddObservation(currentRotation); //3 observaciones

        //la velocidad del agente
        sensor.AddObservation(rBody.velocity.x);//1 observacion
        sensor.AddObservation(rBody.velocity.z);//1 observacion
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        /*//programar 2 actuadores
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];*/
        carController.SetInput(actions.ContinuousActions[0], actions.ContinuousActions[1], false);

        //programar las politicas
        /*float distanciaobjetivo = Vector3.Distance(this.transform.localPosition, Objetivo.localPosition);

        //politica para cuando el agente agarre al objetivo
        if (distanciaobjetivo < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }*/
        //politica en caso de que el agente sea tan pendejo y se caiga

        if (this.transform.localPosition.y < 0)
        {
            SetReward(-2.0f);
            EndEpisode();
        }

        currentRotation = transform.eulerAngles;

        // Asegurarse de que los ángulos estén en el rango -180 a 180 grados
        if (currentRotation.x > 180)
        {
            currentRotation.x -= 360;
        }
        if (currentRotation.z > 180)
        {
            currentRotation.z -= 360;
        }

        // Verificar si la rotación en el eje X es mayor que 80 grados en valor absoluto
        if (Mathf.Abs(currentRotation.x) > 80 || Mathf.Abs(currentRotation.z) > 80)
        {
            SetReward(-2f);
            EndEpisode();
        }
        SetReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var conti = actionsOut.ContinuousActions;
        conti[0] = alexInputSystem.GetHorizontalAxis();
        conti[1] = alexInputSystem.GetVerticalAxis();
    }

    public void CollisionDelantera()
    {
        SetReward(1.0f);
        EndEpisode();
    }
    public void CollisionTrasera()
    {
        SetReward(-1.0f);
        EndEpisode();
    }
}
