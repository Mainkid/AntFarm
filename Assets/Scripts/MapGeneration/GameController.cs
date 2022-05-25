using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int money;
    [SerializeField]
    private Text pointsText;

    private void Start()
    {
        //Application.targetFrameRate = 60;
        //Time.fixedDeltaTime = 0.03f;
        money = 0;
        pointsText.text = "Points: 0";
    }

    public void ChangeMoneyValue(int value)
    {
        money += value;
        pointsText.text = "Points: " + money;
    }

}
