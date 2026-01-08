using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestDataCompiler : MonoBehaviour
{
    public void ExportStatsToCSV()
    {
        //BasDataNeeded.Instance.missionAccomplished = true;
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string path = Path.Combine(Application.persistentDataPath, $"task_stats{timeStamp}.csv");

        using (StreamWriter writer = new StreamWriter(path))
        {
            //writer.WriteLine("Day: " + StringLiterals.MAIN_SCENE.ToString().Replace("Day", string.Empty));
            writer.WriteLine("Minigame, Total Time, Total Clicks, Mission Accomplished");

            for (int i = 0; i < TestAnalytics.Instance.allLists.Count; i++)
            {
                for (int j = 0; j < TestAnalytics.Instance.allLists[i].Count; j++)
                {
                    var row = TestAnalytics.Instance.allLists[i][j];
                    // writer.WriteLine($"{row.taskEnum + " " + j}, {row.time}, {row.clicks}, {row.accomplished}");
                    writer.WriteLine("{ row.time}, { row.clicks}, { row.accomplished}");
                    
                }
            }
        }

        Debug.Log("task_stats.csv exported to: " + path);
        TestAnalytics.Instance.ResetTesting();
    }



}
