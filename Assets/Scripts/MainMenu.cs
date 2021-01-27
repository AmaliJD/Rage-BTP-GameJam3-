using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button Play_Button;
    public GameObject score_grid;

    public Sprite star, nostar;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        Play_Button.onClick.AddListener(() => { Play(); });

        int i = 0;
        foreach (Transform levelscore in score_grid.transform)
        {
            i++;
            levelscore.GetChild(1).GetComponent<Image>().sprite = GlobalData.Stars[i] >= 1 ? star : nostar;
            levelscore.GetChild(0).GetComponent<Image>().sprite = GlobalData.Stars[i] >= 2 ? star : nostar;
            levelscore.GetChild(2).GetComponent<Image>().sprite = GlobalData.Stars[i] >= 3 ? star : nostar;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }

    public void Play()
    {
        GlobalData.Index = 1;
        SceneManager.LoadScene(GlobalData.Scene[GlobalData.Index]);
    }
}
