using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("旁白Timeline绑定表")]
    public List<NamedNarration> narrationTimelines;

    private Dictionary<string, PlayableAsset> narrationDict;
    private PlayableDirector director;

    // 事件名
    public const string PlayDialogue = "PlayDialogue";
    public const string DialogueFinished = "DialogueFinished";

    [Serializable]
    public class NamedNarration
    {
        public string key;
        public PlayableAsset timelineAsset;
    }

    private void Awake()
    {
        // 单例
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        director = GetComponent<PlayableDirector>();

        // 初始化 key → Timeline 字典
        narrationDict = new Dictionary<string, PlayableAsset>();
        foreach (var item in narrationTimelines)
        {
            if (!narrationDict.ContainsKey(item.key))
            {
                narrationDict.Add(item.key, item.timelineAsset);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(PlayDialogue, OnPlayDialogue);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(DialogueFinished, OnPlayDialogue);
    }

    private void OnPlayDialogue(object param)
    {
        string key = param as string;
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("PlayNarration 参数为空！");
            return;
        }
        if (!narrationDict.ContainsKey(key))
        {
            Debug.LogWarning($"未找到旁白 Timeline，key: {key}");
            return;
        }
        var asset = narrationDict[key];
        director.playableAsset = asset;
        // 清除旧的回调，避免重复注册
        director.stopped -= OnDialogueFinished;
        director.stopped += OnDialogueFinished;

        director.Play();
    }

    private void OnDialogueFinished(PlayableDirector _)
    {
        Debug.Log("旁白播放完成！");
        EventManager.Instance.Trigger(DialogueFinished);
    }
}
