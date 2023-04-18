using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class SplineController : MonoBehaviour
{
    [Header("State monitor")]
    public bool isPlaying;
    public float dist;
    public int questionNumber;

    [Header("Other parameters")]
    public float speed;
    public float timeRemaining = 5; // the amount of time to countdown from


    public Vector3 posModifier;
    public string pathIndicator;
    private Vector3 prevPos;

    public delegate void CountEventHandler(int count); // define a delegate to handle count events
    public event CountEventHandler OnCount; // define an event to handle count events
    public event CountEventHandler OnCountZero; // define an event to handle the end of the countdown
    private bool isCounting = false;
    private bool isAnswerCorrect = true;
    [SerializeField] float state = 0;
    private float baseImageSize;
    private float baseNameSize;
    private int correctAnswer;
    private int playerAnswer;

    private enum GAME_STATE
    {
        RUNNING,
        CORRECT,
        FALSE,
        RUN_TO_COR,
        RUN_TO_FAL
    }
    [SerializeField] GAME_STATE gameState;


    private CSVProcessing csvProcessing;

    public GameObject[] splines;
    public float[] triggers;
    public GameObject trolleyObject;
    private SplineContainer spline;
    public GameObject startCountdownText;
    public GameObject CountdownText;
    public GameObject QuestionText;
    public GameObject QuestionPanel;
    public GameObject Answer_Left;
    public GameObject Answer_Right;
    public GameObject Answer_Left_Name;
    public GameObject Answer_Right_Name;
    public GameObject CorrectAnswer;
    public GameObject FalseAnswer;
    public GameObject GameclearText;
    public GameObject GameoverText;
    public GameObject databaseManager;

    public GameObject RetryButton;
    public GameObject EndButton;
    public GameObject coverImage;







    void Awake()
    {
        csvProcessing = databaseManager.GetComponent<CSVProcessing>();
        //Debug.Log("Num: " + csvProcessing.);
        spline = splines[0].transform.GetChild(0).GetComponent<SplineContainer>();
        gameState = GAME_STATE.RUNNING;

    }

    void Start()
    {
        startGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            moveTrolley(spline.GetComponent<SplineContainer>());
        }


        if (gameState == GAME_STATE.RUNNING)
        {

            if (state == triggers[1])
            {
                DetectPlayersAnswer();
            }

            if (getPercentage(spline, dist) > triggers[0] && state < triggers[0])
            {
                state = triggers[0];
                decideQuestionsFromDatabase(questionNumber);
                showQuestionPanel();
            }
            else if (getPercentage(spline, dist) > triggers[1] && state < triggers[1])
            {
                state = triggers[1];
                startCountdown();
            }
            else if (getPercentage(spline, dist) > triggers[2] && state < triggers[2])
            {
                state = triggers[2];
                confirmPlayersAnswer();
                changePath();
            }
            else if (getPercentage(spline, dist) > triggers[3] && state < triggers[3])
            {

            }

        }

        if (gameState == GAME_STATE.RUN_TO_COR || gameState == GAME_STATE.RUN_TO_FAL)
        {
            if (getPercentage(spline, dist) > 0.99)
            {
                changePath();
            }
        }

        if (gameState == GAME_STATE.CORRECT)
        {
            if (getPercentage(spline, dist) > 0.3 && state < 0.3)
            {
                state = 0.3f;
                //正解表示する
                showCorrect();
            }
        }

        if (gameState == GAME_STATE.FALSE)
        {
            if (getPercentage(spline, dist) > 0.6 && state < 0.6)
            {
                state = 0.6f;
                //不正解表示する
                showWrong();

            }
            else if (getPercentage(spline, dist) > 0.8 && state < 0.8)
            {
                state = 0.8f;
                //不正解表示する
                showGameover();
                displayReturnOptions();
            }
        }

    }

    public void moveTrolley(SplineContainer splineContainer)
    {
        dist += Time.deltaTime * speed;
        trolleyObject.transform.position = splineContainer.EvaluatePosition(getPercentage(splineContainer, dist));
        trolleyObject.transform.position = new Vector3(trolleyObject.transform.position.x + posModifier.x, trolleyObject.transform.position.y + posModifier.y, trolleyObject.transform.position.z + posModifier.z);
        trolleyObject.transform.rotation = Quaternion.Euler(0, getCameraAngle(trolleyObject.transform.position, prevPos), 0);

        prevPos = new Vector3(trolleyObject.transform.position.x, trolleyObject.transform.position.y, trolleyObject.transform.position.z);
    }

    public float getCameraAngle(Vector3 curPos, Vector3 prevPos)
    {
        return Mathf.Atan2(curPos.x - prevPos.x, curPos.z - prevPos.z);
    }

    public float getPercentage(SplineContainer splineContainer, float curPoint)
    {
        return curPoint / getTotalLength(splineContainer);
    }

    public float getTotalLength(SplineContainer splineContainer)
    {
        return splineContainer.CalculateLength();
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

        //トロッコ動かすスクリプト
        isPlaying = true;
    }

    private IEnumerator CountdownCoroutine(GameObject countdownText, float settingTime)
    {
        countdownText.SetActive(true);
        while (settingTime > 0)
        {
            int minutes = Mathf.FloorToInt(settingTime / 60);
            int seconds = Mathf.FloorToInt(settingTime % 60);
            countdownText.GetComponent<TextMeshProUGUI>().text = string.Format("{00}", seconds);

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

    public void showQuestionPanel()
    {
        Debug.Log("Question!");

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
        baseNameSize = Answer_Left_Name.transform.localScale.x;

        if (!isCounting)
        {
            isCounting = true;
            StartCoroutine(CountdownCoroutine(CountdownText, timeRemaining));
        }
    }

    public void decideQuestionsFromDatabase(int questionCount)
    {
        //ランダムにクイズの問題を決定
        int quizNum = csvProcessing.pickQuestionNum();

        //QuizDatabaseManager から問題文/選択肢を取り出す
        string question_contents = csvProcessing.getQuestionDatas(quizNum);
        string question_True_Option = csvProcessing.getChoicesData(quizNum, true);
        string question_False_Option = csvProcessing.getChoicesData(quizNum, false);

        //Resources/Images/ から問題を取り出す
        Sprite Answer_True_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "1");
        Sprite Answer_False_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "0");


        //問題文/選択肢を差し替える, 画像をあてる
        QuestionText.GetComponent<TextMeshProUGUI>().text = question_contents;

        int setOptions = int.Parse(pathIndicator.Substring((questionCount - 1) * 2, (questionCount - 1) * 2 + 1));
        if (setOptions == 1)
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_True_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_False_Image;

            // Left に 1
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
        Debug.Log("player's answer : " + playerAnswer);
        return playerAnswer;
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


        //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
        //    new Vector3(0, 0, 0),
        //    1f
        //    ).SetDelay(3f);




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

    public void updateQuestionNumber()
    {
        questionNumber++;
    }

    public void changePath()
    {
        if (gameState == GAME_STATE.RUNNING)
        {
            if (playerAnswer == correctAnswer)
            {
                gameState = GAME_STATE.RUN_TO_COR;
                spline = splines[questionNumber - 1].transform.GetChild(1).GetComponent<SplineContainer>();
            }
            else
            {
                gameState = GAME_STATE.RUN_TO_FAL;
                spline = splines[questionNumber - 1].transform.GetChild(2).GetComponent<SplineContainer>();
            }
        }
        else if (gameState == GAME_STATE.RUN_TO_COR)
        {
            gameState = GAME_STATE.CORRECT;
            spline = splines[questionNumber - 1].transform.GetChild(3).GetComponent<SplineContainer>();
        }
        else if (gameState == GAME_STATE.RUN_TO_FAL)
        {
            gameState = GAME_STATE.FALSE;
            spline = splines[questionNumber - 1].transform.GetChild(4).GetComponent<SplineContainer>();
        }
        else if (gameState == GAME_STATE.CORRECT)
        {
            updateQuestionNumber();
            gameState = GAME_STATE.RUNNING;
            spline = splines[questionNumber - 1].transform.GetChild(0).GetComponent<SplineContainer>();
        }

        dist = 0;
        state = 0;


    }

    public void showCorrect()
    {
        if (playerAnswer == correctAnswer)
        {
            //正解 -> 〇を表示
            Debug.Log("CORRECT");
            CorrectAnswer.GetComponent<Animator>().SetTrigger("ShowCircleTrigger");

        }
    }

    public void showWrong()
    {
        FalseAnswer.GetComponent<Animator>().SetTrigger("ShowFalseTrigger");
    }

    public void showGameover()
    {

        DOTween.To
            (
            () => GameoverText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => GameoverText.GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
            );

    }

    public void displayReturnOptions()
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

        RetryButton.GetComponent<Button>().interactable = true;
        EndButton.GetComponent<Button>().interactable = true;
    }



    public void BackToRetry()
    {
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene("Main");

        RetryButton.GetComponent<Button>().interactable = false;
        EndButton.GetComponent<Button>().interactable = false;
    }

    public void BackToMenu()
    {
        //GetComponent<TrolleyMoveController>().enabled = true;
        //timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        SceneManager.LoadScene("StartScene");

        
        RetryButton.GetComponent<Button>().interactable = false;
        EndButton.GetComponent<Button>().interactable = false;

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
        float BiggerSize = baseImageSize * 1.1f;
        float SmallerSize = baseImageSize * 0.8f; 
        float BiggerName = baseNameSize * 1.1f;
        float SmallerName = baseNameSize * 0.8f;

        if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x == Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Left_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
            //    new Vector3(0, 0, 6),
            //    0.2f
            //    );

        }
        else if (Input.GetKeyDown(KeyCode.S) & Answer_Right.transform.localScale.x == Answer_Left.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Left_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            Answer_Right_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.2f)
                .SetEase(Ease.OutQuad);

            //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
            //    new Vector3(0, 0, -6),
            //    0.2f
            //    );

        }
        else if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Left_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
            //    new Vector3(0, 0, 12),
            //    0.2f
            //    );

        }
        else if (Input.GetKeyDown(KeyCode.S) && Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        {
            Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Left_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            Answer_Right_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.4f)
                .SetEase(Ease.OutQuad);

            //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
            //    new Vector3(0, 0, -12),
            //    0.2f
            //    );

        }
    }

}
