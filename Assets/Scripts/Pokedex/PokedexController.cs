using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class PokedexItem
{
    public Button pokemonButton;
    public RawImage icon;
    public TextMeshProUGUI name;
    [HideInInspector]
    public string url;
    [HideInInspector]
    public int id;
}

public class PokedexController : MonoBehaviour
{
    [Header("Pokedex")]
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

    [Header("Pokemon Info")]
    [SerializeField]
    private GameObject pokemonInfoGO;
    [SerializeField]
    private TextMeshProUGUI pokemonName;
    [SerializeField]
    private RawImage pokemonImage;
    [SerializeField]
    private TextMeshProUGUI pokemonHeight;
    [SerializeField]
    private TextMeshProUGUI pokemonWeight;
    [SerializeField]
    private TextMeshProUGUI pokemonType;
    [SerializeField]
    private TextMeshProUGUI pokemonAbilities;
    [SerializeField]
    private Button backButton;


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

        backButton.onClick.AddListener(() =>
        {
            TogglePokemonInfo(false);
        });

        for (int i = 0; i < pokedexItemList.Count; i++)
        {
            int counter = i;
            pokedexItemList[i].pokemonButton.onClick.AddListener(()=>
            {
                pokemonName.text = pokedexItemList[counter].name.text;
                pokemonImage.texture = pokedexItemList[counter].icon.texture;
                EventManager.Instance.Trigger(new OnAPIRequestEvent
                {
                    url = pokedexItemList[counter].url,
                    type = Env.APIResponseType.POKEMON
                });
            });
        }
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

    private void TogglePokemonInfo(bool showPokemonInfo)
    {
        pokemonInfoGO.SetActive(showPokemonInfo);
        pokedexGO.SetActive(!showPokemonInfo);
    }

    private IEnumerator ShowPokedex(OnAPIResponseEvent e)
    {

        if (e.responseType == Env.APIResponseType.POKEDEX_PAGE)
        {
            prevPageButton.interactable = false;
            nextPageButton.interactable = false;

            TogglePokedex(false);

            nextPage = e.json["next"];
            prevPage = e.json["previous"];
            if (pokedexItemList.Count == e.json["results"].Count)
            {
                int counter = 0;
                foreach (var pokemon in e.json["results"])
                {
                    pokedexItemList[counter].name.text = pokemon.Value["name"];
                    pokedexItemList[counter].name.text = Utils.FirstCharToUpper(pokedexItemList[counter].name.text);
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

        if(e.responseType == Env.APIResponseType.POKEMON)
        {
            pokemonHeight.text = e.json["height"];
            pokemonWeight.text = e.json["weight"];

            pokemonType.text = "";
            pokemonAbilities.text = "";

            foreach (var type in e.json["types"])
            {
                if (string.IsNullOrEmpty(pokemonType.text))
                {
                    pokemonType.text += Utils.FirstCharToUpper(type.Value["type"]["name"].Value.ToString());
                }
                else
                {
                    pokemonType.text += "\n" + Utils.FirstCharToUpper(type.Value["type"]["name"].Value.ToString());
                }
            }

            foreach (var abilities in e.json["abilities"])
            {
                if (string.IsNullOrEmpty(pokemonAbilities.text))
                {
                    pokemonAbilities.text += Utils.FirstCharToUpper(abilities.Value["ability"]["name"].Value.ToString());
                }
                else
                {
                    pokemonAbilities.text += "\n" + Utils.FirstCharToUpper(abilities.Value["ability"]["name"].Value.ToString());
                }
            }
            
            TogglePokemonInfo(true);
        }
    }
}
