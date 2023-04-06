using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Timeline;



public class Timeline : MonoBehaviour
{
    public GameObject QuestionPanel;
    public GameObject Answer_Left;
    public GameObject Answer_Right;
    public GameObject Answer_Left_Name;
    public GameObject Answer_Right_Name;
    public GameObject CountdownText;
    public GameObject QuestionText;
    public GameObject Circle;

    private float baseImageSize;

    public float timeRemaining = 5; // the amount of time to countdown from
    public delegate void CountEventHandler(int count); // define a delegate to handle count events
    public event CountEventHandler OnCount; // define an event to handle count events
    public event CountEventHandler OnCountZero; // define an event to handle the end of the countdown
    private bool isCounting = false;

    public GameObject databaseManager;
    private CSVProcessing csvProcessing;

    private int correctAnswer;
    private int playerAnswer;

    public GameObject timeline_gameOver;


    void Awake()
    {
        Input.gyro.enabled = true;
        csvProcessing = databaseManager.GetComponent<CSVProcessing>();
    }

    void Start()
    {

    }

    void Update()
    {
        if(isCounting)
        {
            DetectPlayersAnswer();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(databaseManager.GetComponent<CSVProcessing>().getQuestionDatas(0));
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

        Answer_Left_Name.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetDelay(2f);

        Answer_Right_Name.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f)
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
        List<GameObject> Answer_texts = new List<GameObject> { Answer_Left_Name, Answer_Right_Name };
        int answerSide = getPlayersAnswer();

        //Let players answer move to the center
        Answers[answerSide].transform.DOLocalMove(new Vector3(0 - Answers[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(1f);

        Answer_texts[answerSide].transform.DOLocalMove(new Vector3(0 - Answer_texts[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(1f);

        //Make other answer disappear
        Answers[1 - answerSide].transform.DOScale(new Vector3(0.01f, 0.01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(1f);

        Answer_texts[1 - answerSide].transform.DOScale(new Vector3(0.01f, 0.01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(1f);


        //Let a question sentence and an aswer go up
        QuestionPanel.transform.DOLocalMove(new Vector3(0, 800, 0), .2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(2f);




    }

    public void decideQuestionsFromDatabase()
    {
        //ランダムにクイズの問題を決定
        int quizNum = csvProcessing.pickQuestionNum();

        //QuizDatabseManager から問題文/選択肢を取り出す
        string question_contents = csvProcessing.getQuestionDatas(quizNum);
        string question_True_Option = csvProcessing.getChoicesData(quizNum, true);
        string question_False_Option = csvProcessing.getChoicesData(quizNum, false);

        //Resources/Images/ から問題を取り出す
        Sprite Answer_True_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "1");
        Sprite Answer_False_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "0");


        //問題文/選択肢を差し替える, 画像をあてる
        QuestionText.GetComponent<TextMeshProUGUI>().text = question_contents;
        int setOptionsRandom = Random.Range(0, 1);
        if(setOptionsRandom == 0)
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_True_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_False_Image;

            // Left に 0
            Answer_Left_Name.GetComponent<TextMeshProUGUI>().text = question_True_Option;
            // Right に 残った方
            Answer_Right_Name.GetComponent<TextMeshProUGUI>().text = question_False_Option;

            correctAnswer = 0;

        }
        else
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_False_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_True_Image;

            // Left に 1
            Answer_Left_Name.GetComponent<TextMeshProUGUI>().text = question_False_Option;
            Answer_Right_Name.GetComponent<TextMeshProUGUI>().text = question_True_Option;

            correctAnswer = 1;
        }

    }



    public int getPlayersAnswer()
    {
        if (Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        {
            playerAnswer = 0;
        }
        else if (Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        {
            playerAnswer = 1;
        }
        else
        {
            playerAnswer = Random.Range(0, 1);
        }

        return playerAnswer;
    }

    public void showQuizAnswer()
    {
        if(playerAnswer == correctAnswer)
        {
            //正解 -> 〇を表示
            Circle.GetComponent<Animator>().SetTrigger("ShowCircleTrigger");

        }
        else
        {

        }

    }

    public void changePath()
    {
        if (playerAnswer == correctAnswer)
        {
            //正解 -> タイムライン変わらず
        }
        else
        {
            //不正解 -> 問題番号に応じて別のタイムラインに移動
            goThroughWrongPath();
        }
    }

    private void goThroughWrongPath()
    {
        GetComponent<PlayableDirector>().Pause();
        timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        //GetComponent<TrolleyMoveController>().enabled = false;
        //timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        //timeline_gameOver.GetComponent<PlayableDirector>().Play();
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
        //float baseAngle = rotation.eulerAngles.z;

        //if (baseAngle > 180)
        //{
        //    baseAngle -= 360;
        //}

        //float tiltAngle = Mathf.Clamp(baseAngle, -30, 30);
        //float sizeScale = (tiltAngle / 60) + 1;



        //float leftImageSize = baseImageSize * sizeScale;
        //float rightImageSize = baseImageSize * (2 - sizeScale);

        //Answer_Left.transform.localScale = new Vector3(leftImageSize, leftImageSize, 1);
        //Answer_Right.transform.localScale = new Vector3(rightImageSize, rightImageSize, 1);


        //PC テスト用スクリプト
        float BiggerSize = baseImageSize * 1.2f;
        float SmallerSize = baseImageSize * 0.8f;

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
