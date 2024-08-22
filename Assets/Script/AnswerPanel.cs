using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


[System.Serializable]
public class OptionsItem
{
    public Button choiceBtn;  //ѡ��ѡ�ť

    public Text selectTipsText; // ѡ����ʾ�б�

    public Text selectChoiceText; // ѡ�������б�

    public GameObject selectRight; // ��ȷ���

    public GameObject selectError; // �������

    private bool isOn = false; //ѡ��״̬

    public void InitUI(string key)
    {
        this.selectTipsText.canvasRenderer.SetAlpha(0f); // ����ѡ����ʾ
        this.selectChoiceText.text = key;  // ����ѡ������
        this.selectRight.SetActive(false); // ������ȷ��ʾ
        this.selectError.SetActive(false); // ���ش�����ʾ
    }
    public void onClickBtn()
    {

        choiceBtn.onClick.AddListener(() => {
            ChangeColor(isOn ? Color.cyan : Color.red);  //����״̬�ı���ɫ
            isOn = !isOn;  //�л�ѡ��״̬

        });
    }
    private void ChangeColor(Color color)
    {
        Image buttonImage = choiceBtn.GetComponent<Image>();
        buttonImage.color = color;
    }
}


public class AnswerPanel : MonoBehaviour
{

    public DataReader data = new DataReader(); //DataReader �����

    private List<Answer> answerList = new List<Answer>(); //��Ŀ�б�

    private int currentLine;  //��ǰ�к�

    private Answer currentAnswer;  //��ǰ��Ŀ

    public Text answerIdText; // ��Ŀ���UI

    public Text answerQuestionText;// ��Ŀ����UI

    public Text answerTypeText;// ��Ŀ����UI

    public Text tipsText;//��ʾ��Ϣ

    public List<OptionsItem> optionsItems; //ѡ��

    public Button prevBtn;  // ��һ�ⰴť

    public Button nextBtn;  // ��һ�ⰴť

    public Button sureBtn;  // ȷ�ϰ�ť

    public Button submitBtn;  // ȷ�ϰ�ť


    //public List<OptionsItem> toggleList;//����Toggle

    void Awake()
    {

    }


    void Start()
    {
        Debug.Log("This is a Start.");
        // ʹ�� SelectRandomAnswers ��������ѡȡ���������Ĵ�
        answerList = data.SelectRandomAnswers("Single", 7);
        
        answerList.AddRange(data.SelectRandomAnswers("Single", 3));

        currentAnswer = answerList[currentLine];  //�ҵ���ǰ��Ŀ
        
        UpdateUI();  //��ʼ������UI

        
    }

    

    // �����ı���Ϣ
    void UpdateUI()
    {
        answerIdText.text = currentAnswer.id;  // �������
        Debug.Log("�Ѹ������");
        answerQuestionText.text = currentAnswer.question;  //������Ŀ
        answerTypeText.text = currentAnswer.type.Equals("Single") == true ? "��ѡ" : "��ѡ";  //������Ŀ����
        tipsText.text = "��ѡ����Ĵ𰸣�"; // ������ʾ��Ϣ

        for (int i = 0; i < 4; i++)
        {
            optionsItems[i].InitUI(currentAnswer.keys[i]); // ����ѡ����
        }

        prevBtn.gameObject.SetActive(false);  //���ء���һ�⡱��ť
        nextBtn.gameObject.SetActive(false);  //���ء���һ�⡱��ť
        sureBtn.gameObject.SetActive(false);  //���ء�ȷ  �ϡ���ť
        submitBtn.gameObject.SetActive(false);  //���ء���  ������ť

    }

}


public class Answer
{
    public string id { get; set; }
    public string question { get; set; }
    public List<string> keys { get; set; }
    public string type { get; set; }
    public List<string> trueKeys { get; set; }
}


public class DataReader
{

    // �Ӹ����� answers �б������ѡȡ����
    public List<Answer> SelectRandomAnswers(string type, int i)
    {
        
        // ��ȡ�������� Answer ������б�
        List<Answer> allAnswers = GetAllAnswers();
        
        // ����һ�������������
        System.Random random = new System.Random();
        // �� answers ��ɸѡ������Ϊ type �Ķ��󣬲����ѡȡ i ��
        List<Answer> selectedAnswers = allAnswers
            .Where(answer => answer.type.Equals(type) == true) // ɸѡ������Ϊ type �� Answer ����
            .OrderBy(x => random.Next())              // �������
            .Take(i)                                  // ȡǰ i ������
            .ToList();
        return selectedAnswers; // ����ѡȡ���б�
    }


    // ��ȡ Excel �ļ�������һ���������� Answer ������б�
    public List<Answer> GetAllAnswers()
    {
        // ����һ���洢���д𰸵��б�
        List<Answer> answers = new List<Answer>();

        // Excel �ļ���·��
        string filePath = Application.dataPath + "/Data/TestData.xlsx";
        
        // ʹ�� FileStream �� Excel �ļ�
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            SceneManager.LoadScene("MainScene");
            // ����һ�� IWorkbook ���������� Excel �ļ�
            IWorkbook workbook = new XSSFWorkbook(file);
            // ��ȡ Excel �ļ��еĵ�һ��������
            ISheet sheet = workbook.GetSheetAt(0);

            // �ӹ�����ĵڶ��п�ʼ��ȡ���ݣ�ͨ����һ���Ǳ���
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                // ��ȡ��ǰ��
                IRow row = sheet.GetRow(i);
                if (row.GetCell(0) == null || row.GetCell(1) == null || row.GetCell(2) == null || row.GetCell(3) == null || row == null) continue; // �����Ϊ�գ�����

                // ����һ���µ� Answer �����������
                Answer answer = new Answer
                {
                    id = row.GetCell(0).ToString(), // ��ȡ��һ�е����
                    question = row.GetCell(1).ToString(), // ��ȡ�ڶ��е���Ŀ
                    keys = new List<string>(),         // ��ʼ��ѡ���б�
                    type = row.GetCell(4).ToString(), // ��ȡ�����е�����
                    trueKeys = new List<string>()       // ��ʼ�����б�
                };

                // ��ȡѡ��ʹ�����
                String rkeys = row.GetCell(2).ToString();
                String rtrueKeys = row.GetCell(3).ToString();
                answer.keys = new List<string>(rkeys.Split(','));  // ��ѡ���ַ�����ֳɶ���𰸣����洢�����б���
                answer.trueKeys = new List<string>(rtrueKeys.Split(',')); // �����ַ�����ֳɶ���𰸣����洢�����б���

                // �����õ� Answer ������ӵ����б���
                answers.Add(answer);
            }
        }

        // ���ذ������� Answer ������б�
        return answers;
    }
}

