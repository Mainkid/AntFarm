using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class AntMoveToGoal : Agent
{
    [Header("Specific to Ant")]
    public GameObject targetPosition;
    public GameObject foodParticle;
    public GameObject antHill;
    public Vector2 ScreenResolution;
    private CollectTasksManager taskManager;
    private GameController gameController;
    private SpriteRenderer foodSpriteRenderer;
    public bool is_full;

    public override void Initialize()
    {
        
        taskManager = Camera.main.GetComponent<CollectTasksManager>();
        gameController = Camera.main.GetComponent<GameController>();
        foodParticle.SetActive(false);
        is_full = false;
        ScreenResolution = new Vector2(10.0f, 10.0f);
        foodSpriteRenderer = foodParticle.GetComponent<SpriteRenderer>();
    }

    public override void OnEpisodeBegin()
    {

        GetFoodPosition();
        foodParticle.SetActive(false);
        is_full = false;
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
        continiousActions[0]= forwardAction;
        continiousActions[1] = turnAction;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!is_full)
            sensor.AddObservation(Vector3.Normalize(transform.position - targetPosition.transform.position));
        else
            sensor.AddObservation(Vector3.Normalize(transform.position - antHill.transform.position));
        sensor.AddObservation(transform.localRotation.normalized);
        
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        float rotation = 0.5f;
        float speed = 2.0f;
        float turnSpeed = 2.0f;

        if (vectorAction.DiscreteActions[0] == 0)
        {
            transform.position += transform.up  * speed* Time.fixedDeltaTime;
        }
        else if (vectorAction.DiscreteActions[0]==1)
        {
            //transform.position -= transform.up * speed* Time.fixedDeltaTime;
        }
        
        if (vectorAction.DiscreteActions[1]==1)
        {
            transform.Rotate(new Vector3(0, 0, 1) * turnSpeed);
        }
        else if (vectorAction.DiscreteActions[1]==0)
        {
            transform.Rotate(new Vector3(0, 0, -1) * turnSpeed);
        }






       
        Vector3 a = Vector3.Normalize(transform.position - targetPosition.transform.position);
      
        Vector3 b = Vector3.Normalize(transform.up);

        float dotProduct = Mathf.Abs(Vector3.Dot(a, b));

        AddReward(0.003f*dotProduct);

        AddReward(-0.001f);

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food" && !is_full&&collision.gameObject==targetPosition)
        {
            AddReward(10);
            //EndEpisode();
            is_full = true;
            foodParticle.SetActive(true);
            targetPosition.GetComponent<FoodValues>().decrementValue();
            foodSpriteRenderer.sprite = targetPosition.GetComponent<FoodValues>().foodSO.Particle;
            //Debug.Log("Reward!");
            targetPosition = antHill;
        }
        else if (collision.gameObject.tag== "wall")
        {
            AddReward(-10);
            //Debug.Log("EndEpisode!");
            EndEpisode();
        }
        else if (collision.gameObject.tag== "Anthill" && is_full)
        {
            AddReward(10);
            GetFoodPosition();
            is_full = false;
            foodParticle.SetActive(false);
            gameController.ChangeMoneyValue(1);
        }
    }

    private void GetFoodPosition()
    {
        try
        {
            targetPosition = taskManager.foodList[Random.Range(0, taskManager.foodList.Count)];
            targetPosition.GetComponent<FoodValues>().decrementPotentialValue();
            Debug.Log(targetPosition.transform.position);
        }
        catch
        {
            Debug.Log("Food ended");
            targetPosition = Camera.main.gameObject;
        }
    }
   
   
}
