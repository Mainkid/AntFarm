using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;


public class HedgehogAgent : Agent
{
    [Header("Specific to Ant")]
    public Vector2 ScreenResolution;
    public GameObject Ant;
    
    public override void Initialize()
    {

        Application.targetFrameRate = 30;
        ScreenResolution = new Vector2(10.0f, 10.0f);
        
    }

    
    public override void OnEpisodeBegin()
    {
        
        transform.localPosition = new Vector2(Random.Range(-ScreenResolution.x, ScreenResolution.x), Random.Range(-ScreenResolution.y, ScreenResolution.y));
        while (Vector2.Distance(Vector2.zero, transform.localPosition) < 1.5f)
        {
          transform.localPosition=new Vector2(Random.Range(-ScreenResolution.x, ScreenResolution.x), Random.Range(-ScreenResolution.y, ScreenResolution.y));
        }

        base.OnEpisodeBegin();
    }

    public void AntWinner()
    {
        AddReward(-10);
        EndEpisode();
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
        sensor.AddObservation(Vector3.Normalize(transform.position - Ant.transform.position));
        sensor.AddObservation(transform.localRotation.normalized);

    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        float rotation = 0.5f;
        float speed = 2.0f;
        float turnSpeed = 3.0f;

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


        AddReward(-0.001f);



        
    }

    private void Update()
    {
        //AddReward(-0.001f);
        //text.text = System.Convert.ToString(GetCumulativeReward());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "Ant")
        {

            AddReward(5);
            collision.gameObject.GetComponent<AntEscapeFromHedgehog>().AntIsEaten();
            EndEpisode();
        }
        else if (collision.gameObject.tag == "wall")
        {
            AddReward(-10);
            Ant.GetComponent<AntEscapeFromHedgehog>().EndEpisode();
            EndEpisode();
        }
       
    }

}
