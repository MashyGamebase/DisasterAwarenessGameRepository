using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject toDisable;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        toDisable.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        toDisable.SetActive(true);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}