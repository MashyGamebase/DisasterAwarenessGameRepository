using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;

    public void OnClick_LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
        source.PlayOneShot(clip);
    }
}