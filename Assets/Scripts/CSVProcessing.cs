using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVProcessing : MonoBehaviour
{
    public int questionNum;
    public QuizQuestionsData[] questionsData;
    public QuizChoicesData[] choicesData;

    private List<int> questionOptions = new List<int>();


    void Awake()
    {
        //�@�e�L�X�g�t�@�C���̓ǂݍ��݂��s���Ă����N���X
        TextAsset textasset = new TextAsset();
        //�@��قǗp�ӂ���csv�t�@�C����ǂݍ��܂���B
        //�@�t�@�C���́uResources�v�t�H���_�����A�����ɓ���Ă������ƁB�܂�"CSVTestData"�̕����̓t�@�C�����ɍ��킹�ĕύX����B
        textasset = Resources.Load("QuizQuestionsData", typeof(TextAsset)) as TextAsset;
        //�@CSVSerializer��p����csv�t�@�C����z��ɗ������ށB
        questionsData = CSVSerializer.Deserialize<QuizQuestionsData>(textasset.text);

        textasset = Resources.Load("QuizChoicesData", typeof(TextAsset)) as TextAsset;
        choicesData = CSVSerializer.Deserialize<QuizChoicesData>(textasset.text);

        resetSetting();

        

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(pickQuestionNum());
        }
    }

    public string getQuestionDatas(int num)
    {
        return questionsData[num].question_contents;
    }



    public string getChoicesData(int num, bool isAnswer)
    {
        for(int i=0; i < choicesData.Length; i++)
        {
            if(choicesData[i].question_id == num)
            {
                if(choicesData[i].is_answer == isAnswer)
                {
                    return choicesData[i].question_options;
                }
            }
        }

        return null;
    }

    public int pickQuestionNum()
    {
        int ind = Random.Range(0, questionOptions.Count);
        int ransu = questionOptions[ind];
        questionOptions.RemoveAt(ind);
        //Debug.Log(questionOptions.Count + " " + ind + " " + ransu);
        return ransu;
    }

    private void resetSetting()
    {
        int totalQuestions = getHowmanyQuestions();
        for (int i = 0; i < totalQuestions; i++)
        {
            questionOptions.Add(i);
        }




    }

    public int getHowmanyQuestions()
    {
        return questionsData.Length;
    }


}
