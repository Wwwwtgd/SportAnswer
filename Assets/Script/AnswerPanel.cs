using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class NewBehaviourScript : MonoBehaviour
{
    private List<Answer> answerList = new(); //题目列表

    private int currLine;  //当前行号

    //public GameObject tipsbtn;//提示按钮
    //public Text tipsText;//提示信息
    //public List<OptionsItem> toggleList;//答题Toggle

    public Text answerIdText; //当前第题目序号

    public Text answerTitleText;//当前题目文本

    void Awake()
    {
        DataReader data = new();  //创建DataReader对象
        currLine = 0;
        // 使用 SelectRandomAnswers 方法从中选取符合条件的答案
        answerList = data.SelectRandomAnswers("Single", 7);  
        answerList.AddRange(data.SelectRandomAnswers("Single", 3));
    }

    void Start()
    {
        UpdateUI(currentAnswer, currLine);

    }

    void UpdataUI()
    {

    }
}
