using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;



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

    public void decideQuestionsFromDatabase()
    {
        //�����_���ɃN�C�Y�̖�������
        int quizNum = csvProcessing.pickQuestionNum();

        //QuizDatabseManager �����蕶/�I���������o��
        string question_contents = csvProcessing.getQuestionDatas(quizNum);
        string question_True_Option = csvProcessing.getChoicesData(quizNum, true);
        string question_False_Option = csvProcessing.getChoicesData(quizNum, false);

        //Resources/Images/ ����������o��
        Sprite Answer_True_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "1");
        Sprite Answer_False_Image = Resources.Load<Sprite>("Images/" + quizNum.ToString() + "0");


        //��蕶/�I�����������ւ���, �摜�����Ă�
        QuestionText.GetComponent<TextMeshProUGUI>().text = question_contents;
        int setOptionsRandom = Random.Range(0, 1);
        if(setOptionsRandom == 0)
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_True_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_False_Image;

            // Left �� 0
            Answer_Left_Name.GetComponent<TextMeshProUGUI>().text = question_True_Option;
            // Right �� �c������
            Answer_Right_Name.GetComponent<TextMeshProUGUI>().text = question_False_Option;

            correctAnswer = 0;

        }
        else
        {
            Answer_Left.GetComponent<Image>().sprite = Answer_False_Image;
            Answer_Right.GetComponent<Image>().sprite = Answer_True_Image;

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

        return playerAnswer;
    }

    public void showQuizAnswer()
    {
        if(playerAnswer == correctAnswer)
        {
            //���� -> �^�C�����C���ς�炸 �Z��\��
            Circle.GetComponent<Animator>().SetTrigger("ShowCircleTrigger");
        }
        else
        {
            //�s���� -> ���ԍ��ɉ����ĕʂ̃^�C�����C���Ɉړ�, �~�p�̃A�j���[�V�����X�^�[�g

        }


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

        //PC �e�X�g�p�X�N���v�g
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
