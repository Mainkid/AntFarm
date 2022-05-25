using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Values : MonoBehaviour
{
    protected int Value;
    protected int potentialValue;
    protected CollectTasksManager taskManager;
    //[SerializeField]
    //public FoodScriptableObject foodSO;
    protected float spriteStep = 0;
    protected SpriteRenderer spriteRenderer;
    protected GameObject valueText;
    protected Animator animatorText;


    protected virtual void Awake()
    {
        valueText = transform.Find("Value").gameObject;
        animatorText = valueText.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        //Value = Parameters.foodPrice[foodSO.foodName];
        potentialValue = Value;

        //spriteStep = Value * 1.0f / foodSO.foodSprites.Count;
        Debug.Log(spriteStep);
    }

    protected virtual void Start()
    {
        taskManager = Camera.main.GetComponent<CollectTasksManager>();
    }

    public virtual void decrementValue()
    {
        Value--;
        animatorText.Play("ValueShow");
        
    }

    public virtual void decrementPotentialValue()
    {

        potentialValue--;
       

    }
}
