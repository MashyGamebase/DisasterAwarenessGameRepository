using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

[System.Serializable]
public class Sequence
{
    public GameObject[] objectsToEnable;
    public GameObject[] objectsToDisable;

    [Tooltip("For the sequence header text")]
    public string headerText;
    [Tooltip("For the positive choice text button")]
    public string positiveChoiceText;
    [Tooltip("For the negative choice text button")]
    public string negativeChoiceText;

    [Tooltip("Should the next sequence have a game over trigger")]
    public bool isNextGameOver;

    [Tooltip("Should the next sequence fade")]
    public bool shouldFade;

    [Tooltip("Next position of the camera")]
    public Vector3 positionNext;
    [Tooltip("Next rotation of the camera")]
    public Vector3 rotationNext;

    [Tooltip("Text for the remark at the end of the game")]
    public string remarkText;
}


/// <summary>
/// Used for Game Event Sequencing
/// </summary>
public class SequenceManager : MonoBehaviour
{
    public GameObject cameraObject;
    public int currentSequence;
    public int targetSequence;
    public List<Sequence> sequences;
    public float sequencePhaseNextSpeed = 0.5f;
    public int numWaypoints = 10;

    public static SequenceManager instance;

    public bool isProceeding = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        targetSequence = sequences.Count;
    }

    public void ProceedSequence()
    {
        HybridCameraController.instance.SequenceHiderButton.interactable = false;
        if (currentSequence >= targetSequence)
        {
            HybridCameraController.instance.GameOver();
            return;
        }

        isProceeding = true;

        if (sequences[currentSequence].shouldFade)
        {
            HybridCameraController.instance.StartFade();
        }

        Vector3[] waypoints = new Vector3[numWaypoints];
        for (int i = 0; i < numWaypoints; i++)
        {
            float t = (float)i / (numWaypoints - 1);
            waypoints[i] = Vector3.Lerp(cameraObject.transform.position, sequences[currentSequence].positionNext, t);
        }

        // Move smoothly along the waypoints to the next position
        cameraObject.transform.DOPath(waypoints, sequencePhaseNextSpeed, PathType.Linear, PathMode.Full3D)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            cameraObject.transform.DORotate(new Vector3(sequences[currentSequence].rotationNext.x, sequences[currentSequence].rotationNext.y, sequences[currentSequence].rotationNext.z), sequencePhaseNextSpeed)
            .SetEase(Ease.OutQuint)
            .OnComplete(() =>
            {
                HybridCameraController.instance.SetTextNext(sequences[currentSequence].headerText, sequences[currentSequence].positiveChoiceText, sequences[currentSequence].negativeChoiceText, sequences[currentSequence].isNextGameOver);
                HybridCameraController.instance.SequenceHiderButton.interactable = true;
                isProceeding = false;
                currentSequence++;
            });
        });

        if (sequences[currentSequence].objectsToEnable.Length > 0)
        {
            foreach (GameObject obj in sequences[currentSequence].objectsToEnable)
            {
                obj.SetActive(true);
            }
        }

        if (sequences[currentSequence].objectsToDisable.Length > 0)
        {
            foreach(GameObject obj in sequences[currentSequence].objectsToDisable)
            {
                obj.SetActive(false);
            }
        }

    }

    #region GIZMOS_VISUALS
    private void OnDrawGizmos()
    {
        foreach(var sequence in sequences)
        {
            // Calculate the rotation from the given Euler angles
            Quaternion rotation = Quaternion.Euler(sequence.rotationNext);

            // Set the gizmo matrix to use the rotation
            Gizmos.matrix = Matrix4x4.TRS(sequence.positionNext, rotation, Vector3.one);

            // Draw a "camera" view frustum using Gizmos
            // Draw the camera "lens"
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);

            // Draw the camera "frustum"
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, new Vector3(0.5f, 0.5f, 1));
            Gizmos.DrawLine(Vector3.zero, new Vector3(-0.5f, 0.5f, 1));
            Gizmos.DrawLine(Vector3.zero, new Vector3(0.5f, -0.5f, 1));
            Gizmos.DrawLine(Vector3.zero, new Vector3(-0.5f, -0.5f, 1));
            Gizmos.DrawLine(new Vector3(0.5f, 0.5f, 1), new Vector3(-0.5f, 0.5f, 1));
            Gizmos.DrawLine(new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, -0.5f, 1));
            Gizmos.DrawLine(new Vector3(-0.5f, -0.5f, 1), new Vector3(0.5f, -0.5f, 1));
            Gizmos.DrawLine(new Vector3(-0.5f, -0.5f, 1), new Vector3(-0.5f, 0.5f, 1));
        }
    }
    #endregion
}