using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoManager : MonoBehaviour
{
    #region Instance variables & Init Method
    
    public AudioSource audioSource;
    public CanvasGroup canvasGroup;
    public float fadeTime = 2f;   // 2.5sec

    private void Init()
    {
        if (audioSource == null) { print("Cannot Find Component : AudioSource"); }
        if (canvasGroup == null) { print("Cannot Find Component : CanvasGroup"); }
        //audioSource.Play();
        StartCoroutine("FadeIn");
        canvasGroup.alpha = 0.01f;
    }
    
    #endregion

    
    #region Unity Event Function
    
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (canvasGroup.alpha >= 1.0f)
        {
            StopCoroutine("FadeIn");
            StartCoroutine("FadeOut");
        }
        if (canvasGroup.alpha <= 0f)
        {
            StopCoroutine("FadeOut");
            SceneManager.LoadScene("LoginScene");
        }
    }

    #endregion

    
    #region Coroutines for fade effect
    
    IEnumerator FadeIn()
    {
        while (true)
        {
            canvasGroup.alpha += 0.01f;
            yield return new WaitForSeconds(fadeTime * 0.01f);
        }
    }

    IEnumerator FadeOut()
    {
        while (true)
        {
            canvasGroup.alpha -= 0.01f;
            yield return new WaitForSeconds(fadeTime * 0.01f);
        }
    }
    #endregion
}
