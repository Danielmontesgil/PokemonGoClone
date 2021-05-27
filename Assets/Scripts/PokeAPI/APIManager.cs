using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using SimpleJSON;

public class APIManager : MonoBehaviour
{
    private JSONNode data;

    void Start()
    {
        EventManager.Instance.AddListener<OnAPIRequestEvent>(OnAPIRequestEventListener);
        EventManager.Instance.AddListener<OnImageRequestEvent>(OnImageRequestEventListener);
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnAPIRequestEvent>(OnAPIRequestEventListener);
            EventManager.Instance.RemoveListener<OnImageRequestEvent>(OnImageRequestEventListener);
        }
    }

    IEnumerator GetText(string url, Env.APIResponseType type)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            data = JSON.Parse(www.downloadHandler.text);
            switch (type)
            {
                case Env.APIResponseType.POKEDEX_PAGE:
                    EventManager.Instance.Trigger(new OnAPIResponseEvent
                    {
                        json = data,
                        responseType = Env.APIResponseType.POKEDEX_PAGE
                    });
                    break;
                case Env.APIResponseType.POKEMON:
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator GetImage(string url, RawImage image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            image.texture = DownloadHandlerTexture.GetContent(www);
        }
    }

    private void OnAPIRequestEventListener(OnAPIRequestEvent e)
    {
        StartCoroutine(GetText(e.url,e.type));
    }

    private void OnImageRequestEventListener(OnImageRequestEvent e)
    {
        StartCoroutine(GetImage(e.url, e.image));
    }
}