using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("旁白Timeline绑定表")]
    public List<NamedNarration> narrationTimelines;

    private Dictionary<string, PlayableDirector> narrationDict;

    /*// 事件名
    public const string PlayDialogue = "PlayDialogue";
    public const string DialogueFinished = "DialogueFinished";*/

    [Serializable]
    public class NamedNarration
    {
        public string key;
        public PlayableDirector director; // 改为绑定 PlayableDirector 而非 Asset
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

        // 初始化 key → Director 字典
        narrationDict = new Dictionary<string, PlayableDirector>();
        foreach (var item in narrationTimelines)
        {
            if (!narrationDict.ContainsKey(item.key))
            {
                narrationDict.Add(item.key, item.director);
            }
        }
    }

    /*
    [SerializeField] GameObject eventManager; // 事件管理器引用
    */
    
    private void OnEnable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.Subscribe(PhotoGrabTrigger.OnPhotoGrabbed, OnPlayDialogue);
        else
            Debug.LogWarning("EventManager.Instance 为空，DialogueManager 订阅失败");;
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PhotoGrabTrigger.OnPhotoGrabbed, OnPlayDialogue);
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

        var targetDirector = narrationDict[key];
        targetDirector.stopped -= OnDialogueFinished;
        targetDirector.stopped += OnDialogueFinished;

        targetDirector.Play();
    }

    private void OnDialogueFinished(PlayableDirector _)
    {
        Debug.Log("旁白播放完成！");
    }
}
