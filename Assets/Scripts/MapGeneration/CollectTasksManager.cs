using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollectTasksManager : MonoBehaviour
{
    public List<GameObject> foodList = new List<GameObject>();
    public List<string> materialNamesList = new List<string>();
    public Dictionary<string, List<GameObject>> materialsDictionary = new Dictionary<string, List<GameObject>>()
    {
        {"Soil",new List<GameObject>() },
        {"Clay",new List<GameObject>() },
        {"Sandstone", new List<GameObject>() },
        {"Sand", new List<GameObject>() }

    };



    private void Awake()
    {
        convertToList(GameObject.FindGameObjectsWithTag("Food"), foodList);
       
        foreach (KeyValuePair<string, List<GameObject>> keyPair in materialsDictionary)
        {
            materialNamesList.Add(keyPair.Key);
        }
        foreach (string key in materialNamesList)
        {
            materialsDictionary[key] = GameObject.FindGameObjectsWithTag(key).ToList();
        }


    }
    private void Start()
    {
       
        //materialList = GameObject.FindGameObjectsWithTag("Material");
    }

    private void convertToList(GameObject[] gameObjectsArray,List<GameObject> targetList)
    {
        foreach (GameObject gameobject in gameObjectsArray)
        {
            targetList.Add(gameobject);
            Debug.Log(gameobject.name);
        }

        Debug.Log(foodList.Count);
            
    }

    public void deleteFoodFromList(GameObject food)
    {
        foodList.Remove(food);
    }

    public void deleteMaterialFromList(GameObject material, string materialKey)
    {
        materialsDictionary[materialKey].Remove(material);
    }

}
