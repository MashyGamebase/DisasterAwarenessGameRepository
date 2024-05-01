using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0f;
    }

    public void OnClick_Start()
    {
        Time.timeScale = 1f;
        SequenceManager.instance.ProceedSequence();
    }
}