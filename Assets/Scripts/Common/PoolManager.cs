using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, List<GameObject>> objectsPool = new Dictionary<string, List<GameObject>>();

    public GameObject GetObject(string path, bool multipleObjs, bool isRandom)
    {
        if (!objectsPool.ContainsKey(path))
        {
            objectsPool.Add(path, new List<GameObject>());
        }

        if (objectsPool[path].Count == 0)
            AddObject(path,multipleObjs);

        return AllocateObject(path, isRandom);
    }

    public void ReleaseObject(string path, GameObject prefab, bool returnToPool)
    {
        prefab.gameObject.SetActive(false);
        prefab.transform.SetParent(this.transform);

        if (returnToPool)
        {
            if (!objectsPool.ContainsKey(path))
            {
                objectsPool.Add(path, new List<GameObject>());
            }
            objectsPool[path].Add(prefab);
        }
    }

    private void AddObject(string path, bool multipleObjs)
    {
        GameObject instance = null;
        if (multipleObjs)
        {
            List<GameObject> objs = ResourcesManager.Instance.GetAllGameObjects(path);

            foreach (var obj in objs)
            {
                instance = Instantiate(obj, transform);
                AddObject(instance, path);
            }
        }
        else
        {
            instance = Instantiate(ResourcesManager.Instance.GetGameObject(path), transform);
            AddObject(instance, path);
        }
    }

    private void AddObject(GameObject instance,string path)
    {
        instance.gameObject.SetActive(false);
        instance.transform.position = this.transform.position;
        objectsPool[path].Add(instance);
    }

    private GameObject AllocateObject(string path, bool isRandom)
    {
        GameObject objectPool = null;
        int index = 0;
        if (isRandom)
        {
            index = Random.Range(0, objectsPool[path].Count);
            objectPool = objectsPool[path][index];
        }
        else
        {
            objectPool = objectsPool[path][0];
        }

        objectsPool[path].RemoveAt(index);
        objectPool.gameObject.SetActive(true);
        return objectPool;
    }
}
