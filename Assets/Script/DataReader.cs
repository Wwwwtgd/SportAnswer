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
        // ʹ�� SelectRandomAnswers ��������ѡȡ���������Ĵ�
        List<Answer> singleAnswer = SelectRandomAnswers("Single",7);
        List<Answer> multipleAnswer = SelectRandomAnswers("Single",3);
    }
    */
    

    // �Ӹ����� answers �б������ѡȡ����
    public List<Answer> SelectRandomAnswers(string type, int i)
    {
        // ��ȡ�������� Answer ������б�
        List<Answer> allAnswers = GetAllAnswers();
        // ����һ�������������
        Random random = new Random();

        // �� answers ��ɸѡ������Ϊ type �Ķ��󣬲����ѡȡ i ��
        List<Answer> singleAnswers = answers
            .Where(answer => answer.answerType == type) // ɸѡ������Ϊ type �� Answer ����
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
        string filePath = Application.Data + "/TestData.xlsx";

        // ʹ�� FileStream �� Excel �ļ�
        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            // ����һ�� IWorkbook ���������� Excel �ļ�
            IWorkbook workbook = new XSSFWorkbook(file);
            // ��ȡ Excel �ļ��еĵ�һ��������
            ISheet sheet = workbook.GetSheetAt(0);

            // �ӹ�����ĵڶ��п�ʼ��ȡ���ݣ�ͨ����һ���Ǳ���
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                // ��ȡ��ǰ��
                IRow row = sheet.GetRow(i);
                if (row.GetCell(0) != Null && row.GetCell(1) != Null && row.GetCell(2) != Null && row.GetCell(3) != Null) continue; // �����Ϊ�գ�����

                // ����һ���µ� Answer �����������
                Answer answer = new Answer
                {
                    id = row.GetCell(0).ToString(), // ��ȡ��һ�е����
                    answerTitle = row.GetCell(1).ToString(), // ��ȡ�ڶ��е���Ŀ
                    answerKeys = new List<string>(),         // ��ʼ��ѡ���б�
                    answerType = row.GetCell(3).ToString(), // ��ȡ�����е�����
                    answerTrueKeys = new List<string>()       // ��ʼ�����б�
                };

                // ��ȡѡ��ʹ�����
                String keys = row.GetCell(2).ToString();
                String trueKeys = row.GetCell(4).ToString();
                answer.answerKeys = new List<string>(keys.Split(','));  // ��ѡ���ַ�����ֳɶ���𰸣����洢�����б���
                answer.answerTrueKeys = new List<string>(trueKeys.Split(',')); // �����ַ�����ֳɶ���𰸣����洢�����б���

                // �����õ� Answer ������ӵ����б���
                answers.Add(answer);
            }
        }

        // ���ذ������� Answer ������б�
        return answers;
    }
}
