

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class AntMoveToTarget : Agent
{
    [Header("Specific to Ant")]
    public GameObject targetPosition;
    public GameObject foodParticle;
    public GameObject antHill;
    public Vector2 ScreenResolution;
    private CollectTasksManager taskManager;
    private GameController gameController;
    public bool is_full;

    public override void Initialize()
    {
        taskManager = Camera.main.GetComponent<CollectTasksManager>();
        gameController = Camera.main.GetComponent<GameController>();
        foodParticle.SetActive(false);
        is_full = false;
        ScreenResolution = new Vector2(10.0f, 10.0f);
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("Episode began");
      
        transform.localPosition= new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        //antHill.transform.localPosition = new Vector2(Random.Range(-12.0f, 12.0f), Random.Range(-12.0f, 12.0f));
        targetPosition.transform.localPosition = new Vector2(Random.Range(-ScreenResolution.x, ScreenResolution.x), Random.Range(-ScreenResolution.y, ScreenResolution.y));
        transform.localPosition = new Vector2(0,0);
        foodParticle.SetActive(false);
        is_full = false;

        while (Vector2.Distance(targetPosition.transform.localPosition, transform.localPosition) < 1.5f)
        {
            targetPosition.transform.localPosition = new Vector2(Random.Range(-ScreenResolution.x, ScreenResolution.x), Random.Range(-ScreenResolution.y, ScreenResolution.y));

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
        
        sensor.AddObservation(Vector3.Normalize(transform.position - targetPosition.transform.position));
        sensor.AddObservation(transform.localRotation.normalized);

    }
    
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        float rotation = 0.5f;
        float speed = 5.0f;
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







        Vector3 a = Vector3.Normalize(transform.position - targetPosition.transform.position);

        Vector3 b = Vector3.Normalize(transform.up);

        float dotProduct = Mathf.Abs(Vector3.Dot(a, b));

        AddReward(0.003f * dotProduct);

        AddReward(-0.001f);

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food" && !is_full)
        {
            AddReward(10);
            EndEpisode();
        }
        else if (collision.gameObject.tag == "wall")
        {
            AddReward(-10);
            EndEpisode();
        }
      
    }

   


}

