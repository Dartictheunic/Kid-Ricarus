/*
Pour appeler une fonction il faut faire TimeManager.timemanager.Void(argument1, argument2...)

Si vous appelez une fonction vous arrêtez les autres (on va éviter de niquer trop le timeScale si possible mdr

Certaines fonctions existent en plusieurs format, si vous avez la flemme pour les animation curves il existe des presets (AnimationCurve.Constant par exemple)



Pour call un hitfreeze par exemple:

        if (Input.GetKeyDown(KeyCode.T))
        {
            TimeManager.timeManager.HitFreeze(3f);
        }


Débizous <3
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager timeManager;
    bool isRunningCoroutine;

    private void Awake()
    {
        timeManager = this;
    }

    public void HitFreeze(float freezeTime)
    {
        if (isRunningCoroutine)
        {
            StopAllCoroutines();
            StartCoroutine(Freeze(freezeTime));
        }

        else
        {
            StartCoroutine(Freeze(freezeTime));
        }
    }

    public void SlowTime(float newTimeScale, float slowTime)
    {
        if (isRunningCoroutine)
        {
            StopAllCoroutines();
            StartCoroutine(FixedSlowTime(newTimeScale, slowTime));
        }

        else
        {
            StartCoroutine(FixedSlowTime(newTimeScale, slowTime));
        }
    }

    public void SlowTime(float initialTimeScale, float timeScaleIncrement, AnimationCurve slowCurve)
    {
        if (isRunningCoroutine)
        {
            StopAllCoroutines();
            StartCoroutine(ProgressiveSlowTime(initialTimeScale, timeScaleIncrement, slowCurve));
        }

        else
        {
            StartCoroutine(ProgressiveSlowTime(initialTimeScale, timeScaleIncrement, slowCurve));
        }
    }

    IEnumerator Freeze(float freezeTime)
    {
        isRunningCoroutine = true;
        Time.timeScale = Mathf.Epsilon;
        yield return new WaitForSecondsRealtime(freezeTime);
        Time.timeScale = 1f;
        isRunningCoroutine = false;
    }


    IEnumerator FixedSlowTime(float newTimeScale, float slowTime)
    {
        isRunningCoroutine = true;
        Time.timeScale = newTimeScale;
        yield return new WaitForSecondsRealtime(slowTime);
        Time.timeScale = 1f;
        isRunningCoroutine = false;
    }

    IEnumerator ProgressiveSlowTime(float initialTimeScale, float timeScaleIncrement, AnimationCurve slowCurve)
    {
        isRunningCoroutine = true;
        float timer = 0f;
        Time.timeScale = initialTimeScale;
        while (Time.timeScale < 1f)
        {
            Time.timeScale += timeScaleIncrement * slowCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
        isRunningCoroutine = false;
    }


    private void Update()
    {
        if (isRunningCoroutine)
        {
            Debug.LogWarning("Timescale is actually : " + Time.timeScale);
        }
    }
}
