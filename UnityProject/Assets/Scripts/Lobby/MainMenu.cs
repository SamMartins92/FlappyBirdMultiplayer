using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerEnterHandler
{
    #region Fields
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource hoverSound;
    [SerializeField] AudioSource clickSound;
    [SerializeField] Animator goButtonAnimator;
    #endregion

    private void Start()
    {
        backgroundMusic.Play();
    }

    public void GoToMainScene()
    {
        OnClickEnter();
        StartCoroutine(WaitforClickSound());
    }

    IEnumerator WaitforClickSound()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverSound.Play();
    }

    public void OnClickEnter()
    {
        clickSound.Play();
    }

    public void setButtonBigger()
    {
        goButtonAnimator.SetBool("isHover", true);
    }
    public void resetButtonBigger()
    {
        goButtonAnimator.SetBool("isHover", false);
    }
}
