using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private Button pokedexButton;
    [SerializeField]
    private GameObject pokedexContainer;

    private void Start()
    {
        EventManager.Instance.AddListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);

        pokedexButton.onClick.AddListener(() =>
        {
            pokedexContainer.SetActive(true);
            pokedexButton.gameObject.SetActive(false);
            EventManager.Instance.Trigger(new OnAPIRequestEvent
            {
                url = "https://pokeapi.co/api/v2/pokemon/?limit=10/",
                type = Env.APIResponseType.POKEDEX_PAGE
            });
        });
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);
        }
    }

    private void OnPokedexClosedEventListener(OnPokedexClosedEvent e)
    {
        pokedexContainer.SetActive(false);
        pokedexButton.gameObject.SetActive(true);
    }
}
