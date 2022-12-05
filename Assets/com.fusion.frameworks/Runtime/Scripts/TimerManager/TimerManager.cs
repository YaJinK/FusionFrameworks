using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Timer
{
    public class Timer
    {
        public enum InvokeType
        {
            Frame,
            Time
        }

        private float frequency = 1;
        private int count = 1;
        private int round = 0;
        private InvokeType type = InvokeType.Time;

        private Action action = null;
        private bool enabled = true;

        public float Frequency { get => frequency; }
        public int Count { get => count;}
        public int Round { get => round; }
        public InvokeType Type { get => type; }
        public Action Action { get => action; }
        public bool Enabled { get => enabled; }

        public void Pause()
        {
            enabled = false;
        }

        public void Play()
        {
            enabled = true;
        }

        /// <summary>
        /// 计时器执行频率 默认 1
        /// Type = Frame  frequency是帧数
        /// Type = Time frequency是秒数
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public Timer FrequencySetter(float frequency)
        {
            this.frequency = frequency;
            return this;
        }

        /// <summary>
        /// 计时器每一轮调用自定义方法的次数 默认 1
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Timer CountSetter(int count)
        {
            this.count = count;
            return this;
        }

        /// <summary>
        /// 计时器一共执行几轮
        /// Round 默认 0 循环执行
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Timer RoundSetter(int round)
        {
            this.round = round;
            return this;
        }

        /// <summary>
        /// 计时器类型
        /// Frame 隔帧执行
        /// Time 隔秒执行
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Timer TypeSetter(InvokeType type)
        {
            this.type = type;
            return this;
        }

        /// <summary>
        /// 计时器开始执行某个方法，设置好参数后最后调用
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Timer Invoke(Action action)
        {
            this.action = action;
            TimerManager.Instance.ScheduleTimer(this);
            return this;
        }
    }

    public class TimerManager : MonoBehaviour
    {
        private static TimerManager instance;

        public static TimerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject timerManagerObject = new GameObject("TimerManager");
                    instance = timerManagerObject.AddComponent<TimerManager>();
                    DontDestroyOnLoad(timerManagerObject);
                }
                return instance;
            }
        }

        private class TimerData { 
            public Timer timer = null;
            public float frequencyCounter = 0;
            public float roundCounter = 0;
        }

        private List<TimerData> timerDatas = new List<TimerData>();

        void LateUpdate()
        {
            for (int index = timerDatas.Count - 1; index >= 0; index--)
            {
                TimerData data = timerDatas[index];
                HandleTimerData(data);
            }
        }

        private void HandleTimerData(TimerData timerData)
        {
            Timer timer = timerData.timer;
            if (!timer.Enabled)
            {
                return;
            }
            if (timer.Type == Timer.InvokeType.Frame)
            {
                timerData.frequencyCounter++;
            }
            else if (timer.Type == Timer.InvokeType.Time)
            {
                timerData.frequencyCounter += Time.unscaledDeltaTime;
            }
            if (timerData.frequencyCounter >= timer.Frequency)
            {
                for (int index=0; index < timer.Count; index++)
                {
                    timer.Action();
                }
                timerData.frequencyCounter -= timer.Frequency;
                if (timer.Round > 0)
                {
                    timerData.roundCounter++;
                    if (timerData.roundCounter >= timer.Round)
                    {
                        timerDatas.Remove(timerData);
                    }
                }
            }
        }

        public void ScheduleTimer(Timer timer)
        {
            if (!CheckTimerScheduled(timer))
            {
                TimerData timerData = new TimerData();
                timerData.timer = timer;
                timerDatas.Add(timerData);
            }
        }

        private bool CheckTimerScheduled(Timer timer)
        {
            for (int index = 0; index < timerDatas.Count; index++)
            {
                if (timerDatas[index].timer == timer)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveTimer(Timer timer)
        {
            for (int index = 0; index < timerDatas.Count; index++)
            {
                if (timerDatas[index].timer == timer)
                {
                    timerDatas.RemoveAt(index);
                    break;
                }
            }
        }

        public void Clear()
        {
            timerDatas.Clear();
        }

        public void Pause()
        {
            for (int index = 0; index < timerDatas.Count; index++)
            {
                timerDatas[index].timer.Pause();
            }
        }

        public void Play()
        {
            for (int index = 0; index < timerDatas.Count; index++)
            {
                timerDatas[index].timer.Play();
            }
        }
    }
}

