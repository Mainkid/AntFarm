using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodValues : Values
{

    [SerializeField]
    public CollectableScriptableObject foodSO;
    

    protected override void Awake()
    {
        base.Awake();

        Value = Parameters.foodPrice[foodSO.Name];
        potentialValue = Value;
        spriteStep = Value*1.0f / foodSO.Sprites.Count;
        Debug.Log(spriteStep);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void decrementValue()
    {
        base.decrementValue();
        if (Value == 0)
        {

            Destroy(gameObject, 0.5f);
        }
        else
        {
            Debug.Log(Mathf.CeilToInt(Value / spriteStep) - 1);
            spriteRenderer.sprite = foodSO.Sprites[Mathf.CeilToInt(Value / spriteStep) - 1];

        }
    }

    public override void decrementPotentialValue()
    {

        base.decrementPotentialValue();
        if (potentialValue == 0)
        {
            taskManager.deleteFoodFromList(this.gameObject);
        }

    }
}
