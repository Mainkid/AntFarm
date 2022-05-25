using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class GrassGenerator : MonoBehaviour
{


   
    [SerializeField]
    private TileBase grassTile;
    [SerializeField]
    private TileBase waterTile;
    [SerializeField]
    private TileBase fogTile;
    [SerializeField]
    private TileBase fogTileBorder;
    [SerializeField]
    private Tilemap grassTileMap;
    [SerializeField]
    public Tilemap fogTileMap;
    [SerializeField]
   
    private List<TileBase> tileBaseList;

    [Header("FoodParams")]
    
    [SerializeField]
    private GameObject food;

    [Header("SandBiom")]
    [SerializeField]
    private List<TileBase> sandTiles;
    [SerializeField]
    private TileBase sandTile;

    [Header("StoneBiom")]
    [SerializeField]
    private List<TileBase> stoneTiles;
    [SerializeField]
    private TileBase stoneTile;

    [Header("ForestBiom")]
    [SerializeField]
    private List<TileBase> forestTiles;
    [SerializeField]
    private TileBase forestTile;


    private HashSet<Vector2Int> grassTileSet = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> waterTileSet = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> fogTileSet = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> borderFogTileSet = new HashSet<Vector2Int>();

    //_____Биомы______
    private List<Vector2Int> biomPoints = new List<Vector2Int>(); // "Радиусы" биомов
    private HashSet<Vector2Int> sandBiom = new HashSet<Vector2Int>(); //Песчаный биом
    private HashSet<Vector2Int> stoneBiom = new HashSet<Vector2Int>(); //Каменный биом
    private HashSet<Vector2Int> forestBiom = new HashSet<Vector2Int>(); //Лесной биом



    public List<Vector2Int> sortedFogTilesList = new List<Vector2Int>();




    private enum tilePositions
    {
        Bot,
        BotRight,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BotLeft,
        Middle,
        SoloTop,
        SoloRight,
        SoloBot,
        SoloLeft,
        SoloVertical,
        SoloGorizontal
    };

    void Start()
    {
        GenerateSquareGrassField(); //Grass generation
        GenerateWarFogField(); //Fog generation
        //GenerateFood(); //Generation food in visible area
        GenerateBiomPoints(); //Generate centers of bioms
        GenerateBioms();
    }

    void GenerateSquareGrassField()
    {
        for (int i = -Parameters.fieldSize / 2; i < Parameters.fieldSize / 2; i++)
            for (int j = -Parameters.fieldSize / 2; j < Parameters.fieldSize / 2; j++)
            {
                grassTileMap.SetTile(new Vector3Int(i, j, 0), grassTile);
                grassTileSet.Add(new Vector2Int(i, j));
                fogTileSet.Add(new Vector2Int(i, j));
            }
        for (int i = -Parameters.fieldSize /2; i < Parameters.fieldSize /2; i++)
        {
            //Horizontal and vertical borders of the square
            grassTileMap.SetTile(new Vector3Int(i, Parameters.fieldSize /2 - 1, 0), tileBaseList[(int)tilePositions.Top]);
            grassTileMap.SetTile(new Vector3Int(i, -Parameters.fieldSize /2, 0), tileBaseList[(int)tilePositions.Bot]);
            grassTileMap.SetTile(new Vector3Int(-Parameters.fieldSize / 2 ,i,  0), tileBaseList[(int)tilePositions.Left]);
            grassTileMap.SetTile(new Vector3Int(Parameters.fieldSize / 2 - 1, i, 0), tileBaseList[(int)tilePositions.Right]);

            //Water borders

            grassTileMap.SetTile(new Vector3Int(i, Parameters.fieldSize / 2 , 0),waterTile);
            grassTileMap.SetTile(new Vector3Int(i, -Parameters.fieldSize / 2-1, 0), waterTile);
            grassTileMap.SetTile(new Vector3Int(-Parameters.fieldSize / 2-1, i, 0), waterTile);
            grassTileMap.SetTile(new Vector3Int(Parameters.fieldSize / 2, i, 0), waterTile);

            //grassTileMap.Set
        }
    }

    void GenerateWarFogField()
    {
        Vector2Int startPos=new Vector2Int(0,0);
        HashSet<Vector2Int> noFogSet = new HashSet<Vector2Int>();
        

        foreach (Vector2Int fogPos in fogTileSet)
        {
            if (Vector2Int.Distance(fogPos, startPos) > Parameters.fogWarRadius)
                fogTileMap.SetTile(new Vector3Int(fogPos.x, fogPos.y, 0), fogTile);
            else
                noFogSet.Add(fogPos);

        }
        fogTileSet.ExceptWith(noFogSet);
        foreach (Vector2Int fogPos in noFogSet)
        {
            CheckBordersInSet(fogPos, noFogSet);
        }

        foreach(Vector2Int fogPos in borderFogTileSet)
        {
            fogTileMap.SetTile((Vector3Int)fogPos, fogTileBorder);
        }
        sortedFogTilesList = fogTileSet.ToList();
        sortedFogTilesList=sortedFogTilesList.OrderBy(x => Vector2Int.Distance(x, startPos)).ToList();

            Debug.Log(sortedFogTilesList.Count);
        Debug.Log(fogTileSet.Count);

    }

    void GenerateBiomPoints()
    {
        Vector2Int startPos = new Vector2Int(UnityEngine.Random.Range(-Parameters.fieldSize / 3, Parameters.fieldSize / 3),
           UnityEngine.Random.Range(-Parameters.fieldSize / 3, Parameters.fieldSize / 3));
        biomPoints.Add(startPos);

        bool isDistanceOK = false;

        int biomsAmount = 0;

        if (Parameters.isForestBiomActive)
            biomsAmount += Parameters.forestBiomLocations;
        if (Parameters.isSandBiomActive)
            biomsAmount += Parameters.sandBiomLocations;
        if (Parameters.isStoneBiomActive)
            biomsAmount += Parameters.stoneBiomLocations;



        for (int i=0;i<biomsAmount-1;i++)
        {
            while (!isDistanceOK)
            {
                isDistanceOK = true;
                startPos = new Vector2Int(UnityEngine.Random.Range(-Parameters.fieldSize / 3, Parameters.fieldSize / 3),
           UnityEngine.Random.Range(-Parameters.fieldSize / 3, Parameters.fieldSize / 3));

                foreach (Vector2Int biomPoint in biomPoints)
                {
                    if (Vector2Int.Distance(biomPoint, startPos) < Parameters.minBiomPointDistance)
                        isDistanceOK = false;

                }

            }
            biomPoints.Add(startPos);
            isDistanceOK = false;

        }
        biomPoints.Add(Vector2Int.zero);
        //biomPoints.Add(new Vector2Int(-Parameters.fieldSize / 2, Parameters.fieldSize / 2));
        //biomPoints.Add(new Vector2Int(Parameters.fieldSize / 2, Parameters.fieldSize / 2));
        //biomPoints.Add(new Vector2Int(-Parameters.fieldSize / 2, -Parameters.fieldSize / 2));
        //biomPoints.Add(new Vector2Int(Parameters.fieldSize / 2,- Parameters.fieldSize / 2));
    }

    HashSet<Vector2Int> GenerateBiom(int idInList)
    {
        HashSet<Vector2Int> biomSet = new HashSet<Vector2Int>();
        float minDistance = 100000;
        float minPointDistance = 1000000;
        int idMinDistance = 0;
        float sumDistance = 0;

        foreach (Vector2Int biompointA in biomPoints)
        {
            if (Vector2Int.Distance(biompointA, biomPoints[idInList]) < minPointDistance)
                minPointDistance = Vector2Int.Distance(biompointA, biomPoints[idInList]);

            minPointDistance += Vector2Int.Distance(biompointA, biomPoints[idInList]);
        }

        for (int i = -Parameters.fieldSize / 2; i < Parameters.fieldSize / 2; i++)
            for (int j = -Parameters.fieldSize / 2; j < Parameters.fieldSize / 2; j++)
            {
                for (int k=0;k<biomPoints.Count;k++)
                {
                    if (Vector2Int.Distance(biomPoints[k],new Vector2Int(i,j))<minDistance)
                    {
                        minDistance = Vector2Int.Distance(biomPoints[k], new Vector2Int(i, j));
                        idMinDistance = k;
                    }
                }
                if (idMinDistance == idInList  )
                {
                    if (1-UnityEngine.Random.Range(minDistance,minPointDistance)/minPointDistance>0.00f)
                        biomSet.Add(new Vector2Int(i, j));
                }
                minDistance = 1000000;
                
               
            }

        for (int i = -Parameters.fieldSize / 2; i < Parameters.fieldSize / 2; i++)
            for (int j = -Parameters.fieldSize / 2; j < Parameters.fieldSize / 2; j++)
            {

                if (!biomSet.Contains(new Vector2Int(i, j)) && biomSet.Contains(new Vector2Int(i + 1, j)) &&
                    biomSet.Contains(new Vector2Int(i - 1, j)) && biomSet.Contains(new Vector2Int(i, j + 1)) &&
                    biomSet.Contains(new Vector2Int(i, j - 1)))
                    biomSet.Add(new Vector2Int(i, j));

            }


                return biomSet;

    }

    private void PaintTiles(HashSet<Vector2Int> tileSet, TileBase tile)
    {
        foreach (Vector2Int vectorPos in tileSet)
            grassTileMap.SetTile((Vector3Int)vectorPos, tile);
    }

    private void PaintBorderTiles(HashSet<Vector2Int> tileSet,List<TileBase> tileBaseList)
    {
        bool is_rightTile = false;
        bool is_leftTile = false;
        bool is_topTile = false;
        bool is_botTile = false;

        foreach (Vector2Int tile in tileSet)
        {
            is_rightTile = false;
             is_leftTile = false;
            is_topTile = false;
            is_botTile = false;

            if (tileSet.Contains(tile + Direction2D.Up))
                is_topTile = true;
            if (tileSet.Contains(tile + Direction2D.Down))
                is_botTile = true;
            if (tileSet.Contains(tile + Direction2D.Right))
                is_rightTile = true;
            if (tileSet.Contains(tile + Direction2D.Left))
                is_leftTile = true;


            if (is_botTile && is_rightTile && !is_topTile && !is_leftTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.TopLeft]);
            else if (is_botTile && is_rightTile && is_leftTile && !is_topTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.Top]);
            else if (is_botTile && !is_rightTile && is_leftTile && !is_topTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.TopRight]);
            else if (is_topTile && is_leftTile && is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.Right]);
            else if (is_topTile && is_leftTile && !is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.BotRight]);
            else if (is_topTile && is_leftTile && !is_botTile && is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.Bot]);
            else if (is_topTile && !is_leftTile && !is_botTile && is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.BotLeft]);
            else if (is_topTile && !is_leftTile && is_botTile && is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.Left]);
            else if (!is_topTile && !is_leftTile && is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloTop]);
            else if (!is_topTile && is_leftTile && !is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloRight]);
            else if (is_topTile && !is_leftTile && !is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloBot]);
            else if (!is_topTile && !is_leftTile && !is_botTile && is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloLeft]);
            else if (is_topTile && !is_leftTile && is_botTile && !is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloVertical]);
            else if (!is_topTile && is_leftTile && !is_botTile && is_rightTile)
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.SoloGorizontal]);
            else
                grassTileMap.SetTile((Vector3Int)tile, tileBaseList[(int)tilePositions.Middle]);
        }    
    }

    private void CheckBordersInSet(Vector2Int fogPos, HashSet<Vector2Int> noFogSet)
    {
        if (!noFogSet.Contains(fogPos + Direction2D.Up))
            borderFogTileSet.Add(fogPos + Direction2D.Up);
        if (!noFogSet.Contains(fogPos + Direction2D.Down))
            borderFogTileSet.Add(fogPos + Direction2D.Down);
        if (!noFogSet.Contains(fogPos + Direction2D.Left))
            borderFogTileSet.Add(fogPos + Direction2D.Left);
        if (!noFogSet.Contains(fogPos + Direction2D.Right))
            borderFogTileSet.Add(fogPos + Direction2D.Right);
    }

    public void ClearFogInPosition(Vector2Int pos)
    {
        StartCoroutine(alphaFogDissolve(pos));
       //fogTileSet.Remove(pos);
       //sortedFogTilesList.Remove(pos);
       //fogTileMap.SetTile((Vector3Int)pos, null);
                
        
    }

    //public void GenerateFood()
    //{
    //    HashSet<Vector2Int> visibleFieldSet = new HashSet<Vector2Int>();
    //    visibleFieldSet = grassTileSet;
    //    visibleFieldSet.ExceptWith(fogTileSet);
    //    List<Vector2Int> visibleFieldList= visibleFieldSet.ToList();
    //    Vector2 cellSize = fogTileMap.cellSize;

    //    Debug.Log(fogTileMap.cellSize);
        
    //    for (int i=0;i<visibleFieldSet.Count;i++)
    //    {
    //        for (int j=0; j<Parameters.foodAmount/visibleFieldSet.Count;j++)
    //        {
    //            Vector2 RandomVector = new Vector2(UnityEngine.Random.Range(fogTileMap.CellToWorld((Vector3Int)visibleFieldList[i]).x,
    //               cellSize.x + fogTileMap.CellToWorld((Vector3Int)visibleFieldList[i]).x),
    //               UnityEngine.Random.Range( fogTileMap.CellToWorld((Vector3Int)visibleFieldList[i]).y,
    //               cellSize.y + fogTileMap.CellToWorld((Vector3Int)visibleFieldList[i]).y));
    //            Debug.Log(RandomVector);
    //            GenerateFoodInCurrentPosition(RandomVector);
    //        }
    //    }

    //}

    //public void GenerateFoodInCurrentPosition(Vector2 position)
    //{
    //    Instantiate(food, (Vector3)position, Quaternion.identity);
    //}

    public void GenerateBioms()
    {
        int headCounter = 0;
        //Генерация песчаных биомов
        if (Parameters.isSandBiomActive)
        {
            for (int i = 0; i < Parameters.sandBiomLocations;i++)
            {
                sandBiom.UnionWith(GenerateBiom(headCounter));
                headCounter++;
            }
        }
        PaintTiles(sandBiom,sandTile);
        PaintBorderTiles(sandBiom,sandTiles);

        if (Parameters.isStoneBiomActive)
        {
            for (int i = 0; i < Parameters.stoneBiomLocations; i++)
            {
                stoneBiom.UnionWith(GenerateBiom(headCounter));
                headCounter++;
            }
        }
        PaintTiles(stoneBiom, stoneTile);
        PaintBorderTiles(stoneBiom, stoneTiles);

        if (Parameters.isForestBiomActive)
        {
            for (int i = 0; i < Parameters.forestBiomLocations; i++)
            {
                forestBiom.UnionWith(GenerateBiom(headCounter));
                headCounter++;

            }
        }
        PaintTiles(forestBiom, forestTile);
        PaintBorderTiles(forestBiom, forestTiles);

    }

    IEnumerator alphaFogDissolve(Vector2Int pos)
    {
        fogTileSet.Remove(pos);
        fogTileMap.SetTileFlags((Vector3Int)pos, TileFlags.None);

        float Timer = 0;
        Color32 clr = fogTileMap.GetColor((Vector3Int) pos);
        Color32 targetClr = new Color32(0, 0, 0, 0);
        while (clr.a != 0)
        {
            Timer += Time.deltaTime;
            yield return null;
            clr = Color32.Lerp(clr, targetClr, Timer);
            fogTileMap.SetColor((Vector3Int)pos,clr);
          
        }
        
        
        fogTileMap.SetTile((Vector3Int)pos, null);
    }

}

public static class Direction2D
{
    
    public static Vector2Int Up =new Vector2Int(0, 1);
    public static Vector2Int Down = new Vector2Int(0, -1);
    public static Vector2Int Right = new Vector2Int(1, 0);
    public static Vector2Int Left = new Vector2Int(-1, 0);
    public static List<Vector2Int> DirectionsList = new List<Vector2Int> { Up, Down, Right, Left };

}
