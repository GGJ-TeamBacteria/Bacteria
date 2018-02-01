using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BlueprintReality.Performance
{
    public class CPUTracker : StatsTracker
    {
        public const string KeyName = "CPU";

        public override string Key { get { return KeyName; } }

        private Process process;
        private int processorCount;

        private Process killer;

        private string scheduledTaskName = "Kill MixCast Performance Tracker";

        float killTrackerLoopDelay = 15f;

        void Start()
        {
            // kill the tracker process on startup if it is still running
            KillOldTrackerProcess();
            // remove the task to kill tracker if it is still in scheduled tasks
            RemoveScheduledKillTask();
        }

        void OnDestroy()
        {
            StopProcess();
        }

        public override void StartTracking()
        {
            base.StartTracking();

            StartProcess();
        }

        public override void StopTracking()
        {
            base.StopTracking();

            StopProcess();
        }

        private void StartProcess()
        {
            processorCount = SystemInfo.processorCount;

            string pattern = "\\\"([0-9]+\\.[0-9]+)\\\"";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            string procName = Process.GetCurrentProcess().ProcessName;

            process = new Process();
            process.StartInfo.FileName = "typeperf";
            process.StartInfo.Arguments = string.Format("\"\\Process({0})\\% Processor Time\"", procName);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += (sender, args) =>
            {
                var match = rgx.Match(args.Data);

                if (match.Success)
                {
                    float cpuLoadAllCores;
                    if (float.TryParse(match.Groups[1].ToString(), out cpuLoadAllCores))
                    {
                        int cpuLoad = Mathf.RoundToInt(cpuLoadAllCores / processorCount);

                        SetCurrent(cpuLoad);
                    }
                }
            };

            process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("typeperf error " + args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UnityEngine.PlayerPrefs.SetInt("OldTrackerID", process.Id);

            

            InvokeRepeating("DelayedKillTrackerProcess", 0, killTrackerLoopDelay);

        }

        private void KillOldTrackerProcess()
        {
            killer = new Process();
            killer.StartInfo.FileName = "taskkill";
            killer.StartInfo.CreateNoWindow = true;
            //UnityEngine.Debug.Log(process.Id.ToString());
            string arguments = "/f /pid " + UnityEngine.PlayerPrefs.GetInt("OldTrackerID");
            killer.StartInfo.Arguments = arguments;
            killer.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("kill tracker now error " + args.Data);
            killer.Start();
        }

        private void RemoveScheduledKillTask()
        {
            // Delete the scheduled task that kills the tracker
            Process endScheduleKill = new Process();
            endScheduleKill.StartInfo.FileName = "schtasks";
            string endScheduleKillArgs = "/delete /tn \"" + scheduledTaskName + "\" /F";
            //UnityEngine.Debug.Log(endScheduleKillArgs);
            endScheduleKill.StartInfo.Arguments = endScheduleKillArgs;
            endScheduleKill.StartInfo.CreateNoWindow = true;
            endScheduleKill.StartInfo.UseShellExecute = false;
            endScheduleKill.StartInfo.RedirectStandardError = true;
            endScheduleKill.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("end kill tracker error " + args.Data);
            endScheduleKill.Start();
        } 

        private void DelayedKillTrackerProcess()
        {
            UnityEngine.Debug.Log("Invoke method to delete scheduled task to kill typeperf.exe, and create new scheduled tasks. This happens every " + killTrackerLoopDelay + " seconds while performance tracker is enabled.");
            
            // Kill tracker at a specific time
            /*
            Process scheduleKill = new Process();
            scheduleKill.StartInfo.FileName = "schtasks";
            string killArguments = "/pid " + process.Id.ToString() + " /F";
            string schedArguments = "/create /tn \"Kill Tracker\" /tr \"taskkill " + killArguments + "\"" + " /sc once /st 13:42:00 /F";
            UnityEngine.Debug.Log(schedArguments);
            scheduleKill.StartInfo.Arguments = schedArguments;
            scheduleKill.StartInfo.CreateNoWindow = true;
            scheduleKill.StartInfo.UseShellExecute = false;
            scheduleKill.StartInfo.RedirectStandardError = true;
            scheduleKill.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("start kill tracker error " + args.Data);
            scheduleKill.Start();
            */

            // See comments on Assembla ticket #319 for more detail about the limitations of this implementation
            // https://blueprintreality.assembla.com/spaces/mixcast-studio/tickets/319-(1-6)-when-the-user-closes-the-application-it-remains-in-the-background/details

            // Delete the scheduled task that kills the tracker (clear the task every time this method is invoked)
            RemoveScheduledKillTask();
     
            // Schedule task to kill tracker task in x minutes
            Process scheduleKill = new Process();
            scheduleKill.StartInfo.FileName = "schtasks";
            scheduleKill.StartInfo.CreateNoWindow = true;
            string killArguments = "/pid " + process.Id.ToString() + " /F";
            int hour = System.DateTime.Now.Hour;
            int minute = System.DateTime.Now.Minute;
            int second = 0; // schtasks.exe ignores seconds so don't need them
            //UnityEngine.Debug.Log(hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2"));

            /*
             * 2 minutes is minimum delay, if set lower then typeperf gets killed while the tracker is active
             * 
             * Explanation - this happens because the delay is not the exact amount of time in which the scheduled task will run.
             * Rather, it is the number of minutes (rounded to the minute) for when the task is scheduled (schtasks.exe ignores seconds!)
             * 
             * For example, a task scheduled at 1:49:49 with a 2-minute delay will run at 1:51:00
             * 
             * So, if you have a 1 minute delay, you can run into a situation like the following example:
             * At 1:50:55, a task with a 1-minute delay is scheduled for 1:51:00
             * but that is only 5 seconds away, so the Unity Invoke loop (currently set to invoke every 15 seconds),
             * does not have time to occur, causing the typeperf.exe process to get killed even though Unity is still running and the tracker is enabled.
             * 
             * */

            int delayInMinutes = 2;
            int minDelayInMinutes = 2;
            int maxDelayInMinutes = 59;
            if (delayInMinutes > maxDelayInMinutes || delayInMinutes < minDelayInMinutes)
            {
                UnityEngine.Debug.LogError("minimum delay is 2 minutes; max is 59");
                delayInMinutes = minDelayInMinutes;
            }

            minute += delayInMinutes;

            if (minute > 59)
            {
                hour++;
                minute = minute - 60;
            }

            if (hour > 24)
            {
                hour = 0;
            }

            string timeToKill = hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2");
            //UnityEngine.Debug.Log(timeToKill);
            string schedArguments = "/create /tn \"" + scheduledTaskName + "\" /tr \"taskkill " + killArguments + "\"" + " /sc once /st " + timeToKill + " /F";
            //UnityEngine.Debug.Log(schedArguments);
            scheduleKill.StartInfo.Arguments = schedArguments;
            scheduleKill.StartInfo.UseShellExecute = false;
            scheduleKill.StartInfo.RedirectStandardError = true;
            scheduleKill.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log("start kill tracker error " + args.Data);
            scheduleKill.Start();

        }

        private void StopProcess()
        {
            try
            {
                if (process != null
                    && !process.HasExited)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error during killing CPU stats process: " + e.Message);
            }
        }

        void OnApplicationQuit()
        {
            //UnityEngine.Debug.Log("OnApplicationQuit");
            StopProcess();
        }
    }
}