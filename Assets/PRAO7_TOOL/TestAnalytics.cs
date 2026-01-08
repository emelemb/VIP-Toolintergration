using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class TestAnalytics : MonoBehaviour 
{
    public static TestAnalytics Instance { get; private set; }

    private List<TestObject> coffeeList;
    private List<TestObject> dartList;
    private List<TestObject> printerList;
    private List<TestObject> cleaningList;
    public List<List<TestObject>> allLists { get; private set; }


    //private Dictionary<taskEnum, float> taskTimes;
    //private Dictionary<taskEnum, int> taskPlays;
    //private Dictionary<taskEnum, int> taskClicks;
    //private taskEnum[] allTasks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        allLists = new List<List<TestObject>>();
        coffeeList = new List<TestObject>();
        dartList = new List<TestObject>();
        printerList = new List<TestObject>();
        cleaningList = new List<TestObject>();
        allLists.Add(coffeeList);
        allLists.Add(dartList);
        allLists.Add(printerList);
        allLists.Add(cleaningList);
    }

    public void ResetTesting()
    {
        allLists = new List<List<TestObject>>();
        coffeeList = new List<TestObject>();
        dartList = new List<TestObject>();
        printerList = new List<TestObject>();
        cleaningList = new List<TestObject>();
        allLists.Add(coffeeList);
        allLists.Add(dartList);
        allLists.Add(printerList);
        allLists.Add(cleaningList);
    }

    //public void RecordMinigameData(taskEnum taskEnum, float time, int clicks, bool accomplished)
    //{
    //    switch (taskEnum)
    //    {
    //        case taskEnum.Dart:
    //            dartList.Add(new TestObject(taskEnum, time, clicks, accomplished));
    //            break;
    //        case taskEnum.Coffee:
    //            coffeeList.Add(new TestObject(taskEnum, time, clicks, accomplished));
    //            break;
    //        case taskEnum.Printer:
    //            printerList.Add(new TestObject(taskEnum, time, clicks, accomplished));
    //            break;
    //        case taskEnum.Mopping:
    //            cleaningList.Add(new TestObject(taskEnum, time, clicks, accomplished));
    //            break;
    //    }
    //}

    public void RecordMinigameData( float time, int clicks, bool accomplished)
    {      
     dartList.Add(new TestObject( time, clicks, accomplished));   
    }

}

public class TestObject
{
   // public taskEnum taskEnum { get; set; }
    public float time { get; set; }
    public int clicks { get; set; }
    public bool accomplished { get; set; }
    //public TestObject(taskEnum taskEnum, float time, int clicks, bool accomplished)
    //{
    //    this.taskEnum = taskEnum;
    //    this.time = time;
    //    this.clicks = clicks;
    //    this.accomplished = accomplished;
    //}

    public TestObject( float time, int clicks, bool accomplished)
    {
        this.time = time;
        this.clicks = clicks;
        this.accomplished = accomplished;
    }
}

