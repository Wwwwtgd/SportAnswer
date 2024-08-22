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
    public Button choiceBtn;  //选择选项按钮

    public Text selectTipsText; // 选项提示列表

    public Text selectChoiceText; // 选项内容列表

    public GameObject selectRight; // 正确面板

    public GameObject selectError; // 错误面板

    private bool isOn = false; //选中状态

    public void InitUI(string key)
    {
        this.selectTipsText.canvasRenderer.SetAlpha(0f); // 隐藏选项提示
        this.selectChoiceText.text = key;  // 更新选项内容
        this.selectRight.SetActive(false); // 隐藏正确提示
        this.selectError.SetActive(false); // 隐藏错误提示
    }
    public void onClickBtn()
    {

        choiceBtn.onClick.AddListener(() => {
            ChangeColor(isOn ? Color.cyan : Color.red);  //根据状态改变颜色
            isOn = !isOn;  //切换选中状态

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

    public DataReader data = new DataReader(); //DataReader 类对象

    private List<Answer> answerList = new List<Answer>(); //题目列表

    private int currentLine;  //当前行号

    private Answer currentAnswer;  //当前题目

    public Text answerIdText; // 题目序号UI

    public Text answerQuestionText;// 题目问题UI

    public Text answerTypeText;// 题目题型UI

    public Text tipsText;//提示信息

    public List<OptionsItem> optionsItems; //选项

    public Button prevBtn;  // 上一题按钮

    public Button nextBtn;  // 下一题按钮

    public Button sureBtn;  // 确认按钮

    public Button submitBtn;  // 确认按钮


    //public List<OptionsItem> toggleList;//答题Toggle

    void Awake()
    {

    }


    void Start()
    {
        Debug.Log("This is a Start.");
        // 使用 SelectRandomAnswers 方法从中选取符合条件的答案
        answerList = data.SelectRandomAnswers("Single", 7);
        
        answerList.AddRange(data.SelectRandomAnswers("Single", 3));

        currentAnswer = answerList[currentLine];  //找到当前题目
        
        UpdateUI();  //初始化界面UI

        
    }

    

    // 更新文本信息
    void UpdateUI()
    {
        answerIdText.text = currentAnswer.id;  // 更新序号
        Debug.Log("已更新序号");
        answerQuestionText.text = currentAnswer.question;  //更新题目
        answerTypeText.text = currentAnswer.type.Equals("Single") == true ? "单选" : "多选";  //更新题目类型
        tipsText.text = "请选择你的答案！"; // 更新提示信息

        for (int i = 0; i < 4; i++)
        {
            optionsItems[i].InitUI(currentAnswer.keys[i]); // 设置选项区
        }

        prevBtn.gameObject.SetActive(false);  //隐藏“上一题”按钮
        nextBtn.gameObject.SetActive(false);  //隐藏“下一题”按钮
        sureBtn.gameObject.SetActive(false);  //隐藏“确  认”按钮
        submitBtn.gameObject.SetActive(false);  //隐藏“提  交”按钮

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

    // 从给定的 answers 列表中随机选取对象
    public List<Answer> SelectRandomAnswers(string type, int i)
    {
        
        // 获取包含所有 Answer 对象的列表
        List<Answer> allAnswers = GetAllAnswers();
        
        // 创建一个随机数生成器
        System.Random random = new System.Random();
        // 从 answers 中筛选出题型为 type 的对象，并随机选取 i 个
        List<Answer> selectedAnswers = allAnswers
            .Where(answer => answer.type.Equals(type) == true) // 筛选出题型为 type 的 Answer 对象
            .OrderBy(x => random.Next())              // 随机排序
            .Take(i)                                  // 取前 i 个对象
            .ToList();
        return selectedAnswers; // 返回选取的列表
    }


    // 读取 Excel 文件并返回一个包含所有 Answer 对象的列表
    public List<Answer> GetAllAnswers()
    {
        // 创建一个存储所有答案的列表
        List<Answer> answers = new List<Answer>();

        // Excel 文件的路径
        string filePath = Application.dataPath + "/Data/TestData.xlsx";
        
        // 使用 FileStream 打开 Excel 文件
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            SceneManager.LoadScene("MainScene");
            // 创建一个 IWorkbook 对象来处理 Excel 文件
            IWorkbook workbook = new XSSFWorkbook(file);
            // 获取 Excel 文件中的第一个工作表
            ISheet sheet = workbook.GetSheetAt(0);

            // 从工作表的第二行开始读取数据，通常第一行是标题
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                // 获取当前行
                IRow row = sheet.GetRow(i);
                if (row.GetCell(0) == null || row.GetCell(1) == null || row.GetCell(2) == null || row.GetCell(3) == null || row == null) continue; // 如果行为空，跳过

                // 创建一个新的 Answer 对象并填充数据
                Answer answer = new Answer
                {
                    id = row.GetCell(0).ToString(), // 获取第一列的序号
                    question = row.GetCell(1).ToString(), // 获取第二列的题目
                    keys = new List<string>(),         // 初始化选项列表
                    type = row.GetCell(4).ToString(), // 获取第四列的题型
                    trueKeys = new List<string>()       // 初始化答案列表
                };

                // 读取选项和答案数据
                String rkeys = row.GetCell(2).ToString();
                String rtrueKeys = row.GetCell(3).ToString();
                answer.keys = new List<string>(rkeys.Split(','));  // 将选项字符串拆分成多个答案，并存储到答案列表中
                answer.trueKeys = new List<string>(rtrueKeys.Split(',')); // 将答案字符串拆分成多个答案，并存储到答案列表中

                // 将填充好的 Answer 对象添加到答案列表中
                answers.Add(answer);
            }
        }

        // 返回包含所有 Answer 对象的列表
        return answers;
    }
}

