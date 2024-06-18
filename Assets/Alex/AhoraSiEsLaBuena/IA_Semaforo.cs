using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(AlexInputSystem))]
public class IA_Semaforo : Agent
{

    Rigidbody rBody;
    AlexInputSystem alexInputSystem;
    public MapsV2 maps;

    CarController carController;

    Vector3 initialPos;
    Vector3 initialRot;

    public float score;

    bool reset = false;

    public bool stop;

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
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = initialPos;
            this.transform.localEulerAngles = initialRot;
        }

        maps.NextObjective();

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Vector3.Distance(maps.GetNextCheckPoint().transform.position, transform.position));//1 observacion

        //la velocidad del agente
        sensor.AddObservation(rBody.velocity.x);//1 observacion
        sensor.AddObservation(rBody.velocity.z);//1 observacion

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        
        carController.SetInput(actions.ContinuousActions[1], actions.ContinuousActions[0], false);

        
        score = GetCumulativeReward();

        if(stop)
        {
            if (rBody.velocity.magnitude > 0.2f)
            {
                SetReward(-0.01f);
            }
            else
            {
                SetReward(0.005f);
            }
        }
        else
        {
            if (rBody.velocity.magnitude < 0.2f)
            {
                SetReward(-0.01f);
            }
            else
            {
                SetReward(0.005f);
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
