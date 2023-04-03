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
            .SetRelative()
            .SetDelay(2f);

        Answer_Right.transform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(2f);

        baseImageSize = Answer_Left.transform.localScale.x;

        

    }

    public void startCountdown()
    {
        if (!isCounting)
        {
            isCounting = true;
            StartCoroutine(CountdownCoroutine());
        }
    }



    private IEnumerator CountdownCoroutine()
    {
        CountdownText.SetActive(true);
        while (timeRemaining > 0)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60); // calculate the number of minutes
            int seconds = Mathf.FloorToInt(timeRemaining % 60); // calculate the number of seconds
            CountdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}",seconds); // format the time as mm:ss

            if (OnCount != null) // invoke the count event with the current count
            {
                OnCount(Mathf.FloorToInt(timeRemaining));
            }

            yield return new WaitForSeconds(1); // wait for one second

            timeRemaining--; // decrease the time remaining by one second
        }

        CountdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}", 0); // format the time as mm:ss

        if (OnCountZero != null) // invoke the count zero event
        {
            OnCountZero(0);
        }

        isCounting = false;
    }

    private void DetectPlayersAnswer()
    {
        Quaternion rotation = Input.gyro.attitude;
        float tiltAngle = Mathf.Clamp(rotation.eulerAngles.z, -30, 30);
        float modified_tiltAngle = (tiltAngle / 60) + 1;

        float leftImageSize = baseImageSize * modified_tiltAngle;
        float rightImageSize = baseImageSize * ( 2 - modified_tiltAngle);
        
        Answer_Left.transform.localScale = new Vector3(leftImageSize, leftImageSize, 1);
        Answer_Right.transform.localScale = new Vector3(rightImageSize, rightImageSize, 1);



        //PC テスト用スクリプト
        //if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x == Answer_Right.transform.localScale.x)
        //{
        //    Answer_Left.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.2f)
        //        .SetEase(Ease.OutQuad)
        //        .SetRelative();

        //    Answer_Right.transform.DOScale(new Vector3(0.5f, 0.5f, 1), 0.2f)
        //        .SetEase(Ease.OutQuad)
        //        .SetRelative();
        //}
        //if(Input.GetKeyDown(KeyCode.S) & Answer_Right.transform.localScale.x < Answer_Left.transform.localScale.x)
        //{

        //}
    }
}
