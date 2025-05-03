using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHolder
{
    public static GameObject player;
    public static float fixedFrameTime = 0.02f;
    public static int mapSideLength = 512;
    public static float tileSize = 0.32f;
    public static int freeRoamCoef = 32;
    public static int choice = -1;
    public static int lastFileChoice = -1;
    public static string lastFilePath = "";
    public static AllDataHolder aDH = new AllDataHolder();
    public static string savesPath = @"D:\UnityProjects\MyGameProject\Assets\Saves\";
}

public class AllDataHolder
{
    //Данные генератора врагов
    public float enemiesHealth;
    //public static GameObject[] enemies;
    //public static int[] activeEnemies;
    //Данные игрока
    public float playerSpeed;
    public float playerMovementBorder;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public int playerDamage;
    public int playerScore;
    public int playerCurrentXP;
    public int playerMaxXP;
    public int playerLevel;
    //Данные карты
    public int mapLength;
    public int[][] mapMatrix;
    //Конструкторы
    public AllDataHolder() { }
    public AllDataHolder(float eH, float pS, float pMB, int pCH, int pMH, int pD, int pScore, int pCXP, int pMXP, int pL, int mL, int[][] mM)
    {
        enemiesHealth = eH;
        playerSpeed = pS;
        playerMovementBorder = pMB;
        playerCurrentHealth = pCH;
        playerMaxHealth = pMH;
        playerDamage = pD;
        playerScore = pScore;
        playerCurrentXP = pCXP;
        playerMaxXP = pMXP;
        playerLevel = pL;
        mapLength = mL;
        mapMatrix = new int[mM.Length][];
        for(int i = 0; i < mM.Length; i++)
        {
            mapMatrix[i] = new int[mM[i].Length];
            for(int j = 0; j < mM[i].Length; j++)
            {
                mapMatrix[i][j] = mM[i][j];
            }
        }
    }
}
