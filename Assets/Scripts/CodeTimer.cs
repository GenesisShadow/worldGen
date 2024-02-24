using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics; 

public static class CodeTimer
{
    //debug timer, times how long code taks
	public static Stopwatch codeTimer = new Stopwatch();

    public static List<double>average = new List<double>();

    public static void Start(){
        codeTimer.Start();
    }
	public static void Stop(string name){
		//call CodeTimer.Start(); before calling this function
		codeTimer.Stop();
		TimeSpan ts = codeTimer.Elapsed;
		double milliseconds = codeTimer.Elapsed.TotalMilliseconds;
		string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
		ts.Hours, ts.Minutes, ts.Seconds,
		milliseconds);
		UnityEngine.Debug.Log(name + " RunTime: " + elapsedTime);
		codeTimer.Reset();
	}

    public static void StopAverage(){
        codeTimer.Stop();
		TimeSpan ts = codeTimer.Elapsed;
		double milliseconds = codeTimer.Elapsed.TotalMilliseconds;
        if(milliseconds > 0.01)
            average.Add(milliseconds);
		codeTimer.Reset();
    }

    public static void CollectAverage(string name){
        int count = average.Count;
        double total = 0;
        for(int i = 0; i < count; i++){
            total += average[i];
        }
        UnityEngine.Debug.Log(name + "average RunTime: " + total/count);
    }
}
