using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    Reality,    // 现实阶段
    WingsuitVR, // 翼装飞行VR
    MountainVR, // 爬山VR
    DivingVR    // 潜水VR
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
            if (instance && instance != this)
                Destroy(instance);

            instance = this;
    }
    private GamePhase _currentPhase;
    private void Start()
    {
        _currentPhase = GamePhase.Reality;
    }
    public void GoToNextPhase()
    {
        switch (_currentPhase)
        {
            case GamePhase.Reality:
                // SceneTransitionManager.instance.GoToSceneAsync("WingsuitVRScene");
                _currentPhase = GamePhase.WingsuitVR;
                break;
            case GamePhase.WingsuitVR:
                // SceneTransitionManager.instance.GoToSceneAsync("RealityScene");
                _currentPhase = GamePhase.Reality;
                break;
            // 其他阶段...
        }
    }

}