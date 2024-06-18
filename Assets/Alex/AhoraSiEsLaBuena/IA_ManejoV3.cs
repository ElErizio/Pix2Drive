using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AlexInputSystem))]
public class IA_ManejoV3 : Agent
{

    Rigidbody rBody;
    AlexInputSystem alexInputSystem;
    public MapsV2 maps;

    CarController carController;

    Vector3 initialPos;
    Vector3 initialRot;

    public float score;

    bool reset = false;

    int objs;

    public float timeAFK;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        carController = GetComponent<CarController>();
        alexInputSystem = GetComponent<AlexInputSystem>();
        initialPos = transform.localPosition;
        initialRot = transform.localEulerAngles;
        timeAFK = 0;
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0 || reset == true)
        {
            reset = false;
            maps.Restart();
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = initialPos;
            if (objs > 100)
            {
                initialRot = initialRot * -1;
                maps.reverse = !maps.reverse;
                maps.ReverseCheckpoints();
            }
            this.transform.localEulerAngles = initialRot;
            timeAFK = 0;


        }

        maps.NextObjective();

    }

    //funcion para programar los sensores
    public override void CollectObservations(VectorSensor sensor)
    {

        //sensor.AddObservation(this.transform.localPosition); //3 observaciones
        sensor.AddObservation(Vector3.Distance(maps.GetNextCheckPoint().transform.position, transform.position));

        //la velocidad del agente
        sensor.AddObservation(rBody.velocity.x);//1 observacion
        sensor.AddObservation(rBody.velocity.z);//1 observacion

        //sensor.AddObservation(transform.localEulerAngles.y);//1 observacion
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);


        /*if(actions.ContinuousActions[0] < 0)
        {
            print("Chale");
            SetReward(-0.1f);

        }else*/ if (actions.ContinuousActions[0] > 0)
        {
            SetReward(0.001f);
        }
        carController.SetInput(actions.ContinuousActions[1], actions.ContinuousActions[0], false);


        SetReward(-0.002f);

        score = GetCumulativeReward();

        /*if(rBody.velocity.magnitude > 0.4 && actions.ContinuousActions[0] > 0)
        {
            SetReward(0.01f);
        }
        else
        {
            SetReward(-0.01f);
        }*/

        if(rBody.velocity.magnitude < 0.3)
        {
            timeAFK++;
            SetReward(-0.001f);
            if (timeAFK > 1000)
            {
                print("AFK");
                SetReward(-4f - Vector3.Distance(maps.GetNextCheckPoint().transform.position, transform.position));
                reset = true;
                EndEpisode();
            }
        }

        if (GetCumulativeReward() < -6f)
        {
            print("aaaaa");
            SetReward(-2f);
            reset = true;
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var conti = actionsOut.ContinuousActions;
        conti[0] = alexInputSystem.GetVerticalAxis();
        conti[1] = alexInputSystem.GetHorizontalAxis();
    }

    public void CorrectCheckpoint()
    {
        SetReward(2f);
        objs++;

    }
    public void WrongCheckpoint()
    {
        SetReward(-2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NoManejable"))
        {
            SetReward(-4f - Vector3.Distance(maps.GetNextCheckPoint().transform.position, transform.position));
            reset = true;
            EndEpisode();
            print("Chale");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("NoManejable"))
        {
            SetReward(-0.1f);
            print("Chale");
        }
    }
}
