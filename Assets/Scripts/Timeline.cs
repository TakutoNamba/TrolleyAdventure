using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;


public class Timeline : MonoBehaviour
{
    public GameObject trolleyObject;
    public GameObject startCountdownText;
    public GameObject QuestionPanel;
    public GameObject Answer_Left;
    public GameObject Answer_Right;
    public GameObject Answer_Left_Name;
    public GameObject Answer_Right_Name;
    public GameObject CountdownText;
    public GameObject QuestionText;
    public GameObject CorrectAnswer;
    public GameObject FalseAnswer;
    public GameObject GameoverText;
    public GameObject GameclearText;

    public GameObject RetryButton;
    public GameObject EndButton;
    public GameObject GoalObject;


    private float baseImageSize;

    public float timeRemaining = 5; // the amount of time to countdown from
    private float startTimeRemaining;

    public delegate void CountEventHandler(int count); // define a delegate to handle count events
    public event CountEventHandler OnCount; // define an event to handle count events
    public event CountEventHandler OnCountZero; // define an event to handle the end of the countdown
    private bool isCounting = false;
    private bool isAnswerCorrect = true;

    public GameObject databaseManager;
    private CSVProcessing csvProcessing;

    private int correctAnswer;
    private int playerAnswer;

    public GameObject timeline_gameOver;
    public GameObject coverImage;
    private float coverImageAlpha;

    private int questionCount = 1;






    void Awake()
    {
        Input.gyro.enabled = true;
        csvProcessing = databaseManager.GetComponent<CSVProcessing>();

        startTimeRemaining = timeRemaining;
    }

    void Start()
    {
        startGame();
    }

    void Update()
    {
        if(isCounting)
        {
            DetectPlayersAnswer();
        }

    }

    public void startGame()
    {
        StartCoroutine(startGameCorutine());
    }

