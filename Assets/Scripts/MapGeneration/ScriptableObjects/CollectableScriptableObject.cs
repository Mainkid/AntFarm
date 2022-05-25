using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CollectableScriptableObject", order = 1)]
public class CollectableScriptableObject : ScriptableObject
{
    public List<Sprite> Sprites = new List<Sprite>();
    public Sprite Particle;
    public string Name;
}
