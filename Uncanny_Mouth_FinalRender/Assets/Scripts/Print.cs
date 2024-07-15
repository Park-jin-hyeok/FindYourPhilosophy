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


    //private WebSocketReceiver wsReceiver; 내용이 저장되기전에 프린트가 불리면 오류가 날 수 있기에,  평가가 생성된 이후 불리도록함.

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
    //                                    //ConvertAndPlay("만나서 반가웠습니다. 당신의 철학적 상태를 분석해 주겠습니다.");
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
            UnityEngine.Debug.Log("[알림] Assessment saved successfully");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);
        }
    }

    //private void printAssesment()
    //{
    //    UnityEngine.Debug.Log("PrintStart");
    //    try
    //    {
    //        Process psi = new Process();
    //        psi.StartInfo.FileName = pythonPath;
    //        // 파이썬 환경 연결
    //        psi.StartInfo.Arguments = scriptPath;
    //        // 실행할 파이썬 파일
    //        psi.StartInfo.CreateNoWindow = true;
    //        // 새창에서 시작 걍 일케 두는듯
    //        psi.StartInfo.UseShellExecute = false;
    //        // 프로세스를 시작할때 운영체제 셸을 사용할지 이것도 걍 일케 두는듯
    //        psi.Start();

    //        UnityEngine.Debug.Log("[알림] .py file 실행");
    //    }
    //    catch (Exception e)
    //    {
    //        UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);
    //    }
    //}


    private void printAssesment()
    {
        UnityEngine.Debug.Log("PrintStart");
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = pythonPath;
            // 파이썬 환경 연결
            psi.StartInfo.Arguments = scriptPath;
            // 실행할 파이썬 파일
            psi.StartInfo.CreateNoWindow = true;
            // 새창에서 시작 걍 일케 두는듯
            psi.StartInfo.UseShellExecute = false;
            // 프로세스를 시작할때 운영체제 셸을 사용할지 이것도 걍 일케 두는듯

            // Redirect standard output and error
            psi.StartInfo.RedirectStandardOutput = true;
            psi.StartInfo.RedirectStandardError = true;

            psi.Start();

            // Read the output
            string output = psi.StandardOutput.ReadToEnd();
            string error = psi.StandardError.ReadToEnd();

            psi.WaitForExit();

            // Log to the specified file
            File.AppendAllText(logPrintPath, $"[알림] .py file 실행 output at {DateTime.Now}:\n{output}\n");
            if (!string.IsNullOrEmpty(error))
            {
                File.AppendAllText(logPrintPath, $"[알림] .py file 실행 error at {DateTime.Now}:\n{error}\n");
            }

            UnityEngine.Debug.Log("[알림] .py file 실행");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);

            // Log error to the specified file
            File.AppendAllText(logPrintPath, $"[알림] 에러발생: {e.Message} at {DateTime.Now}\n");
        }
    }
}
