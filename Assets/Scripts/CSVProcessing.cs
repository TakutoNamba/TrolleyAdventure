using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVProcessing : MonoBehaviour
{

    public QuizQuestionsData[] questionsData;
    public QuizChoicesData[] choicesData;

    void Awake()
    {
        //　テキストファイルの読み込みを行ってくれるクラス
        TextAsset textasset = new TextAsset();
        //　先ほど用意したcsvファイルを読み込ませる。
        //　ファイルは「Resources」フォルダを作り、そこに入れておくこと。また"CSVTestData"の部分はファイル名に合わせて変更する。
        textasset = Resources.Load("QuizQuestionsData", typeof(TextAsset)) as TextAsset;
        //　CSVSerializerを用いてcsvファイルを配列に流し込む。
        questionsData = CSVSerializer.Deserialize<QuizQuestionsData>(textasset.text);

        textasset = Resources.Load("QuizChoicesData", typeof(TextAsset)) as TextAsset;
        choicesData = CSVSerializer.Deserialize<QuizChoicesData>(textasset.text);

    }

    void Update()
    {
        
    }
}
