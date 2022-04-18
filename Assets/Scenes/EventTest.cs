using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class EventTest:MonoBehaviour
{
    EventTool mEventTool;
    private void Awake() {
        mEventTool = new EventTool();
        mEventTool.Init();
    }

    private void OnGameStart(bool loadRes,int repeatTime) {
        Debug.Log($"是否加载资源:{loadRes}");
        Debug.Log($"重试次数{repeatTime}");
    }

    private void RegisterEvents() {
        mEventTool.On<bool, int>(GameEvent.GAME_START, OnGameStart);
    }

    private void UnRegisterEvents() {
        mEventTool.OffTarget(this);
    }

    private void UnRegisterBySingle() {
        mEventTool.Off<bool,int>(GameEvent.GAME_START,OnGameStart);
    }

    private void EmitEvents() {
        mEventTool.Emit(GameEvent.GAME_START,true,3);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RegisterEvents();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            EmitEvents();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UnRegisterEvents();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UnRegisterBySingle();
        }
    }
}
