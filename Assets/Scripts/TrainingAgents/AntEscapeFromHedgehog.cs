using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class AntEscapeFromHedgehog : Agent
{
    [Header("Specific to Ant")]
    public GameObject Hedgehog;
    public Vector2 ScreenResolution;

    public bool is_full;

    public override void Initialize()
    {
        
        Application.targetFrameRate = 30;
        ScreenResolution = new Vector2(10.0f, 10.0f);
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("Episode began");

        transform.localPosition = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
       

        while (Vector2.Distance(Hedgehog.transform.localPosition, transform.localPosition) < 1.5f)
        {
            Hedgehog.transform.localPosition = new Vector2(Random.Range(-ScreenResolution.x, ScreenResolution.x), Random.Range(-ScreenResolution.y, ScreenResolution.y));

        }

        base.OnEpisodeBegin();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> continiousActions = actionsOut.DiscreteActions;
        int forwardAction = 0;
        int turnAction = 0;
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2;
        }
        continiousActions[0] = forwardAction;
        continiousActions[1] = turnAction;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(Vector3.Normalize(transform.position - Hedgehog.transform.position));
        sensor.AddObservation(transform.localRotation.normalized);

    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        float rotation = 0.5f;
        float speed = 3.0f;
        float turnSpeed = 5.0f;

        if (vectorAction.DiscreteActions[0] == 0)
        {
            transform.position += transform.up * speed * Time.fixedDeltaTime;
        }
        else if (vectorAction.DiscreteActions[0] == 1)
        {
            //transform.position -= transform.up * speed* Time.fixedDeltaTime;
        }

        if (vectorAction.DiscreteActions[1] == 1)
        {
            transform.Rotate(new Vector3(0, 0, 1) * turnSpeed);
        }
        else if (vectorAction.DiscreteActions[1] == 0)
        {
            transform.Rotate(new Vector3(0, 0, -1) * turnSpeed);
        }
        AddReward(+0.005f);

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "wall")
        {
            AddReward(-10);
            Hedgehog.GetComponent<HedgehogAgent>().EndEpisode();
            //Debug.Log("EndEpisode!");
            EndEpisode();
        }
    }

    public void AntIsEaten()
    {
        AddReward(-5.0f);
        EndEpisode();
    }

}
