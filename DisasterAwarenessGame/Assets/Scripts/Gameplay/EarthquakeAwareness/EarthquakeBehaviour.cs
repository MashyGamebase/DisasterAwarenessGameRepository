using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeBehaviour : MonoBehaviour
{
    public bool isOn = false;


    private void OnEnable()
    {
        isOn = true;
        StartCoroutine(EarthquakeShake());
    }


    IEnumerator EarthquakeShake()
    {
        while (isOn)
        {
            bool isFinished = false;
            Camera.main.transform.DOShakePosition(1f, 0.05f)
                .OnComplete(() =>
                {
                    isFinished = true;
                });

            yield return new WaitUntil(() => isFinished);
            yield return new WaitUntil(() => !SequenceManager.instance.isProceeding);
        }
    }
}