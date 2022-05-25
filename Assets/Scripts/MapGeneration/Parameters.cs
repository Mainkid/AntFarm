using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameters
{
    public static int foodAmount=10; //кол-во еды 
    public static int fieldSize=50; //размер поля (в тайлах)
    public static int fogWarRadius=2; //радиус тумана войны (в тайлах)


    //____ГЕНЕРАЦИЯ БИОМОВ______

    public static int biomsAmount = 3; //Кол-во биомов
    public static int minBiomPointDistance = 5; //Минимальное расстояние между центрами биомов

    public static bool isSandBiomActive = true; //наличие биома пустыни
    //public static int sandBiomTiles = 70; //количество тайлов песка в одной локации
    public static int sandBiomLocations = 9; //количество локаций с биомом песка

    public static bool isStoneBiomActive = true; //наличие биома камня
    public static int stoneBiomLocations = 9; //количество локаций с биомом камня
    public static bool isForestBiomActive = true; //наличие биома леса
    public static int forestBiomLocations = 9; //количество локаций с биомом леса

    //____ГЕНЕРАЦИЯ ЕДЫ______

    public static Dictionary<string, int> foodPrice = new Dictionary<string, int>()
    {
        { "Strawberry",5 },
        { "Mushroom",15 }
    };


    //____ГЕНЕРАЦИЯ РЕСУРСОВ_____

    public static Dictionary<string, int> materialPrice = new Dictionary<string, int>()
    {
        { "Soil",5 },
        { "Sand",5 },
        { "Clay",5 },
        { "Sandstone", 5 }
    };

}
