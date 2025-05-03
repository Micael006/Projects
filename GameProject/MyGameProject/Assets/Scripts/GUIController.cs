using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    GameObject player;
    GameObject playerGUI;
    GameObject Menu;
    public GameObject loadFilePrefab;
    Sprite stopIcon;
    Sprite startIcon;
    Sprite exitIcon;
    Sprite[] potions;
    Sprite[] endGameAnimation;
    List<int> choices;
    int lastChoice;
    int lastFileChoice;
    string lastFilePath;
    public bool isInMainMenu;
    public bool isInSaveMenu;
    public bool isInLoadMenu;
    public bool isLeaving;
    public bool isInBonusMenu;
    public bool isLoadingScreen;
    public bool isLastPanel;
    public Coroutine control;
    public Coroutine loadingScreenControl;
    public Coroutine sleepControl;
    public Coroutine lastPanelControl;
    public AudioSource mainTheme;
    public AudioSource deathTheme;
    private void Awake()
    {
        if(DataHolder.choice == 0)
        {
            ResetNewGame();
        }
        else if(DataHolder.choice == 1)
        {
            ResetLoadGame();
        }
    }

    public void ChangeGameStatus()
    {
        if (player.GetComponent<PlayerControl>().gameStopped)
        {
            player.GetComponent<PlayerControl>().StartGame();
            playerGUI.transform.GetChild(0).GetChild(0).GetComponent<Button>().image.sprite = stopIcon;
            HideMenu();
            HideSaveMenu();
            HideLoadMenu();
            HideLeaveMenu();
        }
        else
        {
            player.GetComponent<PlayerControl>().StopGame();
            playerGUI.transform.GetChild(0).GetChild(0).GetComponent<Button>().image.sprite = startIcon;
            ShowMenu();
        }
    }

    public void FixPlayerGUI()
    {
        playerGUI.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = player.GetComponent<PlayerControl>().damage.ToString();
        playerGUI.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = Math.Round(player.GetComponent<PlayerControl>().Speed / DataHolder.fixedFrameTime, 2).ToString(); ;
        playerGUI.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = player.GetComponent<PlayerControl>().score.ToString();
        playerGUI.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = player.GetComponent<PlayerControl>().curXP.ToString() + " / " + player.GetComponent<PlayerControl>().maxXP.ToString();
        playerGUI.transform.GetChild(2).GetChild(2).GetChild(2).GetComponent<Text>().text = player.GetComponent<PlayerControl>().level.ToString();
        playerGUI.transform.GetChild(3).GetComponent<Text>().text = player.GetComponent<PlayerControl>().curHealth.ToString() + " / " + player.GetComponent<PlayerControl>().maxHealth.ToString();
    }

    public void HidePlayerGUI()
    {
        playerGUI.GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
    }

    public void ShowPlayerGUI()
    {
        playerGUI.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }
    public void HideMenu()
    {
        Menu.GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isInMainMenu = false;
    }

    public void ShowMenu()
    {
        Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isInMainMenu = true;
    }

    public void HideSaveMenu()
    {
        gameObject.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isInSaveMenu = false;
    }

    public void ShowSaveMenu()
    {
        gameObject.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isInSaveMenu = true;
        gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text = "";
    }

    public void HideLoadMenu()
    {
        gameObject.transform.GetChild(3).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isInLoadMenu = false;
        ShowPlayerGUI();
    }

    public void ShowLoadMenu()
    {
        gameObject.transform.GetChild(3).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isInLoadMenu = true;
        FixLoadList();
        gameObject.transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "\n\n\n\n\n\n\nВыберите файл";
        HidePlayerGUI();
    }

    public void HideLeaveMenu()
    {
        gameObject.transform.GetChild(4).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isLeaving = false;
    }

    public void ShowLeaveMenu()
    {
        gameObject.transform.GetChild(4).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isLeaving = true;
    }

    public void HideBonusMenu()
    {
        gameObject.transform.GetChild(5).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        player.GetComponent<PlayerControl>().gameStopped = false;
        isInBonusMenu = false;

        choices = new List<int>() { -1, -1, -1 };
    }

    public void ShowBonusMenu()
    {
        gameObject.transform.GetChild(5).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isInBonusMenu = true;
        player.GetComponent<PlayerControl>().gameStopped = true;
        gameObject.transform.GetChild(5).GetChild(1).GetChild(0).GetComponent<Text>().text = "";
        HideConfirmButton();
        CreateChoices();
    }

    public void HideConfirmButton()
    {
        gameObject.transform.GetChild(5).GetChild(5).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
    }

    public void ShowConfirmButton()
    {
        gameObject.transform.GetChild(5).GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -350, 0);
    }

    public void HideLoadingScreen()
    {
        gameObject.transform.GetChild(6).GetComponent<RectTransform>().localPosition = new Vector3(100000, 0, 0);
        player.GetComponent<PlayerControl>().StartGame();
        isLoadingScreen = false;
    }

    public void ShowLoadingScreen()
    {
        gameObject.transform.GetChild(6).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        player.GetComponent <PlayerControl>().StopGame();
        isLoadingScreen = true;
    }
    public void GoToMainMenu()
    {
        DataHolder.choice = -1;
        SceneManager.LoadScene(0);
    }
    public void SaveGame()
    {
        string saveName = gameObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text + ".xml";
        string path = DataHolder.savesPath + saveName;
        int helper = 1;
        if (File.Exists(path))
        {
            while (File.Exists(path.Substring(0, path.Length - 4) + "("  + helper.ToString() + ").xml"))
            {
                helper++;
            }
            path = path.Substring(0, path.Length - 4) + "(" + helper.ToString() + ").xml";
        }
        //public static GameObject[] enemies;
        //public static int[] activeEnemies;
        //Данные генератора
        float enemiesHealth = player.transform.GetChild(0).GetComponent<EnemiesGenerator>().startHealth;
        //Данные игрока
        float playerSpeed = player.GetComponent<PlayerControl>().Speed / DataHolder.fixedFrameTime;
        float playerMovementBorder = player.GetComponent<PlayerControl>().border;
        int playerCurrentHealth = player.GetComponent<PlayerControl>().curHealth;
        int playerMaxHealth = player.GetComponent<PlayerControl>().maxHealth;
        int playerDamage = player.GetComponent<PlayerControl>().damage;
        int playerScore = player.GetComponent<PlayerControl>().score;
        int playerCurrentXP = player.GetComponent<PlayerControl>().curXP;
        int playerMaxXP = player.GetComponent<PlayerControl>().maxXP;
        int playerLevel = player.GetComponent<PlayerControl>().level;
        //Данные карты
        int mapLength = DataHolder.mapSideLength;
        int[][] mapMatrix = new int[mapLength][];
        for(int i = 0; i < mapLength; i++)
        {
            mapMatrix[i] = new int[mapLength];
            for(int j = 0; j < mapLength; j++)
            {
                mapMatrix[i][j] = player.transform.GetChild(1).GetComponent<MapGenerator>().map[i][j];
            }
        }
        //Объект сериализации
        DataHolder.aDH = new AllDataHolder(enemiesHealth, playerSpeed, playerMovementBorder, playerCurrentHealth, playerMaxHealth, playerDamage, playerScore, playerCurrentXP, playerMaxXP, playerLevel, mapLength, mapMatrix);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllDataHolder));
        //получаем поток, куда будем записывать
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            xmlSerializer.Serialize(fs, DataHolder.aDH);
        }
        HideSaveMenu();
    }

    public void LoadGame()
    {
        //Десериализация данных
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllDataHolder));
        using (FileStream fs = new FileStream(lastFilePath, FileMode.OpenOrCreate))
        {
            DataHolder.aDH = xmlSerializer.Deserialize(fs) as AllDataHolder;
        }
        ResetLoadGame();
        HideLoadMenu();
    }

    public void ChooseGame(int id)
    {
        lastFileChoice = id;
        lastFilePath = DataHolder.savesPath + gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(id).GetChild(0).GetChild(0).GetComponent<Text>().text + ".xml";
        //Временная десериализация для получения данных
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllDataHolder));
        using (FileStream fs = new FileStream(lastFilePath, FileMode.OpenOrCreate))
        {
            DataHolder.aDH = xmlSerializer.Deserialize(fs) as AllDataHolder;
        }
        gameObject.transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text =
        "Имя файла\n" +
        gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(id).GetChild(0).GetChild(0).GetComponent<Text>().text + "\n" +
        "\n" +
        "Характеристики игрока\n" +
        "ATK: " + DataHolder.aDH.playerDamage.ToString() + "\n" +
        "SPD: " + DataHolder.aDH.playerSpeed.ToString() + "\n" +
        "ЖИЗНИ: " + DataHolder.aDH.playerCurrentHealth.ToString() + "/" + DataHolder.aDH.playerMaxHealth.ToString() + "\n" +
        "LVL: " + DataHolder.aDH.playerLevel.ToString() + "\n" +
        "XP: " + DataHolder.aDH.playerCurrentXP.ToString() + "/" + DataHolder.aDH.playerMaxXP.ToString() + "\n" +
        "Очки: " + DataHolder.aDH.playerScore.ToString() + "\n" +
        "\n" +
        "Размер карты\n" +
        ((DataHolder.aDH.mapLength == 512) ? "Маленькая" : (DataHolder.aDH.mapLength == 1024) ? "Средняя" : "Большая");
    }

    public void DeleteGame()
    {
        File.Delete(lastFilePath);
        lastFileChoice = -1;
        FixLoadList();
    }

    public void FixLoadList()
    {
        foreach(Transform go in gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetComponentInChildren<Transform>())
        {
            Destroy(go.gameObject);
        }
        try
        {
            var saves = Directory.EnumerateFiles(DataHolder.savesPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            foreach(string currentFile in saves)
            {
                int choice = count;
                GameObject item = Instantiate(loadFilePrefab);
                item.transform.SetParent(gameObject.transform.GetChild(3).GetChild(0).GetChild(0));
                item.GetComponent<Button>().onClick.AddListener(() => gameObject.GetComponent<GUIController>().ChooseGame(choice));
                item.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = currentFile.Substring(DataHolder.savesPath.Length, currentFile.Length - 4 - DataHolder.savesPath.Length);
                DateTime dateTime = File.GetCreationTime(currentFile);
                item.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Дата создания: " + dateTime.Date.Day.ToString() + "/" + dateTime.Date.Month.ToString() + "/" + dateTime.Date.Year.ToString();
                item.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Время создания: " + dateTime.TimeOfDay.Hours.ToString() + ":" + ((dateTime.TimeOfDay.Minutes < 10) ? "0" + dateTime.TimeOfDay.Minutes.ToString() : dateTime.TimeOfDay.Minutes.ToString()); //+ ":" + dateTime.TimeOfDay.TotalSeconds.ToString();
                count++;
            }
            StartCoroutine(SetScroll());//new Vector3(0, -69 * (count - 1), 0);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        lastFileChoice = -1;
    }

    IEnumerator SetScroll()
    {
        yield return new WaitForSeconds(DataHolder.fixedFrameTime);
        gameObject.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<Scrollbar>().value = 1f;
    }

    public void CreateChoices()
    {
        System.Random random = new System.Random();
        for(int i = 0; i < choices.Count; i++)
        {
            int helper = random.Next(potions.Length);
            while(choices.IndexOf(helper) != -1)
            {
                helper = random.Next(potions.Length);
            }
            choices[i] = helper;
            gameObject.transform.GetChild(5).GetChild(2 + i).GetChild(0).GetComponent<Image>().sprite = potions[choices[i]];
        }
    }

    public void MakeChoice(int id)
    {
        string helper = "";
        lastChoice = id;
        if(choices[id] < 3)
        {
            helper = "Вы увеличите урон на " + ((choices[id] + 1) * 10) + " единиц"; 
        }
        else if(choices[id] < 6)
        {
            helper = "Вы получите ещё " + ((choices[id] % 3) + 1) + " жизни";
        }
        else
        {
            helper = "Вы увеличите скорость на " + ((choices[id] == 6) ? 0.02f : (choices[id] == 7) ? 0.05f : 0.1f) + " тайлов/сек";
        }
        gameObject.transform.GetChild(5).GetChild(1).GetChild(0).GetComponent<Text>().text = helper;
        ShowConfirmButton();
    }

    public void ConfirmChoice()
    {
        if (choices[lastChoice] < 3)
        {
            player.GetComponent<PlayerControl>().damage += ((choices[lastChoice] + 1) * 10);
        }
        else if (choices[lastChoice] < 6)
        {
            player.GetComponent<PlayerControl>().maxHealth += ((choices[lastChoice] % 3) + 1);
            player.GetComponent<PlayerControl>().curHealth += ((choices[lastChoice] % 3) + 1);
        }
        else
        {
            player.GetComponent<PlayerControl>().Speed += DataHolder.fixedFrameTime * ((choices[lastChoice] == 6) ? 0.02f : (choices[lastChoice] == 7) ? 0.05f : 0.1f);
        }
        lastChoice = -1;
        FixPlayerGUI();
        HideBonusMenu();
    }

    public void ResetGUI()
    {
        if (control != null)
        {
            StopCoroutine(control);
        }
        if(loadingScreenControl == null)
        {
            loadingScreenControl = StartCoroutine(TextTransparencyChanger());
        }
        else
        {
            StopCoroutine(loadingScreenControl);
            loadingScreenControl = StartCoroutine(TextTransparencyChanger());
        }
        if (sleepControl == null)
        {
            sleepControl = StartCoroutine(SleepAnimation());
        }
        else
        {
            StopCoroutine(sleepControl);
            sleepControl = StartCoroutine(SleepAnimation());
        }
        if(lastPanelControl != null)
        {
            StopCoroutine(lastPanelControl);
        }
        control = null;
        player = GameObject.Find("Player");
        playerGUI = gameObject.transform.GetChild(0).gameObject;
        Menu = gameObject.transform.GetChild(1).gameObject;
        stopIcon = Resources.Load<Sprite>("Sprites/GUI/Buttons/ButtonIcons/StopIcon");
        startIcon = Resources.Load<Sprite>("Sprites/GUI/Buttons/ButtonIcons/ResumeIcon");
        exitIcon = Resources.Load<Sprite>("Sprites/GUI/Buttons/ButtonIcons/ExitIcon");
        potions = new Sprite[9];
        endGameAnimation = new Sprite[6];
        for (int i = 0; i < endGameAnimation.Length; i++)
        {
            endGameAnimation[i] = Resources.Load<Sprite>("Sprites/GUI/LoadingScreen/EndGameAnimation/endGameAnimation" + i);
        }
        choices = new List<int>() { -1, -1, -1 };
        lastChoice = -1;
        lastFileChoice = -1;
        lastFilePath = "";
        potions[0] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/AttackPotion/Red/AttackPotionRed");
        potions[1] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/AttackPotion/Green/AttackPotionGreen");
        potions[2] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/AttackPotion/Blue/AttackPotionBlue");
        potions[3] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/LifePotion/Red/LifePotionRed");
        potions[4] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/LifePotion/Green/LifePotionGreen");
        potions[5] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/LifePotion/Blue/LifePotionBlue");
        potions[6] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/SpeedPotion/Red/SpeedPotionRed");
        potions[7] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/SpeedPotion/Green/SpeedPotionGreen");
        potions[8] = Resources.Load<Sprite>("Sprites/GUI/BonusMenu/Potions/SpeedPotion/Blue/SpeedPotionBlue");
        isInMainMenu = false;
        isInSaveMenu = false;
        isInLoadMenu = false;
        isLeaving = false;
        isLoadingScreen = true;
        isLastPanel = false;
        control = StartCoroutine(CheckInput());
        mainTheme = gameObject.transform.GetChild(8).GetChild(0).GetComponent<AudioSource>();
        deathTheme = gameObject.transform.GetChild(8).GetChild(1).GetComponent<AudioSource>();
        mainTheme.Stop();
        deathTheme.Stop();
        HideMenu();
        HideSaveMenu();
        HideLoadMenu();
        HideLeaveMenu();
        HideBonusMenu();
        HideLastPanel();
        FixPlayerGUI();
    }

    public void ResetNewGame()
    {
        //Перезагрузка GUI
        ResetGUI();
        //Вызов экрана загрузки
        ShowLoadingScreen();
        gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = "";
        //Перезагрузка игрока
        player.GetComponent<PlayerControl>().ResetNewGame();
        //Перезагрузка карты
        player.transform.GetChild(1).GetComponent<MapGenerator>().ResetNewGame();
        //Перезагрузка генератора врагов
        player.transform.GetChild(0).GetComponent<EnemiesGenerator>().ResetNewGame();
        gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = "Нажмите Esc чтобы продолжить";
    }

    public void ResetLoadGame()
    {
        //Перезагрузка GUI
        ResetGUI();
        //Вызов экрана загрузки
        ShowLoadingScreen();
        gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = "";
        //Перезагрузка игрока
        player.GetComponent<PlayerControl>().ResetLoadGame();
        //Перезагрузка карты
        player.transform.GetChild(1).GetComponent<MapGenerator>().ResetLoadGame();
        //Перезагрузка генератора врагов
        player.transform.GetChild(0).GetComponent<EnemiesGenerator>().ResetLoadGame();
        gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = "Нажмите Esc чтобы продолжить";
    }

    IEnumerator CheckInput()
    {
        while (true)
        {
            bool helper = isInSaveMenu || isInLoadMenu || isLeaving || isInBonusMenu || isLoadingScreen || isLastPanel;
            if (Input.GetKey(KeyCode.Escape))
            {
                if (!helper)
                {
                    ChangeGameStatus();
                }
                else if (isInSaveMenu)
                {
                    HideSaveMenu();
                }
                else if (isInLoadMenu)
                {
                    HideLoadMenu();
                }
                else if (isLeaving)
                {
                    HideLeaveMenu();
                }
                else if (isLoadingScreen)
                {
                    ChangeGameStatus();
                    HideLoadingScreen();
                    PlayMainTheme();
                }
                yield return new WaitForSeconds(0.18f);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
    IEnumerator TextTransparencyChanger()
    {
        Color lastColor = gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().color;
        float step = 1f / 255f;
        while (true)
        {
            gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().color = new Color(lastColor.r, lastColor.g, lastColor.b, (lastColor.a + step));
            lastColor = gameObject.transform.GetChild(6).GetChild(1).GetComponent<Text>().color;
            step = (lastColor.a * 255f >= 255f) ? -1f / 255f : (lastColor.a * 255f <= 50f) ? 1 / 255f : step;
            yield return new WaitForSeconds(0.008f);
        }
    }
    IEnumerator SleepAnimation()
    {
        Transform body = gameObject.transform.GetChild(6).GetChild(0);
        float time = 1.5f;
        while (true)
        {
            //Вылетает первая Z
            StartCoroutine(ZMovement(0));
            yield return new WaitForSeconds(1f);
            //Вылетает вторая Z
            StartCoroutine(ZMovement(1));
            yield return new WaitForSeconds(1f);
            //Вылетает третья Z
            StartCoroutine(ZMovement(2));
            //Ожидание конца всех анимаций
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator ZMovement(int zNum)
    {
        gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().localPosition = new Vector3(-32, 72, 0);
        gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        //Расширяем с 0 до пи/2, сужаем с 3*пи/2 до 2*пи
        for(float i = 0; i < 0.5f * Mathf.PI; i += 0.02f * Mathf.PI)
        {
            gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().localPosition = new Vector3(-32 + 30 * Mathf.Sin(i), 72 + 20 * i, 0);
            gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().sizeDelta += new Vector2((float)48 / 25, (float)48 / 25);
            yield return new WaitForSeconds(0.02f);
        }
        for(float i = 0.5f * Mathf.PI; i < 1.5f * Mathf.PI; i += 0.02f * Mathf.PI)
        {
            gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().localPosition = new Vector3(-32 + 30 * Mathf.Sin(i), 72 + 20 * i, 0);
            yield return new WaitForSeconds(0.02f);
        }
        for(float i = 1.5f * Mathf.PI; i < 2f * Mathf.PI; i += 0.02f * Mathf.PI)
        {
            gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().localPosition = new Vector3(-32 + 30 * Mathf.Sin(i), 72 + 20 * i, 0);
            gameObject.transform.GetChild(6).GetChild(0).GetChild(zNum).GetComponent<RectTransform>().sizeDelta -= new Vector2((float)48 / 25, (float)48 / 25);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void HideLastPanel()
    {
        gameObject.transform.GetChild(7).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        player.GetComponent<PlayerControl>().gameStopped = false;
        isLastPanel = false;
    }
    
    public void ShowLastPanel()
    {
        //Делаем панель невидимой, чтобы начать её проявлять
        gameObject.transform.GetChild(7).GetComponent<Image>().color = new Color(70f / 255, 70f / 255, 70f / 255, 0f);
        gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().color = new Color(212f / 255f, 212f / 255, 212f / 255, 0f);
        gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().sprite = endGameAnimation[0];
        gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().text = "Игра окончена";
        gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().text = "Очки: " + player.GetComponent<PlayerControl>().score;
        gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color = new Color(212f / 255, 212f / 255, 212f / 255, 0f);
        gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color = new Color(212f / 255, 212f / 255, 212f / 255, 0f);
        gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "Вернуться в главное меню";
        gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        gameObject.transform.GetChild(7).GetChild(3).GetComponent<Button>().interactable = false;
        gameObject.transform.GetChild(7).GetChild(4).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        gameObject.transform.GetChild(7).GetChild(4).GetComponent<Text>().text = "Новая игра";
        gameObject.transform.GetChild(7).GetChild(5).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        gameObject.transform.GetChild(7).GetChild(5).GetComponent<Text>().text = "Загрузить игру";
        gameObject.transform.GetChild(7).GetChild(6).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        gameObject.transform.GetChild(7).GetChild(6).GetComponent<Text>().text = "Выйти из игры";
        //Приводим панель на сцену
        gameObject.transform.GetChild(7).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        player.GetComponent<PlayerControl>().gameStopped = true;
        isLastPanel = true;
        //Останавливаем фоновую музыку и играем звук смерти
        StopMainTheme();
        PlayDeathTheme();
        //Проявляем меню
        StartCoroutine(MakeLastPanelVisible());
    }

    public void MainMenuAfterDeath()
    {
        StartCoroutine(PrepareToReturn());
    }

    IEnumerator MakeLastPanelVisible()
    {
        //Делаем панель видимой
        Color lastColor1 = gameObject.transform.GetChild(7).GetComponent<Image>().color; //= new Color(70f / 255, 70f / 255, 70f / 255, 0f);
        Color lastColor2 = gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().color; //= new Color(212f / 255f, 212f / 255, 212f / 255, 0f);
        Color lastColor3 = gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color; //= new Color(212f / 255, 212f / 255, 212f / 255, 0f);
        Color lastColor4 = gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color; //= new Color(212f / 255, 212f / 255, 212f / 255, 0f);
        Color lastColor5 = gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color; //= new Color(1f, 1f, 1f, 0f);
        float step = 1f / 255f;
        while (step <= 1f)
        {
            gameObject.transform.GetChild(7).GetComponent<Image>().color = new Color(lastColor1.r, lastColor1.g, lastColor1.b, lastColor1.a + step);
            gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().color = new Color(lastColor2.r, lastColor2.g, lastColor2.b, lastColor2.a + step);
            gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color = new Color(lastColor3.r, lastColor3.g, lastColor3.b, lastColor3.a + step);
            gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color = new Color(lastColor4.r, lastColor4.g, lastColor4.b, lastColor4.a + step);
            gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(lastColor5.r, lastColor5.g, lastColor5.b, lastColor5.a + step);

            lastColor1 = gameObject.transform.GetChild(7).GetComponent<Image>().color; 
            lastColor2 = gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().color; 
            lastColor3 = gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color; 
            lastColor4 = gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color; 
            lastColor5 = gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color;
            step += 1f / 255;
            yield return new WaitForSeconds(0.008f);
        }
        gameObject.transform.GetChild(7).GetChild(3).GetComponent<Button>().interactable = true;
        lastPanelControl = StartCoroutine(LastPanelTextTransparencyChanger());
    }

    IEnumerator LastPanelTextTransparencyChanger()
    {
        Color lastColor = new Color (1f, 1f, 1f, 1f);
        float step = 1f / 255f;
        while (true)
        {
            gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(lastColor.r, lastColor.g, lastColor.b, (lastColor.a + step));
            lastColor = gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color;
            step = (lastColor.a * 255f >= 255f) ? -1f / 255f : (lastColor.a * 255f <= 50f) ? 1 / 255f : step;
            yield return new WaitForSeconds(0.008f);
        }
    }

    IEnumerator PrepareToReturn()
    {
        //Делаем текст невидимым
        gameObject.transform.GetChild(7).GetChild(3).GetComponent<Button>().interactable = false;
        StopCoroutine(lastPanelControl);
        Color lastColor1 = gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color; 
        Color lastColor2 = gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color; 
        Color lastColor3 = gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color;
        float step = 1f / 255f;

        while (step <= 1f)
        {
            
            gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color = new Color(lastColor1.r, lastColor1.g, lastColor1.b, lastColor1.a - step);
            gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color = new Color(lastColor2.r, lastColor2.g, lastColor2.b, lastColor2.a - step);
            gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(lastColor3.r, lastColor3.g, lastColor3.b, lastColor3.a - step);

            lastColor1 = gameObject.transform.GetChild(7).GetChild(1).GetComponent<Text>().color;
            lastColor2 = gameObject.transform.GetChild(7).GetChild(2).GetComponent<Text>().color;
            lastColor3 = gameObject.transform.GetChild(7).GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color;
            step += 1f / 255;
            yield return new WaitForSeconds(0.004f);
        }

        //Запускаем анимацию окаменения
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(7).GetChild(0).GetComponent<Image>().sprite = endGameAnimation[i + 1];
            yield return new WaitForSeconds(0.4f);
        }
        //Передвигаем иконку в угол
        for (int i = 0; i < 255; i++)
        {
            gameObject.transform.GetChild(7).GetChild(0).GetComponent<RectTransform>().localPosition -= new Vector3(-600f / 255, 300f / 255, 0);
            gameObject.transform.GetChild(7).GetChild(0).GetComponent<RectTransform>().sizeDelta -= new Vector2(148f / 255, 132f / 255);
            if(i > 127)
            {
                gameObject.transform.GetChild(7).GetChild(4).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Min((2f * i - 255f)/ 255, 1f));
                gameObject.transform.GetChild(7).GetChild(5).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Min((2f * i - 255f) / 255, 1f));
                gameObject.transform.GetChild(7).GetChild(6).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Min((2f * i - 255f) / 255, 1f));
            }
            yield return new WaitForSeconds(2f / 255);
        }
        GoToMainMenu();
    }

    public void PlayMainTheme()
    {
        mainTheme.Play();
        mainTheme.loop = true;
    }

    public void StopMainTheme()
    {
        mainTheme.Stop();
    }

    public void PlayDeathTheme()
    {
        deathTheme.Play();
    }
}
