using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public GameObject GetGameObject(string path)
    {
        return Resources.Load<GameObject>(path);
    }

    public List<GameObject> GetAllGameObjects(string path)
    {
        return Resources.LoadAll<GameObject>(path).ToList();
    }
}
