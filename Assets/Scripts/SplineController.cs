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
    public int questionNumber;
    public float dist;
    [SerializeField] float state = 0;
    private enum GAME_STATE
    {
        RUNNING,
        CORRECT,
        FALSE,
        RUN_TO_COR,
        RUN_TO_FAL
    }
    [SerializeField] GAME_STATE gameState;

    [Header("Other parameters")]
    public float speed;
    public float timeRemaining = 5; // the amount of time to countdown from
    public float pushScale;


    public Vector3 posModifier;
    public string pathIndicator;
    private Vector3 prevPos;
    private Vector3 fallPos;

    public delegate void CountEventHandler(int count); // define a delegate to handle count events
    public event CountEventHandler OnCount; // define an event to handle count events
    public event CountEventHandler OnCountZero; // define an event to handle the end of the countdown
    private bool isCounting = false;
    private bool isAnswerCorrect = true;
    private bool isCameraFollowing = true;
    private float baseImageSize;
    private float baseNameSize;
    private int correctAnswer;
    private int playerAnswer;
    private float prevAngle;




    private CSVProcessing csvProcessing;

    public GameObject[] splines;
    public float[] triggers;
    public GameObject player;
    public GameObject trolleyObject;
    public GameObject tiltedTrolleyObject;
    public GameObject mainCamera;
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
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 1000, sequencesCapacity: 400);
        DOTween.Clear(true);
        startGame();

    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            moveTrolley(spline.GetComponent<SplineContainer>());
        }
        else
        {
            //if(gameState == GAME_STATE.FALSE)
            //{
            //    trolleyObject.transform.rotation = Quaternion.Euler(fallPos.x, fallPos.y, fallPos.z);
            //}
        }

       




        if (gameState == GAME_STATE.RUNNING)
        {

            if (state == triggers[1])
            {
                // if(!Input.gyro.enabled)
                // {
                //     Input.gyro.enabled = true;
                // }
                if(isCounting)
                {
                    DetectPlayersAnswer();                   
                }
            }
            else
            {
                // Input.gyro.enabled = false;
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
            else if (getPercentage(spline, dist) >= triggers[2] && state < triggers[2])
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
            if (getPercentage(spline, dist) >= 1)
            {
                changePath();
            }
        }

        if (gameState == GAME_STATE.CORRECT)
        {


            if (pathIndicator.Length == (questionNumber * 2))
            {


            if (getPercentage(spline, dist) > 0.4 && state < 0.4)
            {
                state = 0.4f;
                returnTrollyPos();
            }
                if (getPercentage(spline, dist) > 0.7 && state < 0.7)
                {
                    StartCoroutine(runGameclear());
                }

                if(speed < 3)
                {
                    isCameraFollowing = false;
                }

            }
            else
            {
                if (getPercentage(spline, dist) > 0.4 && state < 0.4)
                {
                    state = 0.4f;
                    returnTrollyPos();
                }

                if (getPercentage(spline, dist) > 0.6 && state < 0.6)
                {
                    state = 0.6f;
                    //����\������
                    showCorrect();
                }

                if (getPercentage(spline, dist) >= 1 && state < 1)
                {
                    state = 1;
                    resetValue();

                    changePath();
                }
            }


        }

        if (gameState == GAME_STATE.FALSE)
        {
            if (getPercentage(spline, dist) > 0.4 && state < 0.4)
            {
                state = 0.4f;
                returnTrollyPos();
            }

            if (getPercentage(spline, dist) > 0.6 && state < 0.6)
            {
                state = 0.6f;
                ////�s����\������

            }
            else if (getPercentage(spline, dist) > 0.8 && state < 0.8)
            {
                state = 0.8f;
                ////�s����\������
                //showGameover();
                //displayReturnOptions();
                //showWrong();

            }
            else if (getPercentage(spline, dist) >= 1 && state < 1)
            {
                state = 1.0f;
                //�s����\������
                isPlaying = false;

                StartCoroutine(runGameover());

            }
        }

        prevPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);



    }

    public void moveTrolley(SplineContainer splineContainer)
    {
        
        dist += Time.deltaTime * speed;
        if (dist >= getTotalLength(spline))
        {
            dist = getTotalLength(spline);
        }
        player.transform.position = splineContainer.EvaluatePosition(getPercentage(splineContainer, dist));
        player.transform.position = new Vector3(player.transform.position.x + posModifier.x, player.transform.position.y + posModifier.y, player.transform.position.z + posModifier.z);

        if(dist != (Time.deltaTime * speed) && isCameraFollowing)
        {
            player.transform.rotation = getCameraAngle(player.transform.position, prevPos);
        }

    }

    public void returnTrollyPos()
    {
        tiltedTrolleyObject.transform.DOLocalRotate
            (new Vector3(player.transform.rotation.x, player.transform.rotation.y, 0),
            2f
        );
    }


    public Quaternion getCameraAngle(Vector3 curPos, Vector3 prevPos)
    {
        return Quaternion.LookRotation(curPos - prevPos, Vector3.up);
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
        player.transform.DOMove(new Vector3(0, -1, 0), 0.5f)
            .SetEase(Ease.InQuad)
            .SetRelative();


        yield return new WaitForSeconds(1.5f);

        StartCoroutine(CountdownCoroutine(startCountdownText, 3f));

        yield return new WaitForSeconds(3f);

        //�g���b�R�������X�N���v�g
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
        Debug.Log("Question! " + questionNumber);

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
        //�����_���ɃN�C�Y�̖�������
        int quizNum = csvProcessing.pickQuestionNum();

        //QuizDatabaseManager �����蕶/�I���������o��
        string question_contents = csvProcessing.getQuestionDatas(quizNum);
        string question_True_Option = csvProcessing.getChoicesData(quizNum, true);
        string question_False_Option = csvProcessing.getChoicesData(quizNum, false);

        //Resources/Images/ ����������o��
        Sprite Answer_True_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "1");
        Sprite Answer_False_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "0");


        //��蕶/�I�����������ւ���, �摜�����Ă�
        QuestionText.GetComponent<TextMeshProUGUI>().text = question_contents;

        int setOptions = int.Parse(pathIndicator.Substring((questionCount - 1) * 2, 1));
        Debug.Log("setOptions : " + setOptions + " questionCount : " + questionCount);
        if (setOptions == 1)
        {
            Answer_Left.transform.GetChild(0).GetComponent<Image>().sprite = Answer_True_Image;
            Answer_Right.transform.GetChild(0).GetComponent<Image>().sprite = Answer_False_Image;

            // Left �� 1
            Answer_Left_Name.GetComponent<TextMeshProUGUI>().text = question_True_Option;
            // Right �� �c������
            Answer_Right_Name.GetComponent<TextMeshProUGUI>().text = question_False_Option;

            correctAnswer = 0;

        }
        else
        {
            Answer_Left.transform.GetChild(0).GetComponent<Image>().sprite = Answer_False_Image;
            Answer_Right.transform.GetChild(0).GetComponent<Image>().sprite = Answer_True_Image;

            // Left �� 1
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
        Debug.Log("Answer ; " + answerSide + " Correct Answer : " + correctAnswer);

        //Let players answer move to the center
        Answers[answerSide].transform.DOLocalMove(new Vector3(0 - Answers[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative();

        Answer_texts[answerSide].transform.DOLocalMove(new Vector3(0 - Answer_texts[answerSide].transform.localPosition.x, 0, 0), 0.2f)
            .SetEase(Ease.OutQuad)
            .SetRelative();

        //Make other answer disappear
        Answers[1 - answerSide].transform.DOScale(new Vector3(0.01f, 0.01f, 1), 0.2f)
            .SetEase(Ease.OutQuad);

        Answer_texts[1 - answerSide].transform.DOScale(new Vector3(0.01f, 0.01f, 1), 0.2f)
            .SetEase(Ease.OutQuad);


        //Let a question sentence and an aswer go up
        QuestionPanel.transform.DOLocalMove(new Vector3(0, 700, 0), .2f)
            .SetEase(Ease.OutQuad)
            .SetRelative()
            .SetDelay(0.7f);


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
        dist = 0;
        state = 0;


        if (gameState == GAME_STATE.RUNNING)
        {
            if (playerAnswer == correctAnswer)
            {
                //���̓��H�ɉf��R���[�`�����Ă�
                //StartCoroutine(pathToPath(splines[questionNumber - 1].transform.GetChild(0).gameObject, splines[questionNumber - 1].transform.GetChild(1).gameObject));

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

            //player.transform.DOMove(
            //    new Vector3(splines[questionNumber - 1].transform.GetChild(3).GetComponent<SplineContainer>().EvaluatePosition(0).x, splines[questionNumber - 1].transform.GetChild(3).GetComponent<SplineContainer>().EvaluatePosition(0).y, splines[questionNumber - 1].transform.GetChild(3).GetComponent<SplineContainer>().EvaluatePosition(0).z),
            //    Time.deltaTime
            //    );

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






    }

    private IEnumerator pathToPath(GameObject fromPath, GameObject toPath)
    {
        SplineContainer fromSpline = fromPath.GetComponent<SplineContainer>();
        SplineContainer toSpline = toPath.GetComponent<SplineContainer>();

        Vector3 fromPos = new Vector3(fromSpline.EvaluatePosition(1).x, fromSpline.EvaluatePosition(1).y, fromSpline.EvaluatePosition(1).z);
        Vector3 toPos = new Vector3(toSpline.EvaluatePosition(0).x, toSpline.EvaluatePosition(0).y, toSpline.EvaluatePosition(0).z);

        float dist = (toPos - fromPos).magnitude;

        player.transform.DOMove(
            new Vector3(toSpline.EvaluatePosition(0).x, toSpline.EvaluatePosition(0).y + 1, toSpline.EvaluatePosition(0).z),
            0.15f
            );
        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator runGameclear()
    {
        //����������
        delayTrolleySpeed();

        yield return new WaitForSeconds(5f);

        //�Q�[���N���A�\������
        showGameclear();
        isPlaying = false;

        yield return new WaitForSeconds(2.0f);

        //�����ǂ������������₷��
        displayReturnOptions();



    }


    public void showCorrect()
    {
        if (playerAnswer == correctAnswer)
        {
            //���� -> �Z��\��
            Debug.Log("CORRECT");
            CorrectAnswer.GetComponent<Animator>().SetTrigger("ShowCircleTrigger");

        }
    }

    public void delayTrolleySpeed()
    {
        DOTween.To
            (
            () => speed,
            (x) => speed = x,
            0,
            5f
         );
    }


    public void showGameclear()
    {
        GameclearText.GetComponent<TextMeshProSimpleAnimator>().enabled = true;
        //GameoverText.GetComponent<TextMeshProGeometryAnimator>().enabled = true;

        DOTween.To
            (
            () => GameclearText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => GameclearText.GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
            );
            
    }

    public void resetValue()
    {
        if (playerAnswer == correctAnswer)
        {
            CorrectAnswer.GetComponent<Animator>().Play("Idle");
        }
        else
        {
            FalseAnswer.GetComponent<Animator>().ResetTrigger("ShowFalseTrigger");
        }

        CountdownText.SetActive(false);

        DOTween.KillAll();
        DOTween.Clear(true);

    }

    private IEnumerator runGameover()
    {
        pushIntoHole();
        
        yield return new WaitForSeconds(2.3f);

        showGameover();

        yield return new WaitForSeconds(2.0f);

        displayReturnOptions();

    }


    public void showWrong()
    {
        FalseAnswer.GetComponent<Animator>().SetTrigger("ShowFalseTrigger");
    }

    public void showGameover()
    {
        GameoverText.GetComponent<TextMeshProSimpleAnimator>().enabled = true;
        //GameoverText.GetComponent<TextMeshProGeometryAnimator>().enabled = true;

        DOTween.To
            (
            () => GameoverText.GetComponent<TextMeshProUGUI>().alpha,
            (x) => GameoverText.GetComponent<TextMeshProUGUI>().alpha = x,
            1,
            .5f
            );

    }

    public void pushIntoHole()
    {
        fallPos = new Vector3(trolleyObject.transform.rotation.x, trolleyObject.transform.rotation.y, trolleyObject.transform.rotation.z);
        Vector3 dir = ((trolleyObject.transform.position - prevPos) * 100).normalized;
        //Debug.Log(dir.x + " " + dir.y + " " + dir.z);
        //Vector3 dir = new Vector3(0, 0, 1);


        //mainCamera.transform.parent = null;
        mainCamera.transform.DOLocalMove(
            new Vector3(0,0,6),
            2f
            ).SetDelay(0.1f);
        //mainCamera.transform.DOLocalMove(
        //    new Vector3(0, 0, 3),
        //    1f
        //    );
        mainCamera.transform.DOLocalRotate(
            new Vector3(90, 0, 0),
            2f
            ).SetDelay(0.3f);

        Rigidbody rb = trolleyObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(dir * pushScale, ForceMode.Impulse);


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
        RetryButton.GetComponent<Button>().interactable = false;
        EndButton.GetComponent<Button>().interactable = false;
        Scene loadScene = SceneManager.GetActiveScene();

        // ���݂̃V�[�����ēǂݍ��݂���
        DOTween.KillAll();
        DOTween.Clear(true);
        SceneManager.LoadScene("Main");


    }

    public void BackToMenu()
    {
        RetryButton.GetComponent<Button>().interactable = false;
        EndButton.GetComponent<Button>().interactable = false;
        //GetComponent<TrolleyMoveController>().enabled = true;
        //timeline_gameOver.GetComponent<TrolleyMoveController>().enabled = true;
        DOTween.KillAll();
        DOTween.Clear(true);
        SceneManager.LoadScene("StartScene");

        


    }


    private void DetectPlayersAnswer()
    {

        

        // Quaternion rotation = Input.gyro.attitude;


        float baseAngle = detectAngle();

        // if (baseAngle > 180)
        // {
        //     baseAngle -= 360;
        // }

        // float tiltAngle = Mathf.Clamp(baseAngle, -30, 30);
        // float sizeScale = (tiltAngle / 90) + 1;
        // float trolleyAngle = tiltAngle / 2;

        float BiggerSize = baseImageSize * 1.1f;
        float SmallerSize = baseImageSize * 0.8f;
        float BiggerName = baseNameSize * 1.1f;
        float SmallerName = baseNameSize * 0.8f;

        if(baseAngle == -15)
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
        else if(baseAngle == 15)
        {
           Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
               .SetEase(Ease.OutQuad);

           Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
               .SetEase(Ease.OutQuad);

           Answer_Left_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.2f)
               .SetEase(Ease.OutQuad);

           Answer_Right_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.2f)
               .SetEase(Ease.OutQuad);

        }
        else
        {
            if(prevAngle == -15 || prevAngle == 15)
            {
                Answer_Left.transform.DOScale(new Vector3(baseImageSize, baseImageSize, 1), 0.2f)
                    .SetEase(Ease.OutQuad);

               Answer_Right.transform.DOScale(new Vector3(baseImageSize, baseImageSize, 1), 0.2f)
                   .SetEase(Ease.OutQuad);

           Answer_Left_Name.transform.DOScale(new Vector3(baseNameSize, baseNameSize, 1), 0.2f)
                   .SetEase(Ease.OutQuad);

           Answer_Right_Name.transform.DOScale(new Vector3(baseNameSize, baseNameSize, 1), 0.2f)
                   .SetEase(Ease.OutQuad);
            }

        }

        prevAngle = baseAngle;

        


        // float leftImageSize = baseImageSize * (2 - sizeScale);
        // float rightImageSize = baseImageSize * sizeScale;

        // float leftNameSize = baseNameSize * (2 - sizeScale);
        // float rightNameSize = baseNameSize * sizeScale;

        // Answer_Left.transform.localScale = new Vector3(leftImageSize, leftImageSize, 1);
        // Answer_Right.transform.localScale = new Vector3(rightImageSize, rightImageSize, 1);
        // Answer_Left_Name.transform.localScale = new Vector3(leftNameSize, leftNameSize, 1);
        // Answer_Right_Name.transform.localScale = new Vector3(rightNameSize, rightNameSize, 1);

        //tiltedTrolleyObject.transform.DOLocalRotate(
        //    new Vector3(0, 0, -12),
        //    0.2f
        //    );

        // tiltedTrolleyObject.transform.localRotation = Quaternion.Euler(0, 0, -trolleyAngle);




        //PC �e�X�g�p�X�N���v�g
        //float BiggerSize = baseImageSize * 1.1f;
        //float SmallerSize = baseImageSize * 0.8f;
        //float BiggerName = baseNameSize * 1.1f;
        //float SmallerName = baseNameSize * 0.8f;

        //if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x == Answer_Right.transform.localScale.x)
        //{
        //    Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Left_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
        //    //    new Vector3(0, 0, 6),
        //    //    0.2f
        //    //    );

        //}
        //else if (Input.GetKeyDown(KeyCode.S) & Answer_Right.transform.localScale.x == Answer_Left.transform.localScale.x)
        //{
        //    Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Left_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.2f)
        //        .SetEase(Ease.OutQuad);

        //    //GetComponent<TrolleyMoveController>().trolleyObject.transform.DOLocalRotate(
        //    //    new Vector3(0, 0, -6),
        //    //    0.2f
        //    //    );

        //}
        //else if (Input.GetKeyDown(KeyCode.A) && Answer_Left.transform.localScale.x < Answer_Right.transform.localScale.x)
        //{
        //    Answer_Left.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Left_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    tiltedTrolleyObject.transform.DOLocalRotate(
        //        new Vector3(0, 0, 12),
        //        0.2f
        //        );



        //}
        //else if (Input.GetKeyDown(KeyCode.S) && Answer_Left.transform.localScale.x > Answer_Right.transform.localScale.x)
        //{
        //    Answer_Left.transform.DOScale(new Vector3(SmallerSize, SmallerSize, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right.transform.DOScale(new Vector3(BiggerSize, BiggerSize, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Left_Name.transform.DOScale(new Vector3(SmallerName, SmallerName, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    Answer_Right_Name.transform.DOScale(new Vector3(BiggerName, BiggerName, 1), 0.4f)
        //        .SetEase(Ease.OutQuad);

        //    tiltedTrolleyObject.transform.DOLocalRotate(
        //        new Vector3(0, 0, -12),
        //        0.2f
        //        );

        //}
    }

    public float detectAngle()
    {
        var dir = Vector3.zero;
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
		dir.z = Input.acceleration.z;

        float angle;

        if(dir.x > 0.1)
        {
            angle = 15;
        }
        else if(dir.x < -0.1)
        {
            angle = -15;
        }
        else
        {
            angle = 0;
        }
        // Debug.Log(dir.x + " " + dir.y + " " + dir.z);
        return angle;
    }

}
