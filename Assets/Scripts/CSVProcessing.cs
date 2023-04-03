using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVProcessing : MonoBehaviour
{

    public QuizQuestionsData[] questionsData;
    public QuizChoicesData[] choicesData;

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

    }

    void Update()
    {
        
    }
}
