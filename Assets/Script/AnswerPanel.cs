using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class NewBehaviourScript : MonoBehaviour
{
    private List<Answer> answerList = new(); //��Ŀ�б�

    private int currLine;  //��ǰ�к�

    //public GameObject tipsbtn;//��ʾ��ť
    //public Text tipsText;//��ʾ��Ϣ
    //public List<OptionsItem> toggleList;//����Toggle

    public Text answerIdText; //��ǰ����Ŀ���

    public Text answerTitleText;//��ǰ��Ŀ�ı�

    void Awake()
    {
        DataReader data = new();  //����DataReader����
        currLine = 0;
        // ʹ�� SelectRandomAnswers ��������ѡȡ���������Ĵ�
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
