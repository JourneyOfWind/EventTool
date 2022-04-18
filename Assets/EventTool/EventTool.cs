using System;
using System.Collections.Generic;
using UnityEngine;

public class EventTool
{
    private Dictionary<string, List<Delegate>> mDelegateDict;
    private Dictionary<object, List<EventWithDelegate>> mTargetEventDict;

    public void Init() {
        mDelegateDict    = new Dictionary<string, List<Delegate>>();
        mTargetEventDict = new Dictionary<object, List<EventWithDelegate>>();
    }

    private void OnListenerAdding(string eventType, Delegate dGate)
    {
        if (!mDelegateDict.ContainsKey(eventType))
            mDelegateDict.Add(eventType, new List<Delegate>());
        else
        {
            if (mDelegateDict[eventType].Contains(dGate))
                throw new Exception($"事件{eventType}和事件{dGate.GetType()}已存在，请勿重复添加");
        }

        List<Delegate> dList = mDelegateDict[eventType];
        Delegate d = null;
        if (dList.Count > 0)
            d = dList[0];

        if (d != null && d.GetType() != dGate.GetType())
            throw new Exception($"尝试为事件{eventType}添加不同类型的委托，当前事件所对应的委托是{d.GetType()}，要添加的委托类型为{dGate.GetType()}");

        if (!mTargetEventDict.ContainsKey(dGate.Target))
            mTargetEventDict.Add(dGate.Target, new List<EventWithDelegate>());

        Debug.Log($"消息 {eventType} 注册成功");
    }

    private void OnListenerRemoving(string eventType, Delegate dGate)
    {
        if (mDelegateDict.ContainsKey(eventType))
        {
            var dList = mDelegateDict[eventType];
            if (dList.Count <= 0)
                throw new Exception($"移除监听错误：事件{eventType}没有对应的委托");

            if (dList[0].GetType() != dGate.GetType())
                throw new Exception($"移除监听错误：尝试为事件 {eventType} 移除不同类型的委托，当前委托类型为 {dList[0].GetType()} ，要移除的委托类型为 {dGate.GetType()} ");
        }
        else
            throw new Exception($"移除监听错误：没有事件码 {eventType} ");

        if (!mTargetEventDict.ContainsKey(dGate.Target))
            throw new Exception($"当前委托 {dGate.GetType()} 的实体 {dGate.Target} 已不存在");

        EventWithDelegate ed = mTargetEventDict[dGate.Target].Find(ed => ed.EventType == eventType);
        if (ed == null)
            throw new Exception($"当前实体  {dGate.Target}未找到绑定的委托 {dGate.GetType()} ");
        if (ed.DGate == null)
        {
            throw new Exception($"当前实体 {dGate.Target} 未找到已绑定的委托");
        }
        if (ed.DGate != dGate)
            throw new Exception($"将要移除的委托 {dGate.Method.Name} 与绑定的委托 {ed.DGate.Method.Name} 不一致");

        Debug.Log($"消息 {eventType} 注销成功");
    }

    private void OnListenerRemoved(string eventType, Delegate dGate)
    {

        if (mDelegateDict[eventType].Count == 0)
        {
            mDelegateDict.Remove(eventType);
        }

        if (mTargetEventDict[dGate.Target].Count == 0)
        {
            mTargetEventDict.Remove(dGate.Target);
        }
    }


    #region 无参数
    public void On(string eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        mDelegateDict[eventType].Add(callBack);
        mTargetEventDict[callBack.Target].Add(new EventWithDelegate(eventType, callBack));

    }
    public void Off(string eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mDelegateDict[eventType].Remove(callBack);

        EventWithDelegate ed = mTargetEventDict[callBack.Target].Find(ed => ed.EventType == eventType);
        mTargetEventDict[callBack.Target].Remove(ed);

        OnListenerRemoved(eventType, callBack);

    }
    public void Emit(string eventType)
    {
        mDelegateDict.TryGetValue(eventType, out List<Delegate> deList);
        if (deList == null || deList.Count <= 0)
        {
            Debug.Log($"事件{eventType}无方法接收");
            return;
        }
        foreach (Delegate dGate in deList)
        {
            if (dGate is CallBack callBack)
                callBack();
            else
                throw new Exception($"广播事件错误：事件{eventType}对应委托具有不同的类型");
        }
    }

