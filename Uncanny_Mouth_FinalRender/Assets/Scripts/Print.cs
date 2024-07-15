using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using System.Diagnostics;

public class RunPython : MonoBehaviour
{
    private WebSocketSharp.WebSocket ws;
    private Vector3 targetPosition;
    // private bool isMessageReceived = false;

    public string savePath = "C:/Users/CAU/Capstone/temp.txt";
    public string pythonPath = "C:/Users/CAU/anaconda3/python.exe"; 
    public string scriptPath = "C:/Users/CAU/Capstone/print_result.py";
     // public string pythonPath = "C:/Users/CAU/AppData/Local/Microsoft/WindowsApps/python3.exe";

    public string logPrintPath = "C:/Users/CAU/Capstone/errorLog.txt";


    //private WebSocketReceiver wsReceiver; ������ ����Ǳ����� ����Ʈ�� �Ҹ��� ������ �� �� �ֱ⿡,  �򰡰� ������ ���� �Ҹ�������.

    //void Start()
    //{
    //    wsReceiver = FindObjectOfType<WebSocketReceiver>();
    //}

    //private void Update() 
    //{
    //    if (wsReceiver != null)
    //    {
    //        if (wsReceiver.end)
    //        {
    //            wsReceiver.end = false; // Reset the end flag after handling
    //                                    //ConvertAndPlay("������ �ݰ������ϴ�. ����� ö���� ���¸� �м��� �ְڽ��ϴ�.");
    //            saveAssessment();
    //            printAssesment();
    //        }
    //    }
    //}

    public void saveAndPrintAssessment(string assessmentStr)
    {
        // save to file 
        saveAssessment(assessmentStr);
        printAssesment();
    }

    private void saveAssessment(string assessmentStr)
    {
        try
        {
            // Save the assessment string to the specified file
            File.WriteAllText(savePath, assessmentStr);
            UnityEngine.Debug.Log("[�˸�] Assessment saved successfully");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[�˸�] �����߻�: " + e.Message);
        }
    }

    //private void printAssesment()
    //{
    //    UnityEngine.Debug.Log("PrintStart");
    //    try
    //    {
    //        Process psi = new Process();
    //        psi.StartInfo.FileName = pythonPath;
    //        // ���̽� ȯ�� ����
    //        psi.StartInfo.Arguments = scriptPath;
    //        // ������ ���̽� ����
    //        psi.StartInfo.CreateNoWindow = true;
    //        // ��â���� ���� �� ���� �δµ�
    //        psi.StartInfo.UseShellExecute = false;
    //        // ���μ����� �����Ҷ� �ü�� ���� ������� �̰͵� �� ���� �δµ�
    //        psi.Start();

    //        UnityEngine.Debug.Log("[�˸�] .py file ����");
    //    }
    //    catch (Exception e)
    //    {
    //        UnityEngine.Debug.LogError("[�˸�] �����߻�: " + e.Message);
    //    }
    //}


    private void printAssesment()
    {
        UnityEngine.Debug.Log("PrintStart");
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = pythonPath;
            // ���̽� ȯ�� ����
            psi.StartInfo.Arguments = scriptPath;
            // ������ ���̽� ����
            psi.StartInfo.CreateNoWindow = true;
            // ��â���� ���� �� ���� �δµ�
            psi.StartInfo.UseShellExecute = false;
            // ���μ����� �����Ҷ� �ü�� ���� ������� �̰͵� �� ���� �δµ�

            // Redirect standard output and error
            psi.StartInfo.RedirectStandardOutput = true;
            psi.StartInfo.RedirectStandardError = true;

            psi.Start();

            // Read the output
            string output = psi.StandardOutput.ReadToEnd();
            string error = psi.StandardError.ReadToEnd();

            psi.WaitForExit();

            // Log to the specified file
            File.AppendAllText(logPrintPath, $"[�˸�] .py file ���� output at {DateTime.Now}:\n{output}\n");
            if (!string.IsNullOrEmpty(error))
            {
                File.AppendAllText(logPrintPath, $"[�˸�] .py file ���� error at {DateTime.Now}:\n{error}\n");
            }

            UnityEngine.Debug.Log("[�˸�] .py file ����");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[�˸�] �����߻�: " + e.Message);

            // Log error to the specified file
            File.AppendAllText(logPrintPath, $"[�˸�] �����߻�: {e.Message} at {DateTime.Now}\n");
        }
    }
}
