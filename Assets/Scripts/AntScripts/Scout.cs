using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Scout : Agent
{
    [Header("Specific to ScoutAnt")]
    public Vector3 targetPosition;
    public GameObject antHill;
    public Vector2 ScreenResolution;

    private  int Radius=2;
    private GrassGenerator taskManager;
    private GameController gameController;
    private Vector2Int position;

    public bool is_full;

    public override void Initialize()
    {
        GetAreaPosition();
        taskManager = Camera.main.GetComponent<GrassGenerator>();
        gameController = Camera.main.GetComponent<GameController>();
        is_full = false;
        ScreenResolution = new Vector2(10.0f, 10.0f);
        
    }

    public override void OnEpisodeBegin()
    {  
        is_full = false;

        base.OnEpisodeBegin();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        if (!is_full)
            sensor.AddObservation(Vector3.Normalize(transform.position - targetPosition));
        else
            sensor.AddObservation(Vector3.Normalize(transform.position - antHill.transform.position));
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







        Vector3 a = Vector3.Normalize(transform.position - targetPosition);

        Vector3 b = Vector3.Normalize(transform.up);

        float dotProduct = Mathf.Abs(Vector3.Dot(a, b));

        AddReward(0.003f * dotProduct);

        AddReward(-0.001f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.gameObject.name);
        if ( !is_full&& collision.gameObject.name=="WarFog")
        {
            Debug.Log((Vector2Int)taskManager.fogTileMap.WorldToCell(targetPosition));
            taskManager.ClearFogInPosition((Vector2Int)taskManager.fogTileMap.WorldToCell(targetPosition));
            is_full = true;
            targetPosition = antHill.transform.position;
        }
        else if (is_full&& collision.gameObject.name=="Anthill")
        {
            is_full = false;
            GetAreaPosition();
        }
       
    }

    private void GetAreaPosition()
    {
        try
        {
            Debug.Log(taskManager.sortedFogTilesList[0]);
            position = taskManager.sortedFogTilesList[0];
            targetPosition =taskManager.fogTileMap.CellToWorld((Vector3Int)position );
            taskManager.sortedFogTilesList.Remove(position);
        }
        catch
        {
            Debug.Log("Area explored!");

        }
    }

}
