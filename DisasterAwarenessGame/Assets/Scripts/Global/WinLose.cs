using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLose : MonoBehaviour
{
    public static WinLose instance;
    public TextMeshProUGUI remarkText;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    public void SetRemark(string text)
    {
        remarkText.text = text;
    }

    public void OnClick_Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClick_Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}