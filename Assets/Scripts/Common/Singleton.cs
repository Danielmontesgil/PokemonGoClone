using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    public static void Init()
    {
        if (instance == null)
        {
            CreateInstance();
        }
    }

    private static void CreateInstance()
    {
        GameObject newObject = GameObject.Find("Managers");
        if (newObject == null)
        {
            newObject = new GameObject();
            newObject.name = "Managers";
            newObject.transform.position = Vector3.one * 100;
        }
        instance = newObject.AddComponent<T>();
        DontDestroyOnLoad(newObject);
    }

    public static bool HasInstance()
    {
        return instance != null;
    }
}
