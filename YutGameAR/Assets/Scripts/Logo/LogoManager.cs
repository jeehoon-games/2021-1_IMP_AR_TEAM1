using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    #region Instance variables & Init Method
    
    public AudioSource audioSource;
    public CanvasGroup canvasGroup;
    public float fadeTime = 1.0f;   // 2.5sec

    private void Init()
    {
        if (audioSource == null) { print("Cannot find component : AudioSource"); }
        if (canvasGroup == null) { print("Cannot find component : CanvasGroup"); }
        canvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
        audioSource.Play();
    }
    
    #endregion

    
    
    #region Unity Event Functions
    
    void Start()
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
            yield return new WaitForSeconds(fadeTime * 0.01f);
        }
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.01f;
            yield return new WaitForSeconds(fadeTime * 0.01f);
        }
        SceneManager.LoadScene("LoginScene");
    }
    #endregion
}
