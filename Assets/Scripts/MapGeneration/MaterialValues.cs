using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialValues : Values
{
    [SerializeField]
    public CollectableScriptableObject materialSO;

    protected override void Awake()
    {
        base.Awake();

        Value = Parameters.materialPrice[materialSO.Name];
        potentialValue = Value;
        spriteStep = Value * 1.0f / materialSO.Sprites.Count;
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
            spriteRenderer.sprite = materialSO.Sprites[Mathf.CeilToInt(Value / spriteStep) - 1];

        }
    }

    public override void decrementPotentialValue()
    {

        base.decrementPotentialValue();
        if (potentialValue == 0)
        {
            taskManager.deleteMaterialFromList(this.gameObject,gameObject.tag);
        }

    }
}
