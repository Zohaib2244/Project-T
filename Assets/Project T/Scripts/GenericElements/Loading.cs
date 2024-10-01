using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Loading: MonoBehaviour
{
    //make singelton
    private static Loading _instance;
    public static Loading Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Loading instance is not initialized.");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject loadingScreen;

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
        loadingScreen.GetComponent<CanvasGroup>().DOFade(1, 0.5f).From(0);
    
    }
    public void HideLoadingScreen()
    {
        loadingScreen.GetComponent<CanvasGroup>().DOFade(0, 0.5f).From(1).OnComplete(() => loadingScreen.SetActive(false));
    }

}