    public IEnumerator startGameCorutine()
    {
        trolleyObject.transform.DOMove(new Vector3(0, -1, 0), 0.5f)
            .SetEase(Ease.InQuad)
            .SetRelative();


        yield return new WaitForSeconds(1.5f);

        StartCoroutine(CountdownCoroutine(startCountdownText, 3f));

        yield return new WaitForSeconds(3f);

        GetComponent<TrolleyMoveController>().enabled = true;
        //
        //timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        //trolleyObject.transform.parent.gameObject.GetComponent<PositionModifier>().enabled = true;
        GetComponent<PlayableDirector>().Play();
        timeline_gameOver.GetComponent<PlayableDirector>().Play();
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
            StartCoroutine(CountdownCoroutine(CountdownText, timeRemaining));
        }
    }

    public void confirmPlayersAnswer()
    {


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
        QuestionPanel.transform.DOLocalMove(new Vector3(0, 700, 0), .2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(2f);


        GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
            new Vector3(0, 0, 0),
            1f
            ).SetDelay(3f);




        //Back to the normal position

        Answers[answerSide].transform.DOLocalMove(new Vector3(Answers[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(3f);

        Answer_texts[answerSide].transform.DOLocalMove(new Vector3(Answer_texts[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(3f);

        Answers[answerSide].transform.DOScale(new Vector3(.01f, .01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(3f);

        Answer_texts[answerSide].transform.DOScale(new Vector3(.01f, .01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(3f);

        Answers[1 - answerSide].transform.DOScale(new Vector3(.01f, .01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(3f);

        Answer_texts[1 - answerSide].transform.DOScale(new Vector3(.01f, .01f, 1), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetDelay(3f);

        DOTween.To
            (
            () => CountdownText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => CountdownText.GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
            ).SetDelay(3f);






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

            correctAnswer = 1;

        }
        else
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_False_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_True_Image;

            // Left に 1
            Answer_Left_Name.GetComponent<TextMeshProUGUI>().text = question_False_Option;
            Answer_Right_Name.GetComponent<TextMeshProUGUI>().text = question_True_Option;

            correctAnswer = 0;
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
        Debug.Log(playerAnswer);
        return playerAnswer;
    }

    public void showQuizAnswer()
    {
        if(playerAnswer == correctAnswer)
        {
            //正解 -> 〇を表示
            CorrectAnswer.GetComponent<Animator>().SetTrigger("ShowCircleTrigger");

        }


    }

    public void showWrongAnswer()
    {
        if (!isAnswerCorrect)
        {
            FalseAnswer.GetComponent<Animator>().SetTrigger("ShowFalseTrigger");
        }

    }

    public void showGameover()
    {
        if(!isAnswerCorrect)
        {
            timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = false;

            DOTween.To
                (
                () => GameoverText.GetComponent<TextMeshProUGUI>().alpha,
                (x) => GameoverText.GetComponent<TextMeshProUGUI>().alpha = x,
                1,
                .5f
                );
        }

    }

    public void resetValue()
    {
        if(playerAnswer == correctAnswer)
        {
            CorrectAnswer.GetComponent<Animator>().Play("Idle");
        }
        else
        {
            FalseAnswer.GetComponent<Animator>().ResetTrigger("ShowFalseTrigger");
        }
        
        CountdownText.SetActive(false);

    }

    public void updateQuestionCount()
    {
        questionCount += 1;
        Debug.Log(questionCount);
        timeline_gameOver.GetComponent<TrolleyMoveController>().pathCreator = timeline_gameOver.GetComponent<TrolleyMoveController>().otherPathCreators[questionCount];
    }

    public void displayContinueOptions()
    {

        coverImage.GetComponent<Image>().DOFade(0.8f, 0.5f);

        DOTween.To
        (
            () => RetryButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha,
            (x) => RetryButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
        );
        DOTween.To
        (
            () => EndButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha,
            (x) => EndButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
        );
        RetryButton.GetComponent<Image>().DOFade(1, .5f);
        EndButton.GetComponent<Image>().DOFade(1, .5f);
    }

    public void displayReturnOptions()
    {
        if(!isAnswerCorrect)
        {
            coverImage.GetComponent<Image>().DOFade(0.8f, 0.5f);

            DOTween.To
            (
                () => RetryButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha,
                (x) => RetryButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha = x,
                1,
                .5f
            ).SetDelay(2.0f);

            DOTween.To
            (
                () => EndButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha,
                (x) => EndButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().alpha = x,
                1,
                .5f
            ).SetDelay(2.0f);

            RetryButton.GetComponent<Image>().DOFade(1, .5f).SetDelay(2.0f);
            EndButton.GetComponent<Image>().DOFade(1, .5f).SetDelay(2.0f);
        }


    }

    public void BackToRetry()
    {
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene("Main");
    }

    public void BackToMenu()
    {
        //GetComponent<TrolleyMoveController>().enabled = true;
        //timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        SceneManager.LoadScene("StartScene");
    }

    public void changePath()
    {
        if (playerAnswer == correctAnswer)
        {
            //正解 -> タイムライン変わらず
            isAnswerCorrect = true;
        }
        else
        {
            //不正解 -> 問題番号に応じて別のタイムラインに移動
            isAnswerCorrect = false;
            goThroughWrongPath();
        }
    }

    private void goThroughWrongPath()
    {
        GetComponent<PlayableDirector>().Pause();
        GetComponent<TrolleyMoveController>().enabled = false;
        timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
    }

    public void gameClear()
    {
        DOTween.To
            (
            () => GameclearText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => GameclearText.GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .2f
            ); ;

        GameclearText.GetComponent<TextMeshProSimpleAnimator>().enabled = true;
        //GameclearText.GetComponent<TextMeshProGeometryAnimator>().enabled = true;

        
    }

    public void hideTreasures()
    {
        GoalObject.transform.DOMove(new Vector3(0, -2, 0), 1f)
            .SetRelative()
            .SetEase(Ease.InQuad);
    }



    private IEnumerator CountdownCoroutine(GameObject countdownText, float settingTime)
    {
        countdownText.SetActive(true);
        while (settingTime > 0)
        {
            int minutes = Mathf.FloorToInt(settingTime / 60); 
            int seconds = Mathf.FloorToInt(settingTime % 60); 
            countdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}",seconds); 

            if (OnCount != null) 
            {
                OnCount(Mathf.FloorToInt(settingTime));
            }

            yield return new WaitForSeconds(1); 

            settingTime--; 
        }

        countdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}", 0); 

        if (OnCountZero != null) 
        {
            OnCountZero(0);
        }

        isCounting = false;
        //timeRemaining = startTimeRemaining;

        DOTween.To
            (
            () => countdownText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => countdownText.GetComponent<TextMeshProUGUI>().alpha = x,
            0,
            .5f
            ); ;
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

        //float trolleyAngle = tiltAngle / 5;



        //float leftImageSize = baseImageSize * sizeScale;
        //float rightImageSize = baseImageSize * (2 - sizeScale);

        //Answer_Left.transform.localScale = new Vector3(leftImageSize, leftImageSize, 1);
        //Answer_Right.transform.localScale = new Vector3(rightImageSize, rightImageSize, 1);

        //GetComponent<TrolleyMoveController>().trolleyObject.transform.rotation = new Vector3(0, 0, trolleyAngle);


        //PC テスト用スクリプト
        float BiggerSize = baseImageSize * 1.2f;
        float SmallerSize = baseImageSize * 0.8f;

        if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x == Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
                new Vector3(0, 0, 6),
                0.2f
                );

        }
        else if (Input.GetKeyDown(KeyCode.S) & Answer_Right.transform.localScale.x == Answer_Left.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
                new Vector3(0, 0, -6),
                0.2f
                );

        }
        else if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
                new Vector3(0, 0, 12),
                0.2f
                );

        }
        else if (Input.GetKeyDown(KeyCode.S) && Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
                new Vector3(0, 0, -12),
                0.2f
                );

        }
    }


}
