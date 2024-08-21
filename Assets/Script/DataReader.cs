using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public class Answer
{
    public string id { get; set; }
    public string answerTitle { get; set; }
    public List<string> answerKeys { get; set; }
    public string answerType { get; set; }
    public List<string> answerTrueKeys { get; set; }
}


public class DataReader : MonoBehaviour
{
    /*
    public void Awake()
    {
        // 使用 SelectRandomAnswers 方法从中选取符合条件的答案
        List<Answer> singleAnswer = SelectRandomAnswers("Single",7);
        List<Answer> multipleAnswer = SelectRandomAnswers("Single",3);
    }
    */
    

    // 从给定的 answers 列表中随机选取对象
    public List<Answer> SelectRandomAnswers(string type, int i)
    {
        // 获取包含所有 Answer 对象的列表
        List<Answer> allAnswers = GetAllAnswers();
        // 创建一个随机数生成器
        Random random = new Random();

        // 从 answers 中筛选出题型为 type 的对象，并随机选取 i 个
        List<Answer> singleAnswers = answers
            .Where(answer => answer.answerType == type) // 筛选出题型为 type 的 Answer 对象
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
        string filePath = Application.Data + "/TestData.xlsx";

        // 使用 FileStream 打开 Excel 文件
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            // 创建一个 IWorkbook 对象来处理 Excel 文件
            IWorkbook workbook = new XSSFWorkbook(file);
            // 获取 Excel 文件中的第一个工作表
            ISheet sheet = workbook.GetSheetAt(0);

            // 从工作表的第二行开始读取数据，通常第一行是标题
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                // 获取当前行
                IRow row = sheet.GetRow(i);
                if (row.GetCell(0) != Null && row.GetCell(1) != Null && row.GetCell(2) != Null && row.GetCell(3) != Null) continue; // 如果行为空，跳过

                // 创建一个新的 Answer 对象并填充数据
                Answer answer = new Answer
                {
                    id = row.GetCell(0).ToString(), // 获取第一列的序号
                    answerTitle = row.GetCell(1).ToString(), // 获取第二列的题目
                    answerKeys = new List<string>(),         // 初始化选项列表
                    answerType = row.GetCell(3).ToString(), // 获取第四列的题型
                    answerTrueKeys = new List<string>()       // 初始化答案列表
                };

                // 读取选项和答案数据
                String keys = row.GetCell(2).ToString();
                String trueKeys = row.GetCell(4).ToString();
                answer.answerKeys = new List<string>(keys.Split(','));  // 将选项字符串拆分成多个答案，并存储到答案列表中
                answer.answerTrueKeys = new List<string>(trueKeys.Split(',')); // 将答案字符串拆分成多个答案，并存储到答案列表中

                // 将填充好的 Answer 对象添加到答案列表中
                answers.Add(answer);
            }
        }

        // 返回包含所有 Answer 对象的列表
        return answers;
    }
}
