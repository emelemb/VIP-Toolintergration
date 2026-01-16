using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


class DayLogInfo
{
    public int dayNumber;
    public int timesClicked;
    public float timeActive;
}

public class TestDataCompiler : MonoBehaviour
{
    bool toolON = false;
    int mouseClicks = 0;
    bool MissionWasAccomplished;
    int testNumber;
    string toolFolderName = "";
    float recordingTime;
    private bool isFirstTimeQueueing;
    List<DayLogInfo> dayLogs;
    GameManager.GameDay day;
    private int currentDay;

    public bool missionAccomplished
    {
        get { return MissionWasAccomplished; }
        set { MissionWasAccomplished = value; }
    }
   // can be if we win or lose in their game 

    public void ExportStatsToCSV()
    {
        //BasDataNeeded.Instance.missionAccomplished = true;
        string path = Path.Combine(Application.dataPath + toolFolderName, $"task_stats.csv");

        using (StreamWriter writer = new StreamWriter(path))
        {
            //writer.WriteLine("Day: " + StringLiterals.MAIN_SCENE.ToString().Replace("Day", string.Empty));
            writer.WriteLine($"Total Time:{recordingTime} s, Total Clicks:{mouseClicks}\n");


            foreach (DayLogInfo dayLog in dayLogs)
            {
                writer.WriteLine($"Day {dayLog.dayNumber}, Times Clicked:{dayLog.timesClicked}, Time Active:{dayLog.timeActive}\n");
            }

            //for (int i = 0; i < TestAnalytics.Instance.allLists.Count; i++)
            //{
            //    for (int j = 0; j < TestAnalytics.Instance.allLists[i].Count; j++)
            //    {
            //        var row = TestAnalytics.Instance.allLists[i][j];
            //        // writer.WriteLine($"{row.taskEnum + " " + j}, {row.time}, {row.clicks}, {row.accomplished}");
            //        writer.WriteLine("{ row.time}, { row.clicks}, { row.accomplished}");
            //    }
            //}
        }
        mouseClicks = 0;
        recordingTime = 0;
        testNumber = 0;

        Debug.Log("task_stats.csv exported to: " + path);
        //TestAnalytics.Instance.ResetTesting();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        isFirstTimeQueueing = true;
        dayLogs = new List<DayLogInfo>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CharacterTest") && isFirstTimeQueueing)
        {
            day = GameManager.Instance.Day;
            currentDay = 0;
            DayLogInfo logInfo = new DayLogInfo();
            logInfo.dayNumber = currentDay;
            dayLogs.Add(logInfo);
            isFirstTimeQueueing = false;
        }
        else
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("CharacterTest") && day != GameManager.Instance.Day && !isFirstTimeQueueing)
            {
                day = GameManager.Instance.Day;
                currentDay++;
                DayLogInfo logInfo = new DayLogInfo();
                logInfo.dayNumber = currentDay;
                dayLogs.Add(logInfo);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!toolON)
            {
                StartTesting();
            }
            if (toolON)
            {

                ExportStatsToCSV();
            }
            toolON = !toolON;
        }

        if (toolON)
        {
            recordingTime += Time.deltaTime; // now total time
            if (!isFirstTimeQueueing)
            {
            dayLogs[dayLogs.Count - 1].timeActive += Time.deltaTime; // per day time
            }
            if (Input.GetMouseButtonDown(0))
            {
                mouseClicks++;  // add timer so we mabye dont take screenshots EVERYTIME  have it only be able to trigger like once every 2 sec. 
                if (!isFirstTimeQueueing)
                {
                dayLogs[dayLogs.Count - 1].timesClicked++; // per day clicks
                }
                ScreenCapture.CaptureScreenshot(Application.dataPath + toolFolderName + "Screenshot" + testNumber + ".jpeg", 1);
                testNumber++;
            }
        }
        // Need something to reset the mouseclick after ....a point? 
        // they have an enum for which day it is, if we get that information we could use that. 
        // we gotta check their code -> see what we can use;
    }

    private void StartTesting()
    {
        toolFolderName = $"/TestData/TestSession {DateTime.Now:yyyy-MM-dd_HH-mm-ss}/";
        Directory.CreateDirectory(Application.dataPath + toolFolderName);
    }

    // Method below should run when u click to export the data;
    public void collectData()
    {       //TestAnalytics.Instance.RecordMinigameData(taskID, Time.timeSinceLevelLoad, mouseClicks, MissionWasAccomplished);
        TestAnalytics.Instance.RecordMinigameData(Time.timeSinceLevelLoad, mouseClicks, MissionWasAccomplished);
    }


}
