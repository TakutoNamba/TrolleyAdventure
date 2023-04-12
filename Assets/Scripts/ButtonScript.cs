using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;



public class ButtonScript : MonoBehaviour
{
    Image image;
    public PlayableDirector _timeline;
    public PlayableDirector _timeline_gameOver;
    public TextMeshProUGUI _text;
    private void Awake()
    {
        image = GetComponent<Image>();

    }

    public void OnClick()
    {
        //Debug.Log("‰Ÿ‚³‚ê‚½I");
        image.DOFade(0, 1f);
        DOTween.To
            (
            () => _text.alpha,
            (x) => _text.alpha = x,
            0,
            1f
            ); ;

        _timeline.Play();
        _timeline_gameOver.Play();
    }

    public void startGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void backToMenu()

    {
        SceneManager.LoadScene("StartScene");
    }
}