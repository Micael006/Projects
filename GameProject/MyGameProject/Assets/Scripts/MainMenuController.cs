using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using System;

public class MainMenuController : MonoBehaviour
{
    Sprite[] animationSprites;
    bool isLoadingScreen;
    bool isInLoadMenu;
    bool isInChooseMenu;
    int lastFileChoice;
    string lastFilePath;
    public GameObject loadFilePrefab;
    public Coroutine control;

    private void Awake()
    {
        animationSprites = new Sprite[10];
        for (int i = 0; i < animationSprites.Length; i++)
        {
            animationSprites[i] = Resources.Load<Sprite>("Sprites/GUI/LoadingScreen/StartGameAnimation/startGameAnimation" + i);
        }
        isLoadingScreen = false;
        isInLoadMenu = false;
        lastFileChoice = -1;
        lastFilePath = "";
        ShowGUI();
        HideLoadMenu();
        HideChooseMenu();
        if(control != null)
        {
            StopCoroutine(control);
        }
        control = StartCoroutine(CheckInput());
    }
    
    public void ShowGUI()
    {
        //Восстановление иконки персонажа
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(600, -300, 0);
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(222, 198);
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = animationSprites[0];
        gameObject.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Новая игра";
        gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "Загрузить игру";
        gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().text = "Выйти из игры";
        gameObject.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f); ;
        gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
    }
    public void NewGame()
    {
        if (isLoadingScreen)
            return;
        StartCoroutine(StartGame(0));
        isLoadingScreen = true;
    }

    public void LoadGame()
    {
        if (isLoadingScreen)
            return;
        StartCoroutine(StartGame(1));
        isLoadingScreen = true;
        //DataHolder.choice = 1;
        //DataHolder.lastFileChoice = 
    }

    public void LeaveGame()
    {
        if (isLoadingScreen)
            return;
        Application.Quit();
    }

    public void HideLoadMenu()
    {
        gameObject.transform.GetChild(4).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isInLoadMenu = false;
    }

    public void ShowLoadMenu()
    {
        if (isLoadingScreen)
            return;
        gameObject.transform.GetChild(4).GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        isInLoadMenu = true;
        FixLoadList();
        gameObject.transform.GetChild(4).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "\n\n\n\n\n\n\nВыберите файл";
    }

    public void HideChooseMenu()
    {
        gameObject.transform.GetChild(5).GetComponent<RectTransform>().localPosition = new Vector3(10000, 0, 0);
        isInChooseMenu = false;
    }

    public void ShowChooseMenu()
    {
        if (isLoadingScreen)
            return;
        gameObject.transform.GetChild(5).GetComponent<RectTransform>().localPosition = Vector3.zero;
        isInChooseMenu = true;
    }

    public void ChooseMapSize(int id)
    {
        DataHolder.mapSideLength = 512 * (int)Mathf.Pow(2, id);
        HideChooseMenu();
        NewGame();
    }
    IEnumerator StartGame(int choice)
    {
        HideLoadMenu();
        //Передвижение иконки игрока к центру меню
        for(int i = 0; i < 255; i++)
        {
            gameObject.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color (1f, 1f, 1f, Mathf.Max(1f - 2f*i / 255, 0));
            gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Max(1f - 2f * i / 255, 0));
            gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Max(1f - 2f * i / 255, 0));
            gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition += new Vector3(-600f / 255, 300f / 255, 0);
            gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(148f / 255, 132f / 255);
            yield return new WaitForSeconds(2f / 255);
        }
        for(int i = 0; i < 9; i++)
        {
            for(float j = 0; j < Mathf.PI; j += 0.1f * Mathf.PI)
            {
                gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3( - 50f * Mathf.Sin(j), 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = animationSprites[i + 1];
            for (float j = Mathf.PI; j <= 2f * Mathf.PI + 0.01f; j += 0.1f * Mathf.PI)
            {
                gameObject.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3( - 50f * Mathf.Sin(j), 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.25f);
        }
        if (choice == 0)
        {
            DataHolder.choice = 0;
            SceneManager.LoadScene(1);
        }
        else if(choice == 1)
        {
            DataHolder.choice = 1;
            LoadChosenGame();
            SceneManager.LoadScene(1);
        }
    }



    public void LoadChosenGame()
    {
        //Десериализация данных
        DataHolder.lastFileChoice = lastFileChoice;
        DataHolder.lastFilePath = lastFilePath;
    }

    public void ChooseGame(int id)
    {
        lastFileChoice = id;
        lastFilePath = DataHolder.savesPath + gameObject.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(id).GetChild(0).GetChild(0).GetComponent<Text>().text + ".xml";
        //Временная десериализация для получения данных
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllDataHolder));
        using (FileStream fs = new FileStream(lastFilePath, FileMode.OpenOrCreate))
        {
            DataHolder.aDH = xmlSerializer.Deserialize(fs) as AllDataHolder;
        }
        gameObject.transform.GetChild(4).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text =
        "Имя файла\n" +
        gameObject.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(id).GetChild(0).GetChild(0).GetComponent<Text>().text + "\n" +
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
        lastFilePath = "";
        FixLoadList();
    }

    public void FixLoadList()
    {
        foreach (Transform go in gameObject.transform.GetChild(4).GetChild(0).GetChild(0).GetComponentInChildren<Transform>())
        {
            Destroy(go.gameObject);
        }
        try
        {
            var saves = Directory.EnumerateFiles(DataHolder.savesPath, "*.xml", SearchOption.AllDirectories);
            int count = 0;
            foreach (string currentFile in saves)
            {
                int choice = count;
                GameObject item = Instantiate(loadFilePrefab);
                item.transform.SetParent(gameObject.transform.GetChild(4).GetChild(0).GetChild(0));
                item.GetComponent<Button>().onClick.AddListener(() => gameObject.GetComponent<MainMenuController>().ChooseGame(choice));
                item.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = currentFile.Substring(DataHolder.savesPath.Length, currentFile.Length - 4 - DataHolder.savesPath.Length);
                DateTime dateTime = File.GetCreationTime(currentFile);
                item.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Дата создания: " + dateTime.Date.Day.ToString() + "/" + dateTime.Date.Month.ToString() + "/" + dateTime.Date.Year.ToString();
                item.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Время создания: " + dateTime.TimeOfDay.Hours.ToString() + ":" + ((dateTime.TimeOfDay.Minutes < 10) ? "0" + dateTime.TimeOfDay.Minutes.ToString() : dateTime.TimeOfDay.Minutes.ToString()); //+ ":" + dateTime.TimeOfDay.TotalSeconds.ToString();
                count++;
            }
            StartCoroutine(SetScroll());
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        lastFileChoice = -1;
    }

    IEnumerator SetScroll()
    {
        yield return new WaitForSeconds(DataHolder.fixedFrameTime);
        gameObject.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Scrollbar>().value = 1f;
    }
    IEnumerator CheckInput()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Escape) && !isLoadingScreen)
            {
                if (isInLoadMenu)
                {
                    HideLoadMenu();
                }
                else if (isInChooseMenu)
                {
                    HideChooseMenu();
                }
                yield return new WaitForSeconds(0.18f);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
}
