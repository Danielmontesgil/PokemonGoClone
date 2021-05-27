using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[Serializable]
public class PokedexPage
{
}

[Serializable]
public class Pokemon
{
    public string name;
}

public class PokeAPIController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon/?limit=10&offset=10/");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);

            var data = JsonUtility.FromJson<object>(www.downloadHandler.text);

            Debug.LogError(data);
        }
    }
}