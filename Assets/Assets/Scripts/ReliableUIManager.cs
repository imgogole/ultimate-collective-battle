using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ReliableUIManager : MonoBehaviour
{
    public float FadeBlackTime = 1f;
    public CanvasGroup MainUI;
    public TMP_Text LoadingText;
    public GameObject LoadingMap;

    private UnityAction NextMethod;

    private static ReliableUIManager instance;
    public static ReliableUIManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool IsLoading => isLoading;

    bool isLoading;

    private void Awake()
    {
        if (!instance) instance = this;
        SetBlack(0);
        SetLoadingMapActive(false);
        SetLoadingState(false);
    }

    /// <summary>
    /// Fade the game in black then call the next method (if Next = true)
    /// </summary>
    /// <param name="Active"></param>
    public void FadeBlack(bool Active, bool Next = false)
    {
        if (Active) SetLoadingMapActive(false);
        StartCoroutine(Coroutine_FadeBlack(Active, Next));
    }

    public void SetBlack(float Value)
    {
        MainUI.alpha = Mathf.Clamp01(Value);
        MainUI.gameObject.SetActive(Value != 0f);
    }

    public void ClearNextMethod()
    {
        NextMethod = null;
    }

    public void SetNextMethod(UnityAction Action)
    {
        NextMethod = Action;
    }

    IEnumerator Coroutine_FadeBlack(bool Active, bool Next)
    {
        SetBlack(Active ? 1 : 0);
        float RemainingTime = FadeBlackTime;
        while (RemainingTime > 0f)
        {
            if (!Active) SetBlack(1 - (RemainingTime / FadeBlackTime));
            else SetBlack(RemainingTime / FadeBlackTime);

            RemainingTime -= Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        SetBlack(Active ? 0 : 1);

        if (Next) NextMethod.Invoke();
    }

    public void SetLoadingMapActive(bool isActive)
    {
        LoadingMap.SetActive(isActive);
    }

    public void StartUIProcedure()
    {
        StartCoroutine(Coroutine_LoadingMap());
    }

    IEnumerator Coroutine_LoadingMap()
    {
        yield return new WaitForSeconds(FadeBlackTime);
        SetLoadingMapActive(true);
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            LoadingText.text = $"Chargement {Mathf.FloorToInt(PhotonNetwork.LevelLoadingProgress * 100)}%";
            yield return null;
        }
    }

    public void SetLoadingState(bool _isLoading)
    {
        isLoading = _isLoading;
    }
}
