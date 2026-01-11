using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestDataCompiler : MonoBehaviour
{
    bool toolON = false;
    int mouseClicks = 0;
    bool MissionWasAccomplished;
    int testNumber;
    string toolFolderName = "";
    float recordingTime;

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
            writer.WriteLine($"Time:{recordingTime} s, Clicks:{mouseClicks}");

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
    }

    void Update()
    {
      
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
            recordingTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                mouseClicks++;  // add timer so we mabye dont take screenshots EVERYTIME  have it only be able to trigger like once every 2 sec. 
                ScreenCapture.CaptureScreenshot(Application.dataPath + toolFolderName + "Screenshot" + testNumber + ".jpeg", -1);
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
