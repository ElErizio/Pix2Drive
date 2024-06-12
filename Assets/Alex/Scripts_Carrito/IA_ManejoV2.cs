using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AlexInputSystem))]
public class IA_ManejoV2 : Agent
{

    Rigidbody rBody;
    AlexInputSystem alexInputSystem;

    public Maps maps;
    CarController carController;
    RotationCounter rot;
    public Transform groundChecker;
    public LayerMask manejable;
    public LayerMask noManejable;
    public float dondeEstoyManejando;

    public int fuerza = 100;
    int mult = 1;

    public int objs = 0;

    float distanciaAnterior;
    bool reset = false;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rot = GetComponent<RotationCounter>();
        carController = GetComponent<CarController>();
        alexInputSystem = GetComponent<AlexInputSystem>();
        transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    public Transform Objetivo;
    public override void OnEpisodeBegin()
    {
        //this.rBody.angularVelocity = Vector3.zero;
        //this.rBody.velocity = Vector3.zero;
        //si te caes este va a ser tu punto de inicio
        if (this.transform.localPosition.y < 0 || reset == true)
        {
            if(reset == true)
            {
                reset = false;
                maps.Restart();
            }
            rot.ReStart();
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.eulerAngles = new Vector3(0, 180, 0);
            

        }

        maps.NextObjective();


        //FASE 1 Linea Recta
        /* if(objs < 20)
         {
             transform.localPosition = new Vector3(0, 0.5f, 0);
             transform.eulerAngles = new Vector3(0, 0, 0);
             Objetivo.localPosition = new Vector3(Random.Range(-2, 2), 0.5f, 15); ;

         }else if(objs < 40)
         {

             mult *= -1;
             //transform.localPosition = new Vector3(0, 0.5f, 0);
             Objetivo.localPosition = new Vector3(Random.Range(10 * mult, 20 * mult), 0.5f, Random.Range(10 * mult, 20 * mult));
         }
         else
         {*/
        //mult *= -1;
        //mover el objetivo dentro del plano de manera aleatoria
        //Objetivo.localPosition = new Vector3(Random.Range(-20, 20), 0.5f, Random.Range(-20, 20));
        //}
        distanciaAnterior = Vector3.Distance(transform.position, Objetivo.transform.position);
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

        sensor.AddObservation(dondeEstoyManejando); //1 observacion
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        /*
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        rBody.velocity = new Vector3(controlSignal.x * fuerza, rBody.velocity.y, controlSignal.z * fuerza);
        */
        if(actions.ContinuousActions[0] < 0)
        {
            SetReward(-0.05f);
        }
        carController.SetInput(actions.ContinuousActions[1], actions.ContinuousActions[0], false);


        bool carr = Physics.CheckSphere(groundChecker.position, 0.1f, manejable);
        bool banqueta = Physics.CheckSphere(groundChecker.position, 0.1f, noManejable);

        if (banqueta)
        {
            SetReward(-0.25f);
            print("NO");
            dondeEstoyManejando = 2;
        }
        else if (carr)
        {
            print("SI");
            dondeEstoyManejando = 1;
        }
        else
        {
            dondeEstoyManejando = 0;
        }

        if(rot.ObtenerVueltas() > 1 || rot.ObtenerVueltas() < -1)
        {
            
            SetReward(-1.0f);
            print("Muchas vueltas");
            reset = true;
            EndEpisode();
        }

        if (this.transform.localPosition.y < -0.3f)
        {

            print("C cae");
            SetReward(-2.0f);
            reset = true;
            EndEpisode();
        }

        SetReward(-0.008f);

        if (distanciaAnterior > Vector3.Distance(transform.position, Objetivo.transform.position))
        {
            SetReward(0.01f);
        }
        else
        {
            SetReward(-0.03f);
        }

        distanciaAnterior = Vector3.Distance(transform.position, Objetivo.transform.position);


        if (GetCumulativeReward() < -10f)
        {
            print("aaaaa");
            transform.eulerAngles = new Vector3(0, 180, 0);
            objs = 0;
            SetReward(-2f);
            maps.Restart();

            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var conti = actionsOut.ContinuousActions;
        conti[0] = alexInputSystem.GetVerticalAxis();
        conti[1] = alexInputSystem.GetHorizontalAxis();
    }

    public void CollisionConObjetivo()
    {
        print("objetivo");
        SetReward(2.0f);
        rot.ReStart();
        objs++;
        EndEpisode();
    }

    public void CollisionTrasera()
    {
        print("Con las nalgas");
        SetReward(-3.0f);
        reset = true;
        EndEpisode();
    }
}
