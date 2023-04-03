using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class Timeline : MonoBehaviour
{
    public GameObject QuestionPanel;
    public GameObject Answer_Left;
    public GameObject Answer_Right;
    public GameObject CountdownText;

    private float baseImageSize;

    public float timeRemaining = 5; // the amount of time to countdown from
    public delegate void CountEventHandler(int count); // define a delegate to handle count events
    public event CountEventHandler OnCount; // define an event to handle count events
    public event CountEventHandler OnCountZero; // define an event to handle the end of the countdown
    private bool isCounting = false;

    void Awake()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        if(isCounting)
        {
            DetectPlayersAnswer();
        }
    }

    public void showQuestionPanel()
    {
        Debug.Log("1st Question!");

        QuestionPanel.transform.DOLocalMove(new Vector3(0, -700, 0), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetRelative();

        Answer_Left.transform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetDelay(2f);

        Answer_Right.transform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetDelay(2f);

    }

    public void startCountdown()
    {
        baseImageSize = Answer_Left.transform.localScale.x;

        if (!isCounting)
        {
            isCounting = true;
            StartCoroutine(CountdownCoroutine());
        }
    }

    public void confirmPlayersAnswer()
    {
        DOTween.To
            (
            () => CountdownText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => CountdownText.GetComponent<TextMeshProUGUI>().alpha = x,
            0,
            .5f
            ); ;

        List<GameObject> Answers = new List<GameObject> { Answer_Left, Answer_Right };

        //Let players answer move to the center
        Answers[getPlayersAnswer()].transform.DOLocalMove(new Vector3(0 - Answers[getPlayersAnswer()].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(1f);

        //Make other answer disappear
        Answers[1 - getPlayersAnswer()].transform.DOScale(new Vector3(0.01f, 0.01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(1f);


        //Let a question sentence and an aswer go up
        QuestionPanel.transform.DOLocalMove(new Vector3(0, 800, 0), .2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(2f);




    }

    public int getPlayersAnswer()
    {
        if (Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        {
            return 0;
        }
        else if (Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        {
            return 1;
        }
        else
        {
            return Random.Range(0, 1);
        }
    }

    public void verifyAnswer()
    {

    }



    private IEnumerator CountdownCoroutine()
    {
        CountdownText.SetActive(true);
        while (timeRemaining > 0)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60); 
            int seconds = Mathf.FloorToInt(timeRemaining % 60); 
            CountdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}",seconds); 

            if (OnCount != null) 
            {
                OnCount(Mathf.FloorToInt(timeRemaining));
            }

            yield return new WaitForSeconds(1); 

            timeRemaining--; 
        }

        CountdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}", 0); 

        if (OnCountZero != null) 
        {
            OnCountZero(0);
        }

        isCounting = false;
    }

    private void DetectPlayersAnswer()
    {
        //Quaternion rotation = Input.gyro.attitude;
        //float tiltAngle = Mathf.Clamp(rotation.eulerAngles.z, -30, 30);
        //float modified_tiltAngle = (tiltAngle / 60) + 1;

        //float leftImageSize = baseImageSize * modified_tiltAngle;
        //float rightImageSize = baseImageSize * ( 2 - modified_tiltAngle);

        //Answer_Left.transform.localScale = new Vector3(leftImageSize, leftImageSize, 1);
        //Answer_Right.transform.localScale = new Vector3(rightImageSize, rightImageSize, 1);

        float BiggerSize = baseImageSize * 1.5f;
        float SmallerSize = baseImageSize * 0.5f;

        //PC テスト用スクリプト
        if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x == Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

        }
        else if (Input.GetKeyDown(KeyCode.S) & Answer_Right.transform.localScale.x == Answer_Left.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);
        }
        else if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);
        }
        else if (Input.GetKeyDown(KeyCode.S) && Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);
        }
    }

    
}
