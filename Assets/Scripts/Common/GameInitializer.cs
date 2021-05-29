using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Init();
        PlayFabManager.Init();
        LoadSceneManager.Init();
        ResourcesManager.Init();
    }
}
