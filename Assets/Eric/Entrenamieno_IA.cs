using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Entrenamieno_IA : MonoBehaviour
{
    Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Objetivo;


    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        { 
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }

    public override void CollectObservations(VectorSensor sensor) 
    {
        sensor.AddObservation(Objetivo.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float multiplicador = 10;
    public override void OnAcitionReceived(ActionBuffers actions) 
    {
        base.OnActionReceived(actions);
    }

}