    #endregion

    #region 一个参数
    public void On<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mDelegateDict[eventType].Add(callBack);
        mTargetEventDict[callBack.Target].Add(new EventWithDelegate(eventType, callBack));
    }
    public void Off<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mDelegateDict[eventType].Remove(callBack);

        EventWithDelegate ed = mTargetEventDict[callBack.Target].Find(ed => ed.EventType == eventType);
        mTargetEventDict[callBack.Target].Remove(ed);

        OnListenerRemoved(eventType, callBack);

    }
    public void Emit<T>(string eventType, T arg1)
    {
        mDelegateDict.TryGetValue(eventType, out List<Delegate> deList);
        foreach (Delegate dGate in deList)
        {
            if (dGate is CallBack<T> callBack)
                callBack(arg1);
            else
                throw new Exception($"广播事件错误：事件{eventType}对应委托具有不同的类型");
        }
    }

    #endregion

    #region 两个参数
    public void On<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mDelegateDict[eventType].Add(callBack);
        mTargetEventDict[callBack.Target].Add(new EventWithDelegate(eventType, callBack));

    }
    public void Off<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mDelegateDict[eventType].Remove(callBack);

        EventWithDelegate ed = mTargetEventDict[callBack.Target].Find(ed => ed.EventType == eventType);
        mTargetEventDict[callBack.Target].Remove(ed);

        OnListenerRemoved(eventType, callBack);

    }
    public void Emit<T, X>(string eventType, T arg1, X arg2)
    {
        if (mDelegateDict.TryGetValue(eventType, out List<Delegate> deList))
        {
            foreach (Delegate dGate in deList) {
                if (dGate is CallBack<T, X> callBack)
                    callBack(arg1, arg2);
                else
                    throw new Exception($"广播事件错误：事件{eventType}对应委托具有不同的类型");
            }
        }
    }

    #endregion

    #region 三个参数
    public void On<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mDelegateDict[eventType].Add(callBack);
        mTargetEventDict[callBack.Target].Add(new EventWithDelegate(eventType, callBack));

    }
    public void Off<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mDelegateDict[eventType].Remove(callBack);

        EventWithDelegate ed = mTargetEventDict[callBack.Target].Find(ed => ed.EventType == eventType);
        mTargetEventDict[callBack.Target].Remove(ed);

        OnListenerRemoved(eventType, callBack);

    }
    public void Emit<T, X, Y>(string eventType, T arg1, X arg2, Y arg3)
    {
        mDelegateDict.TryGetValue(eventType, out List<Delegate> deList);
        foreach (Delegate dGate in deList)
        {
            if (dGate is CallBack<T, X, Y> callBack)
                callBack(arg1, arg2, arg3);
            else
                throw new Exception($"广播事件错误：事件{eventType}对应委托具有不同的类型");
        }
    }
    #endregion


    public void OffTarget(object target)
    {
        if (mTargetEventDict.TryGetValue(target,out List<EventWithDelegate> targetEvent))
        {
            while (targetEvent.Count > 0)
            {
                EventWithDelegate removeEvent = targetEvent[0];
                //移除事件
                OnListenerRemoving(removeEvent.EventType, removeEvent.DGate);
                mDelegateDict[removeEvent.EventType].Remove(removeEvent.DGate);
                //移除实体持有事件和委托
                targetEvent.Remove(removeEvent);

                OnListenerRemoved(removeEvent.EventType, removeEvent.DGate);
                Debug.Log($"目标：{target} 的委托 {removeEvent.DGate.Method.Name} 已注销");
            }
        }
        else
        {
            Debug.Log($"{target}未找到");
        }
    }

    public void OnDestroy() {
        this.mTargetEventDict.Clear();
        this.mDelegateDict.Clear();
    }

    private class EventWithDelegate
    {
        public string EventType;
        public Delegate DGate;

        public EventWithDelegate(string eventTypeType, Delegate dGate)
        {
            this.EventType = eventTypeType;
            this.DGate = dGate;
        }
    }
}
