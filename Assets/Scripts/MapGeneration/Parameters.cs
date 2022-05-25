using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameters
{
    public static int foodAmount=10; //���-�� ��� 
    public static int fieldSize=50; //������ ���� (� ������)
    public static int fogWarRadius=2; //������ ������ ����� (� ������)


    //____��������� ������______

    public static int biomsAmount = 3; //���-�� ������
    public static int minBiomPointDistance = 5; //����������� ���������� ����� �������� ������

    public static bool isSandBiomActive = true; //������� ����� �������
    //public static int sandBiomTiles = 70; //���������� ������ ����� � ����� �������
    public static int sandBiomLocations = 9; //���������� ������� � ������ �����

    public static bool isStoneBiomActive = true; //������� ����� �����
    public static int stoneBiomLocations = 9; //���������� ������� � ������ �����
    public static bool isForestBiomActive = true; //������� ����� ����
    public static int forestBiomLocations = 9; //���������� ������� � ������ ����

    //____��������� ���______

    public static Dictionary<string, int> foodPrice = new Dictionary<string, int>()
    {
        { "Strawberry",5 },
        { "Mushroom",15 }
    };


    //____��������� ��������_____

    public static Dictionary<string, int> materialPrice = new Dictionary<string, int>()
    {
        { "Soil",5 },
        { "Sand",5 },
        { "Clay",5 },
        { "Sandstone", 5 }
    };

}
