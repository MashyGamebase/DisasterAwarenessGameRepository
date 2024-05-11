using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum CameraState
{
    Freeform,
    Touch
}


public class HybridCameraController : MonoBehaviour
{
    //private bool GyroEnabled;
    //private Gyroscope Gyro;
    //public CameraState CameraState = CameraState.Freeform;

    private Vector2 touchStartPos;
    private Vector3 lastMousePos;

    [SerializeField] private float rotationSpeed = 2f;

    public static HybridCameraController instance;

    public GameObject cameraFader;
    public float fadeTime = 0.5f;
    public float fadeDuration = 1f;

    [Header("SequenceChoice")]
    public GameObject sequenceChoiceGO;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI positiveChoiceText;
    public TextMeshProUGUI negativeChoiceText;
    public Button SequenceHiderButton;

    [Header("OnGameEnd Canvas")]
    public GameObject winLoseCanvas;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (SequenceManager.instance.isProceeding) return;
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    // Calculate swipe distance
                    float swipeDistance = touch.position.x - touchStartPos.x;

                    // Apply rotation based on swipe distance
                    Vector3 delta = touch.position - touchStartPos;
                    RotateCamera(delta);
                    break;
            }
        }
#endif
#if UNITY_EDITOR
        MouseRotateCamera();
#endif
    }
    #region EDITOR_INPUT
    void MouseRotateCamera()
    {
        // Check for mouse button down to start dragging
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }

        // Check for mouse button held down to continue dragging
        if (Input.GetMouseButton(0))
        {
            // Calculate the difference in mouse position since last frame
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            // Rotate the camera based on the mouse movement
            RotateCamera(delta);
        }
    }
    #endregion
    void RotateCamera(Vector3 delta)
    {
        // Sensitivity factor for rotation
        float sensitivity = 0.05f;

        // Rotate the camera around the appropriate axis

        float newXRotation = transform.eulerAngles.x - delta.y * sensitivity;
        float newYRotation = transform.eulerAngles.y + delta.x * sensitivity;
        float clampedXRotation = Mathf.Clamp(newXRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(newXRotation, newYRotation, 0);
    }


    /*
    public void OnClick_SwitchCameraControl()
    {
        if (CameraState == CameraState.Freeform)
            CameraState = CameraState.Touch;
        else
            CameraState = CameraState.Freeform;
    }
    */

    public void StartFade()
    {
        StopCoroutine(startFadeCo());
        StartCoroutine(startFadeCo());
    }

    IEnumerator startFadeCo()
    {
        Color originalColor = cameraFader.GetComponent<Image>().color;
        float elapsedTime = 0f; // Elapsed time since the start of the fade

        // Fade in
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color lerpedColor = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 1f), lerpValue);
            cameraFader.GetComponent<Image>().color = lerpedColor;
            yield return null;
        }

        yield return new WaitForSeconds(fadeTime); // Wait for 1 second between fades

        elapsedTime = 0f; // Reset elapsed time for the next fade

        // Fade out
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color lerpedColor = Color.Lerp(new Color(originalColor.r, originalColor.g, originalColor.b, 1f), originalColor, lerpValue);
            cameraFader.GetComponent<Image>().color = lerpedColor;
            yield return null;
        }

        // Ensure that the final color is the original color
        cameraFader.GetComponent<Image>().color = originalColor;
    }

    private bool GameOverNext;

    public void SetTextNext(string header, string p_choice, string n_choice, bool n_gameOver = false)
    {
        headerText.text = header;
        positiveChoiceText.text = p_choice;
        negativeChoiceText.text = n_choice;
        GameOverNext = n_gameOver;
        sequenceChoiceGO.SetActive(true);
    }

    public void OnNextSequence_Positive()
    {
        GameOverNext = false;
        SequenceManager.instance.ProceedSequence();
    }

    public void OnNextSequence_Negative()
    {
        if (!GameOverNext)
            SequenceManager.instance.ProceedSequence();
        else
            GameOver();
    }

    public void GameOver()
    {
        winLoseCanvas.SetActive(true);
        winLoseCanvas.GetComponent<WinLose>().SetRemark(SequenceManager.instance.sequences[SequenceManager.instance.currentSequence - 1].remarkText);
    }

    public void ShowHideSequence()
    {
        source.PlayOneShot(clip);
        if (sequenceChoiceGO.gameObject.activeInHierarchy)
        {
            sequenceChoiceGO.SetActive(false);
        }
        else
        {
            sequenceChoiceGO.SetActive(true);
        }
    }
}