using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class PokedexItem
{
    public RawImage icon;
    public TextMeshProUGUI name;
    [HideInInspector]
    public string url;
    [HideInInspector]
    public int id;
}

public class PokedexController : MonoBehaviour
{
    [SerializeField]
    private GameObject pokedexGO;
    [SerializeField]
    private GameObject loadingText;
    [SerializeField]
    private Button closePokedexButton;
    [SerializeField]
    private Button prevPageButton;
    [SerializeField]
    private Button nextPageButton;
    [SerializeField]
    private List<PokedexItem> pokedexItemList = new List<PokedexItem>();

    private string nextPage;
    private string prevPage;

    void Start()
    {
        EventManager.Instance.AddListener<OnAPIResponseEvent>(OnAPIResponseEventListener);

        prevPageButton.onClick.AddListener(() =>
        {
            GetNewPokedexPage(prevPage);
        });

        nextPageButton.onClick.AddListener(() =>
        {
            GetNewPokedexPage(nextPage);
        });

        closePokedexButton.onClick.AddListener(() =>
        {
            TogglePokedex(false);
            EventManager.Instance.Trigger(new OnPokedexClosedEvent());
        });
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnAPIResponseEvent>(OnAPIResponseEventListener);
        }
    }

    private void OnAPIResponseEventListener(OnAPIResponseEvent e)
    {
        StartCoroutine(ShowPokedex(e));
    }

    private void GetNewPokedexPage(string url)
    {
        EventManager.Instance.Trigger(new OnAPIRequestEvent
        {
            url = url,
            type = Env.APIResponseType.POKEDEX_PAGE
        });
    }

    private void TogglePokedex(bool isPokedexEnable)
    {
        pokedexGO.SetActive(isPokedexEnable);
        loadingText.SetActive(!isPokedexEnable);
    }

    private IEnumerator ShowPokedex(OnAPIResponseEvent e)
    {
        TogglePokedex(false);

        prevPageButton.interactable = false;
        nextPageButton.interactable = false;

        if (e.responseType == Env.APIResponseType.POKEDEX_PAGE)
        {
            nextPage = e.json["next"];
            prevPage = e.json["previous"];
            if (pokedexItemList.Count == e.json["results"].Count)
            {
                int counter = 0;
                foreach (var pokemon in e.json["results"])
                {
                    pokedexItemList[counter].name.text = pokemon.Value["name"];
                    pokedexItemList[counter].name.text = char.ToUpper(pokedexItemList[counter].name.text[0]) + pokedexItemList[counter].name.text.Substring(1); //Bad performace but it saves time
                    pokedexItemList[counter].url = pokemon.Value["url"];

                    pokedexItemList[counter].id = int.Parse(pokedexItemList[counter].url.Split('/')[6]);

                    EventManager.Instance.Trigger(new OnImageRequestEvent
                    {
                        url = string.Format("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{0}.png", pokedexItemList[counter].id),
                        image = pokedexItemList[counter].icon
                    });

                    counter++;
                }
            }
            else
            {
                Debug.LogError("You are trying to put more items that possible in screen");
            }

            yield return new WaitForSeconds(0.5f);

            prevPageButton.interactable = !string.IsNullOrEmpty(prevPage);
            nextPageButton.interactable = !string.IsNullOrEmpty(nextPage);

            TogglePokedex(true);
        }
    }
}
