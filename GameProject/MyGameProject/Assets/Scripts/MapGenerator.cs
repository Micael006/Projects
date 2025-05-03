using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    Sprite[] grassSprites;
    PlayerControl player;
    static int mapSideLength; // Сторона карты 128 => сама карта 128x128
    static int tileSideLength; //Размер генерируемого квадрата 32x32
    static float tileSize; //Размер одной клеточки 0.32 x 0.32
    static int tileXSide; // Количество тайлов по X
    static int tileYSide; //Количество тайлов по y
    static int freeRoamCoef;
    public float playerCurrentX, playerCurrentY;
    GameObject mapHolder;
    public int[][] map;
    GameObject[][] currentMap;
    System.Random rand;
    int[][] patterns;
    // Start is called before the first frame update
    void Start()
    {
        //ResetNewGame();
        /*for (int i = 0; i < map.Length; i++)
            map[i] = new int[mapSideLength];
        for(int i = 0; i < tileXSide; i++)
        {
            currentMap[i] = new GameObject[tileYSide];
        }
        //grassSprites = Resources.LoadAll<Sprite>("Sprites/Environment/Ground/Grass/grass");
        for (int i = 0; i < grassSprites.Length; i++)
        {
            grassSprites[i] = Resources.Load<Sprite>("Sprites/Environment/Ground/Grass/grass" + i);
        }
        mapHolder = new GameObject();
        mapHolder.transform.position = new Vector3(0, 0, 0);
        mapHolder.name = "MapHolder";
        mapHolder.AddComponent<SpriteRenderer>();
        PatternCreation();
        GenerateMap();*/
    }

    public void ResetNewGame()
    {
        grassSprites = new Sprite[9];
        mapSideLength = DataHolder.mapSideLength; // Сторона карты 128 => сама карта 128x128
        tileSideLength = 32; //Размер генерируемого квадрата 32x32
        tileSize = DataHolder.tileSize; //Размер одной клеточки 0.32 x 0.32
        tileXSide = 128; // Количество тайлов по X
        tileYSide = 100; //Количество тайлов по y
        freeRoamCoef = DataHolder.freeRoamCoef;
        map = new int[mapSideLength][];
        currentMap = new GameObject[tileXSide][];
        rand = new System.Random();

        for (int i = 0; i < map.Length; i++)
            map[i] = new int[mapSideLength];
        for (int i = 0; i < tileXSide; i++)
        {
            currentMap[i] = new GameObject[tileYSide];
        }
        for (int i = 0; i < grassSprites.Length; i++)
        {
            grassSprites[i] = Resources.Load<Sprite>("Sprites/Environment/Ground/Grass/grass" + i);
        }
        Destroy(mapHolder);
        mapHolder = new GameObject();
        mapHolder.transform.position = new Vector3(0, 0, 0);
        mapHolder.name = "MapHolder";
        mapHolder.AddComponent<SpriteRenderer>();
        PatternCreation();
        GenerateMap();
    }

    public void ResetLoadGame()
    {
        grassSprites = new Sprite[9];
        mapSideLength = DataHolder.aDH.mapLength; // Сторона карты 128 => сама карта 128x128
        tileSideLength = 32; //Размер генерируемого квадрата 32x32
        tileSize = DataHolder.tileSize; //Размер одной клеточки 0.32 x 0.32
        tileXSide = 128; // Количество тайлов по X
        tileYSide = 100; //Количество тайлов по y
        freeRoamCoef = DataHolder.freeRoamCoef;
        map = new int[mapSideLength][];
        currentMap = new GameObject[tileXSide][];
        rand = new System.Random();

        for (int i = 0; i < map.Length; i++)
            map[i] = new int[mapSideLength];
        for (int i = 0; i < tileXSide; i++)
        {
            currentMap[i] = new GameObject[tileYSide];
        }
        for (int i = 0; i < grassSprites.Length; i++)
        {
            grassSprites[i] = Resources.Load<Sprite>("Sprites/Environment/Ground/Grass/grass" + i);
        }
        Destroy(mapHolder);
        mapHolder = new GameObject();
        mapHolder.transform.position = new Vector3(0, 0, 0);
        mapHolder.name = "MapHolder";
        mapHolder.AddComponent<SpriteRenderer>();
        GenerateMap(DataHolder.aDH.mapMatrix);
    }

    //Заполнение паттернов (Паттерны могут иметь только чётную сторону, так как алгоритмы работают только с 2N x 2N
    void PatternCreation()
    {
        //Бронирование памяти под паттерны
        int numberOfPatterns = 12;
        int tilesAmount = tileSideLength * tileSideLength;
        int patternNumber;
        patterns = new int[numberOfPatterns][];
        for (int i = 0; i < patterns.Length; i++)
            patterns[i] = new int[tilesAmount];
        //Группа паттернов "Змейка" 
        // #1 "Проверен с помощью консоли"
        // 0 7 8 F
        // 1 6 9 E
        // 2 5 A D
        // 3 4 B C
        patternNumber = 0;
        for (int i = 0; i < tileSideLength; i++)
        {
            for (int j = 0; j < tileSideLength; j++)
            {
                patterns[patternNumber][i * tileSideLength + j] = i + ((i % 2 == 0) ? (tileSideLength * j) : (tileSideLength * (tileSideLength - (j + 1))));
            }
        }
        // #2 "Проверен с помощью консоли"
        // 0 2 3 9
        // 1 4 8 A
        // 5 7 B E
        // 6 C D F
        patternNumber = 1;
        int count = 0;
        patterns[patternNumber][count] = 0;
        count++;
        bool moveUp = true;
        bool passedDiagonal = false;
        int w = 0, h = 1;
        while (count < tilesAmount)
        {
            if (!passedDiagonal && (w + h) == tileSideLength - 1)
            {
                passedDiagonal = true;
            }
            if (moveUp && h > 0 && w < tileSideLength - 1)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w++;
                h--;
                count++;
            }
            else if (moveUp && (h == 0 || w == tileSideLength - 1))
            {
                moveUp = false;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (passedDiagonal)
                {
                    h++;
                }
                else
                {
                    w++;
                }
            }
            else if (!moveUp && h < tileSideLength - 1 && w > 0)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w--;
                h++;
                count++;
            }
            else if (!moveUp && (h == tileSideLength - 1 || w == 0))
            {
                moveUp = true;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (!passedDiagonal)
                {
                    h++;
                }
                else
                {
                    w++;
                }
            }
        }
        // #3 "Проверен с помощью консоли"
        // 0 1 2 3
        // 7 6 5 4
        // 8 9 A B
        // F E D C
        patternNumber = 2;
        for (int i = 0; i < tileSideLength; i++)
        {
            for (int j = 0; j < tileSideLength; j++)
            {
                patterns[patternNumber][i * tileSideLength + j] = i * tileSideLength + ((i % 2 == 0) ? j : (tileSideLength - (j + 1)));
            }
        }
        // #4 "Проверен с помощью консоли"
        // 9 A E F
        // 3 8 B D
        // 2 4 7 C
        // 0 1 5 6
        patternNumber = 3;
        count = 0;
        patterns[patternNumber][count] = tileSideLength * (tileSideLength - 1);
        count++;
        moveUp = true;
        passedDiagonal = false;
        w = 1;
        h = tileSideLength - 1;
        while (count < tilesAmount)
        {
            if (!passedDiagonal && (w + h) == (2 * (tileSideLength - 1)))
            {
                passedDiagonal = true;
            }
            if (moveUp && h > 0 && w > 0)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w--;
                h--;
                count++;
            }
            else if (moveUp && (h == 0 || w == 0))
            {
                moveUp = false;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (passedDiagonal)
                {
                    w++;
                }
                else
                {
                    h--;
                }
            }
            else if (!moveUp && h < tileSideLength - 1 && w < tileSideLength - 1)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w++;
                h++;
                count++;
            }
            else if (!moveUp && (h == tileSideLength - 1 || w == tileSideLength - 1))
            {
                moveUp = true;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (!passedDiagonal)
                {
                    w++;
                }
                else
                {
                    h--;
                }
            }
        }
        // #5 "Проверен с помощью консоли"
        // 3 4 B C
        // 2 5 A D
        // 1 6 9 E 
        // 0 7 8 F
        patternNumber = 4;
        for (int i = 0; i < tileSideLength; i++)
        {
            for (int j = tileSideLength - 1; j >= 0; j--)
            {
                patterns[patternNumber][i * tileSideLength + (tileSideLength - 1 - j)] = i + ((i % 2 == 0) ? (tileSideLength * j) : (tileSideLength * (tileSideLength - (j + 1))));
            }
        }
        // #6 "Проверен с помощью консоли"
        // F E A 9
        // D B 8 3
        // C 7 4 2
        // 6 5 1 0
        patternNumber = 5;
        count = 0;
        patterns[patternNumber][count] = (tileSideLength * tileSideLength) - 1;
        count++;
        moveUp = true;
        passedDiagonal = false;
        w = tileSideLength - 2;
        h = tileSideLength - 1;
        while (count < tilesAmount)
        {
            if (!passedDiagonal && ((w + h) == tileSideLength - 1))
            {
                passedDiagonal = true;
            }
            if (moveUp && h > 0 && w < tileSideLength - 1)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w++;
                h--;
                count++;
            }
            else if (moveUp && (h == 0 || w == tileSideLength - 1))
            {
                moveUp = false;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (passedDiagonal)
                {
                    w--;
                }
                else
                {
                    h--;
                }
            }
            else if (!moveUp && h < tileSideLength - 1 && w > 0)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w--;
                h++;
                count++;
            }
            else if (!moveUp && (h == tileSideLength - 1 || w == 0))
            {
                moveUp = true;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (!passedDiagonal)
                {
                    w--;
                }
                else
                {
                    h--;
                }
            }
        }
        // #7 "Проверен с помощью консоли"
        // 3 2 1 0 
        // 4 5 6 7
        // B A 9 8
        // C D E F
        patternNumber = 6;
        for (int i = 0; i < tileSideLength; i++)
        {
            for (int j = tileSideLength - 1; j >= 0; j--)
            {
                patterns[patternNumber][i * tileSideLength + (tileSideLength - 1 - j)] = (i * tileSideLength) + ((i % 2 == 0) ? j : (tileSideLength - 1 - j));
            }
        }
        // #8 "Проверен с помощью консоли"
        // 6 5 1 0
        // C 7 4 2
        // D B 8 3
        // F E A 9
        patternNumber = 7;
        count = 0;
        patterns[patternNumber][count] = tileSideLength - 1;
        count++;
        moveUp = false;
        passedDiagonal = false;
        w = tileSideLength - 2;
        h = 0;
        while (count < tilesAmount)
        {
            if (!passedDiagonal && (w + h) == 0)
            {
                passedDiagonal = true;
            }
            if (moveUp && h > 0 && w > 0)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w--;
                h--;
                count++;
            }
            else if (moveUp && (h == 0 || w == 0))
            {
                moveUp = false;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (passedDiagonal)
                {
                    h++;
                }
                else
                {
                    w--;
                }
            }
            else if (!moveUp && h < tileSideLength - 1 && w < tileSideLength - 1)
            {
                patterns[patternNumber][count] = w + h * tileSideLength;
                w++;
                h++;
                count++;
            }
            else if (!moveUp && (h == tileSideLength - 1 || w == tileSideLength - 1))
            {
                moveUp = true;
                patterns[patternNumber][count] = w + h * tileSideLength;
                count++;
                if (!passedDiagonal)
                {
                    h++;
                }
                else
                {
                    w--;
                }
            }
        }

        //Группа паттернов "Ракушка"
        // #1 (#9) "Проверен с помощью консоли"
        // 9 8 7 6
        // A F E 5
        // B C D 4
        // 0 1 2 3
        patternNumber = 8;
        count = 0;
        w = 0;
        h = tileSideLength - 1;
        int direction = 0; // 0 - вправо; 1 - вверх; 2 - влево; 3 - вниз
        bool circleStarted = false;
        while (count < tilesAmount)
        {
            if (circleStarted && (w == tileSideLength - 1 - h) && h > w)
            {
                w++;
                h--;
                direction = 0;
                circleStarted = false;
            }
            patterns[patternNumber][count] = w + h * tileSideLength;
            count++;
            circleStarted = true;
            w += (direction % 2 == 0) ? (direction % 4 == 0) ? 1 : -1 : 0;
            h += (direction % 2 == 1) ? ((direction % 4 == 3) ? 1 : -1) : 0;
            if (w == h && w >= (tileSideLength / 2))
            {
                direction = 1;
            }
            else if ((w == tileSideLength - 1 - h) && h < w)
            {
                direction = 2;
            }
            else if (w == h && w < (tileSideLength / 2))
            {
                direction = 3;
            }
        }
        // #2 (#10) "Проверен с помощью консоли"
        // 6 7 8 9
        // 5 E F A
        // 4 D C B
        // 3 2 1 0
        patternNumber = 9;
        count = 0;
        w = tileSideLength - 1;
        h = tileSideLength - 1;
        direction = 2; // 0 - вправо; 1 - вверх; 2 - влево; 3 - вниз
        circleStarted = false;
        while (count < tilesAmount)
        {
            if (circleStarted && (w == h) && h >= (tileSideLength / 2))
            {
                w--;
                h--;
                direction = 2;
                circleStarted = false;
            }
            patterns[patternNumber][count] = w + h * tileSideLength;
            count++;
            circleStarted = true;
            w += (direction % 2 == 0) ? (direction % 4 == 0) ? 1 : -1 : 0;
            h += (direction % 2 == 1) ? ((direction % 4 == 3) ? 1 : -1) : 0;
            if ((w == h) && h < (tileSideLength / 2))
            {
                direction = 0;
            }
            else if ((w == tileSideLength - 1 - h) && w < (tileSideLength / 2))
            {
                direction = 1;
            }
            else if ((w == tileSideLength - 1 - h) && w >= (tileSideLength / 2))
            {
                direction = 3;
            }
        }
        // #3 (#11) "Проверен с помощью консоли"
        // 3 2 1 0
        // 4 D C B
        // 5 E F A
        // 6 7 8 9
        patternNumber = 10;
        count = 0;
        w = tileSideLength - 1;
        h = 0;
        direction = 2; // 0 - вправо; 1 - вверх; 2 - влево; 3 - вниз
        circleStarted = false;
        while (count < tilesAmount)
        {
            if (circleStarted && (w == tileSideLength - 1 - h) && h < (tileSideLength / 2))
            {
                w--;
                h++;
                direction = 2;
                circleStarted = false;
            }
            patterns[patternNumber][count] = w + h * tileSideLength;
            count++;
            circleStarted = true;
            w += (direction % 2 == 0) ? (direction % 4 == 0) ? 1 : -1 : 0;
            h += (direction % 2 == 1) ? ((direction % 4 == 3) ? 1 : -1) : 0;
            if ((w == tileSideLength - 1 - h) && h >= (tileSideLength / 2))
            {
                direction = 0;
            }
            else if ((w == h) && w >= (tileSideLength / 2))
            {
                direction = 1;
            }
            else if ((w == h) && w < (tileSideLength / 2))
            {
                direction = 3;
            }
        }
        // #4 (#12) "Проверен с помощью консоли"
        // 0 1 2 3
        // B C D 4
        // A F E 5
        // 9 8 7 6
        patternNumber = 11;
        count = 0;
        w = 0;
        h = 0;
        direction = 0; // 0 - вправо; 1 - вверх; 2 - влево; 3 - вниз
        circleStarted = false;
        while (count < tilesAmount)
        {
            if (circleStarted && (w == h) && h < (tileSideLength / 2))
            {
                w++;
                h++;
                direction = 0;
                circleStarted = false;
            }
            patterns[patternNumber][count] = w + h * tileSideLength;
            count++;
            circleStarted = true;
            w += (direction % 2 == 0) ? (direction % 4 == 0) ? 1 : -1 : 0;
            h += (direction % 2 == 1) ? ((direction % 4 == 3) ? 1 : -1) : 0;
            if ((w == tileSideLength - 1 - h) && w < (tileSideLength / 2))
            {
                direction = 1;
            }
            if ((w == h) && h >= (tileSideLength / 2))
            {
                direction = 2;
            }
            else if ((w == tileSideLength - 1 - h) && w >= (tileSideLength / 2))
            {
                direction = 3;
            }
        }
    }

    //Генерация карты при помощи NxN объектов (тяжело для игры)
    //Абсолютная индексация в паттернах
    // 0 1 2 3
    // 4 5 6 7
    // 8 9 A B
    // C D E F
    /*void GenerateMap()
    {
        int helper; //Переменная для выбора паттерна
        int sprite = rand.Next(grassSprites.Length);
        for(int i = 0; i < ((map.Length * map.Length) / (tileSideLength * tileSideLength)); i++)
        {
            GameObject tileHolder = new GameObject(); //Вспомогательный объект для упрощения перемещения кусков карты 32x32
            tileHolder.transform.position = new Vector3(0, 0, 0);
            tileHolder.name = "TileHolder(" + i + ")"; 
            tileHolder.transform.parent = mapHolder.transform;
            helper = rand.Next(patterns.Length);
            int lX, lY; 
            for(int j = 0; j < tileSideLength; j++)
            {
                for (int z = 0; z < tileSideLength; z++)
                {
                    lX = (int)(patterns[helper][j * tileSideLength + z] % tileSideLength);
                    lY = (int)(patterns[helper][j * tileSideLength + z] / tileSideLength);
                    /*if(lX < 0 || lY < 0)
                    {
                        Debug.Log("Шиза");
                        Debug.Log("lX = " + lX + ", lY = " + lY);
                        Debug.Log("i = " + i + ", j = " + j + ", z = " + z);
                        Debug.Log("Номер паттерна: " + helper);
                        Debug.Log("Число из паттерна: " + patterns[helper][j * tileSideLength + z]);
                        Debug.Log("Вывод всего паттерна:");
                        string help = "";
                        for(int h = 0; h < patterns[helper].Length; h++)
                            help += patterns[helper][h] + ", ";
                        help = help.Substring(0, help.Length - 2);
                        Debug.Log(help);
                        return;
                    }
                    //участок для замены
                    GameObject tile = new GameObject(); //Создание самого тайла
                    tile.transform.position = new Vector3(lX * tileSize, lY * tileSize, 0); //Установка позиции
                    tile.transform.parent = tileHolder.transform;
                    tile.AddComponent<SpriteRenderer>();
                    tile.GetComponent<SpriteRenderer>().sprite = grassSprites[sprite]; //Установка спрайта
                    tile.name = "tile(" + lY + ")(" + lX + ")";
                    map[lY + ((i % (map.Length / tileSideLength)) * tileSideLength)][lX + ((i / (map.Length / tileSideLength)) * tileSideLength)] = sprite;
                    sprite = rand.Next(grassSprites.Length);
                }
            }
            //Установка позиции вспомогательного объекта
            tileHolder.transform.position = new Vector3((int)(i % (map.Length / tileSideLength)) * tileSize * tileSideLength, (int)(i / (map.Length / tileSideLength)) * tileSize * tileSideLength, 0) ;
        }
        //Смещение карты к центру
        mapHolder.transform.position = new Vector3(-(map.Length / tileSideLength) / 2 * tileSize * tileSideLength, -(map.Length / tileSideLength) / 2 * tileSize * tileSideLength, 0);
    }*/

    //Генерация карты одним объектом с попиксельным копированием (Долгая исходная генерация)
    /*void GenerateMap()
    {
        int helper; //Переменная для выбора паттерна
        int sprite = rand.Next(grassSprites.Length); //Переменная для подбора 
        //Заполнение матрицы карты
        for (int i = 0; i < ((map.Length * map.Length) / (tileSideLength * tileSideLength)); i++)
        {
            helper = rand.Next(patterns.Length);
            int lX, lY;
            for (int j = 0; j < tileSideLength; j++)
            {
                for (int z = 0; z < tileSideLength; z++)
                {
                    lX = (int)(patterns[helper][j * tileSideLength + z] % tileSideLength);
                    lY = (int)(patterns[helper][j * tileSideLength + z] / tileSideLength);

                    map[lY + ((i % (map.Length / tileSideLength)) * tileSideLength)][lX + ((i / (map.Length / tileSideLength)) * tileSideLength)] = sprite;
                    sprite = rand.Next(grassSprites.Length);
                }
            }
        }
        //Сшивка спрайтов в один спрайт

        Resources.UnloadUnusedAssets();
        Texture2D newTex = new Texture2D(mapSideLength * 32, mapSideLength * 32);

        for(int x = 0; x < newTex.width; x++)
        {
            for(int y = 0; y < newTex.height; y++)
            {
                newTex.SetPixel(x, y, new Color(1, 1, 1, 0));
            }
        }
        
        for(int i = 0; i < mapSideLength; i++)
        {
            for(int j = 0; j < mapSideLength; j++)
            {
                for(int x = 0; x < grassSprites[map[i][j]].texture.width; x++)
                {
                    for(int y = 0; y < grassSprites[map[i][j]].texture.height; y++)
                    {
                        var color = grassSprites[map[i][j]].texture.GetPixel(x, y);
                        newTex.SetPixel(i * 32 + x, j * 32 + y, color);
                    }
                }
            }
        }

        newTex.Apply();
        var mapSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
        mapSprite.name = "mapSprite";
        mapHolder.GetComponent<SpriteRenderer>().sprite = mapSprite;
        //Смещение карты к центру
        //mapHolder.transform.position = new Vector3(-(map.Length / tileSideLength) / 2 * tileSize * tileSideLength, -(map.Length / tileSideLength) / 2 * tileSize * tileSideLength, 0);
    }*/

    //Генерация карты с перемещением за игроком
    void GenerateMap()
    {
        int helper; //Переменная для выбора паттерна
        int sprite = rand.Next(grassSprites.Length); //Переменная для подбора 
        //Заполнение матрицы карты
        for (int i = 0; i < ((map.Length * map.Length) / (tileSideLength * tileSideLength)); i++)
        {
            helper = rand.Next(patterns.Length);
            int lX, lY;
            for (int j = 0; j < tileSideLength; j++)
            {
                for (int z = 0; z < tileSideLength; z++)
                {
                    lX = (int)(patterns[helper][j * tileSideLength + z] % tileSideLength);
                    lY = (int)(patterns[helper][j * tileSideLength + z] / tileSideLength);

                    map[lY + ((i % (map.Length / tileSideLength)) * tileSideLength)][lX + ((i / (map.Length / tileSideLength)) * tileSideLength)] = sprite;
                    sprite = rand.Next(grassSprites.Length);
                }
            }
        }
        //Создание тайлов карты
        for (int i = 0; i < tileXSide; i++)
        {
            for (int j = 0; j < tileYSide; j++)
            {
                sprite = map[(mapSideLength / 2) - (tileXSide / 2) + i][(mapSideLength / 2) - (tileYSide / 2) + j];
                GameObject tile = new GameObject(); //Создание самого тайла
                tile.transform.position = new Vector3(i * tileSize, -j * tileSize, 0); //Установка позиции
                tile.transform.parent = mapHolder.transform;
                tile.AddComponent<SpriteRenderer>();
                tile.GetComponent<SpriteRenderer>().sprite = grassSprites[sprite]; //Установка спрайта
                tile.name = "tile(" + i + ")(" + j + ")";
                currentMap[i][j] = tile;
            }
        }
        //Центрирование карты
        mapHolder.transform.position = new Vector3(-tileSize * tileXSide / 2, tileSize * tileYSide / 2, 0);
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        playerCurrentX = player.player.transform.position.x;
        playerCurrentY = player.player.transform.position.y;
    }

    //Загрузка карты
    void GenerateMap(int[][] matrix)
    {
        int sprite; //Переменная для подбора 
        //Заполнение матрицы карты
        map = new int[mapSideLength][];
        for (int i = 0; i < mapSideLength; i++)
        {
            map[i] = new int[mapSideLength];
            for (int j = 0; j < mapSideLength; j++)
            {
                map[i][j] = matrix[i][j];
            }
        }

        //Создание тайлов карты
        for (int i = 0; i < tileXSide; i++)
        {
            for (int j = 0; j < tileYSide; j++)
            {
                sprite = map[(mapSideLength / 2) - (tileXSide / 2) + i][(mapSideLength / 2) - (tileYSide / 2) + j];
                GameObject tile = new GameObject(); //Создание самого тайла
                tile.transform.position = new Vector3(i * tileSize, -j * tileSize, 0); //Установка позиции
                tile.transform.parent = mapHolder.transform;
                tile.AddComponent<SpriteRenderer>();
                tile.GetComponent<SpriteRenderer>().sprite = grassSprites[sprite]; //Установка спрайта
                tile.name = "tile(" + i + ")(" + j + ")";
                currentMap[i][j] = tile;
            }
        }
        //Центрирование карты
        mapHolder.transform.position = new Vector3(-tileSize * tileXSide / 2, tileSize * tileYSide / 2, 0);
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        playerCurrentX = player.player.transform.position.x;
        playerCurrentY = player.player.transform.position.y;
    }

    //Исправление матрицы динамической карты (Данный вариант не дал желаемых результатов)
    /*void FixMap(int direction)
    {
        //Вниз
        if(direction == 0)
        {
            GameObject[][] tmp = new GameObject[freeRoamCoef][];
            //Выделение доп памяти для переноса плиток
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new GameObject[tileXSide];
            }
            for(int i = 0; i < tmp.Length; i++)
            {
                for(int j = 0; j < tmp[i].Length; j++)
                {
                    tmp[i][j] = currentMap[i][j];
                }
            }
            //Перезапись матрицы
            for(int i = 0; i < currentMap.Length - freeRoamCoef; i++)
            {
                for(int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i][j] = currentMap[i + freeRoamCoef][j];
                }
            }
            //Дозапись переноса в матрицу
            for(int i = 0; i < tmp.Length; i++)
            {
                for(int j = 0; j < tmp[i].Length; j++)
                {
                    currentMap[currentMap[i].Length - 1 - freeRoamCoef + i][j] = tmp[i][j];
                }
            }
            //Переопределение координат внутри mapHolder
            for(int i = 0; i < currentMap.Length; i++)
            {
                for(int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i][j].transform.localPosition = new Vector3(i * tileSize - tileSize * tileXSide / 2, -j * tileSize + tileSize * tileYSide / 2, 0);
                }
            }
        }
        //Вправо
        else if(direction == 1)
        {
            GameObject[][] tmp = new GameObject[tileYSide][];
            //Выделение доп памяти для переноса плиток
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new GameObject[freeRoamCoef];
            }
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    tmp[i][j] = currentMap[i][j];
                }
            }
            //Перезапись матрицы
            for (int i = 0; i < currentMap.Length; i++)
            {
                for (int j = 0; j < currentMap[i].Length - freeRoamCoef; j++)
                {
                    currentMap[i][j] = currentMap[i][j + freeRoamCoef];
                }
            }
            //Дозапись переноса в матрицу
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    currentMap[i][currentMap[i].Length - 1 - freeRoamCoef + j] = tmp[i][j];
                }
            }
            //Переопределение координат внутри mapHolder
            for (int i = 0; i < currentMap.Length; i++)
            {
                for (int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i][j].transform.localPosition = new Vector3(i * tileSize - tileSize * tileXSide / 2, -j * tileSize + tileSize * tileYSide / 2, 0);
                }
            }
        }
        //Вверх
        else if(direction == 2)
        {
            GameObject[][] tmp = new GameObject[tileYSide][];
            //Выделение доп памяти для переноса плиток
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new GameObject[freeRoamCoef];
            }
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    tmp[i][j] = currentMap[currentMap.Length - 1 - freeRoamCoef + i][j];
                }
            }
            //Перезапись матрицы
            for (int i = currentMap.Length - freeRoamCoef - 1; i >= 0 ; i--)
            {
                for (int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i + freeRoamCoef][j] = currentMap[i][j];
                }
            }
            //Дозапись переноса в матрицу
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    currentMap[i][j] = tmp[i][j];
                }
            }
            //Переопределение координат внутри mapHolder
            for (int i = 0; i < currentMap.Length; i++)
            {
                for (int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i][j].transform.localPosition = new Vector3(i * tileSize - tileSize * tileXSide / 2, -j * tileSize + tileSize * tileYSide / 2, 0);
                }
            }
        }
        //Влево
        else if(direction == 3)
        {
            GameObject[][] tmp = new GameObject[tileYSide][];
            //Выделение доп памяти для переноса плиток
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new GameObject[freeRoamCoef];
            }
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    tmp[i][j] = currentMap[i][currentMap[i].Length - freeRoamCoef - 1 + j];
                }
            }
            //Перезапись матрицы
            for (int i = 0; i < currentMap.Length; i++)
            {
                for (int j = currentMap[i].Length - freeRoamCoef - 1; j >= 0 ; j++)
                {
                    currentMap[i][j + freeRoamCoef] = currentMap[i][j];
                }
            }
            //Дозапись переноса в матрицу
            for (int i = 0; i < tmp.Length; i++)
            {
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    currentMap[i][j] = tmp[i][j];
                }
            }
            //Переопределение координат внутри mapHolder
            for (int i = 0; i < currentMap.Length; i++)
            {
                for (int j = 0; j < currentMap[i].Length; j++)
                {
                    currentMap[i][j].transform.localPosition = new Vector3(i * tileSize - tileSize * tileXSide / 2, -j * tileSize + tileSize * tileYSide / 2, 0);
                }
            }
        }
    }
    */
    //Перемещение и перекраска карты за игроком
    void FixedUpdate()
    {
        if (player != null)
        {
            //Смещение карты вправо
            if (player.player.transform.position.x - playerCurrentX >= freeRoamCoef * tileSize)
            {
                playerCurrentX += freeRoamCoef * tileSize;
                //FixMap(1);
                for (int i = 0; i < currentMap.Length; i++)
                {
                    for (int j = 0; j < currentMap[i].Length; j++)
                    {
                        currentMap[i][j].GetComponent<SpriteRenderer>().sprite = grassSprites[map[(mapSideLength / 2) - (tileXSide / 2) + (int)(playerCurrentX / tileSize) + i][(mapSideLength / 2) - (tileYSide / 2) - (int)(playerCurrentY / tileSize) + j]];
                    }
                }
                mapHolder.transform.position = new Vector3(mapHolder.transform.position.x + freeRoamCoef * tileSize, mapHolder.transform.position.y, 0);
            }
            //Смещение карты влево
            else if (player.player.transform.position.x - playerCurrentX <= -freeRoamCoef * tileSize)
            {

                playerCurrentX -= freeRoamCoef * tileSize;
                //FixMap(3);
                for (int i = 0; i < currentMap.Length; i++)
                {
                    for (int j = 0; j < currentMap[i].Length; j++)
                    {
                        currentMap[i][j].GetComponent<SpriteRenderer>().sprite = grassSprites[map[(mapSideLength / 2) - (tileXSide / 2) + (int)(playerCurrentX / tileSize) + i][(mapSideLength / 2) - (tileYSide / 2) - (int)(playerCurrentY / tileSize) + j]];
                    }
                }
                mapHolder.transform.position = new Vector3(mapHolder.transform.position.x - freeRoamCoef * tileSize, mapHolder.transform.position.y, 0);
            }
            //Смещение карты вверх
            if (player.player.transform.position.y - playerCurrentY >= freeRoamCoef * tileSize)
            {
                playerCurrentY += freeRoamCoef * tileSize;
                //FixMap(2);
                for (int i = 0; i < currentMap.Length; i++)
                {
                    for (int j = 0; j < currentMap[i].Length; j++)
                    {
                        currentMap[i][j].GetComponent<SpriteRenderer>().sprite = grassSprites[map[(mapSideLength / 2) - (tileXSide / 2) + (int)(playerCurrentX / tileSize) + i][(mapSideLength / 2) - (tileYSide / 2) - (int)(playerCurrentY / tileSize) + j]];
                    }
                }
                mapHolder.transform.position = new Vector3(mapHolder.transform.position.x, mapHolder.transform.position.y + freeRoamCoef * tileSize, 0);

            }
            //Смещение карты вниз
            else if (player.player.transform.position.y - playerCurrentY <= -freeRoamCoef * tileSize)
            {
                playerCurrentY -= freeRoamCoef * tileSize;
                //FixMap(0);
                for (int i = 0; i < currentMap.Length; i++)
                {
                    for (int j = 0; j < currentMap[i].Length; j++)
                    {
                        currentMap[i][j].GetComponent<SpriteRenderer>().sprite = grassSprites[map[(mapSideLength / 2) - (tileXSide / 2) + (int)(playerCurrentX / tileSize) + i][(mapSideLength / 2) - (tileYSide / 2) - (int)(playerCurrentY / tileSize) + j]];
                    }
                }
                mapHolder.transform.position = new Vector3(mapHolder.transform.position.x, mapHolder.transform.position.y - freeRoamCoef * tileSize, 0);

            }
        }
    }
}