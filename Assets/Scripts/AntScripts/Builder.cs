using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class Builder : Agent
{
    [Header("Specific to Ant")]
    public GameObject targetPosition;
    public GameObject materialParticle;
    public GameObject antHill;
    public Vector2 ScreenResolution;
    private CollectTasksManager taskManager;
    private GameController gameController;
    private SpriteRenderer materialSpriteRenderer;
    public bool is_full;

    public override void Initialize()
    {

        taskManager = Camera.main.GetComponent<CollectTasksManager>();
        gameController = Camera.main.GetComponent<GameController>();
        materialParticle.SetActive(false);
        is_full = false;
        ScreenResolution = new Vector2(10.0f, 10.0f);
        materialSpriteRenderer = materialParticle.GetComponent<SpriteRenderer>();
    }

    public override void OnEpisodeBegin()
    {
        GetFoodPosition();
       
        materialParticle.SetActive(false);
        is_full = false;

        

        base.OnEpisodeBegin();
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
        Debug.Log("BuilderCollision");

        if (taskManager.materialNamesList.Contains(collision.gameObject.tag ) && !is_full && collision.gameObject == targetPosition)
        {
            AddReward(10);
            is_full = true;
            materialParticle.SetActive(true);
            targetPosition.GetComponent<MaterialValues>().decrementValue();
            materialSpriteRenderer.sprite = targetPosition.GetComponent<MaterialValues>().materialSO.Particle;
            targetPosition = antHill;
        }
        else if (collision.gameObject.tag == "wall")
        {
            AddReward(-10);
            //Debug.Log("EndEpisode!");
            EndEpisode();
        }
        else if (collision.gameObject.tag == "Anthill" && is_full)
        {
            AddReward(10);
            GetFoodPosition();
            is_full = false;
            materialParticle.SetActive(false);
            gameController.ChangeMoneyValue(1);
        }
    }

    private void GetFoodPosition()
    {
        try
        {
            string randomMaterialType = taskManager.materialNamesList[Random.Range(0, taskManager.materialNamesList.Count)];
            Debug.Log(randomMaterialType);
            targetPosition = taskManager.materialsDictionary[randomMaterialType][Random.Range(0, taskManager.materialsDictionary[randomMaterialType].Count)];
            targetPosition.GetComponent<MaterialValues>().decrementPotentialValue();
            Debug.Log(targetPosition.transform.position);
        }
        catch
        {
            Debug.Log("Material ended");
            Debug.Log(taskManager.materialsDictionary);
            targetPosition = Camera.main.gameObject;
        }
    }
}
