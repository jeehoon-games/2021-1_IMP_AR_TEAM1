using System;
using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogIn
{
    public class LogoManager : MonoBehaviour
    {
        #region Instance variables & Init Method

        public AudioSource audioSource;
        public CanvasGroup canvasGroup;
        private float _fadeTime = 0.75f; // 0.75 sec

        private void Init()
        {
            if (audioSource == null) { print("Cannot find component : AudioSource"); }
            if (canvasGroup == null) { print("Cannot find component : CanvasGroup"); }

            canvasGroup.alpha = 0;
            audioSource.Play();
            StartCoroutine(FadeIn());
            Screen.orientation = ScreenOrientation.Portrait;
        }

        #endregion



        #region Unity Event Functions

        void Awake()
        {
            Init();
        }

        #endregion



        #region Coroutines for fade effect

        IEnumerator FadeIn()
        {
            while (canvasGroup.alpha < 1.0f)
            {
                canvasGroup.alpha += 0.01f;
                yield return new WaitForSeconds(_fadeTime * 0.01f);
            }

            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut() 
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= 0.01f;
                yield return new WaitForSeconds(_fadeTime * 0.01f);
            }

            SceneManager.LoadScene("LoginScene");
        }

        #endregion
    }
}