using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private Button pokedexButton;
    [SerializeField]
    private GameObject pokedexContainer;
    [SerializeField]
    private TextMeshProUGUI capturedText;

    private void Start()
    {
        EventManager.Instance.AddListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);
        EventManager.Instance.AddListener<OnPokemonHittedEvent>(OnPokemonHittedEventListener);

        pokedexButton.onClick.AddListener(() =>
        {
            pokedexContainer.SetActive(true);
            pokedexButton.gameObject.SetActive(false);
            EventManager.Instance.Trigger(new OnAPIRequestEvent
            {
                url = "https://pokeapi.co/api/v2/pokemon/?limit=10/",
                type = Env.APIResponseType.POKEDEX_PAGE
            });

            EventManager.Instance.Trigger(new OnPokedexOpenEvent());
        });
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);
            EventManager.Instance.RemoveListener<OnPokemonHittedEvent>(OnPokemonHittedEventListener);
        }
    }

    private void OnPokedexClosedEventListener(OnPokedexClosedEvent e)
    {
        pokedexContainer.SetActive(false);
        pokedexButton.gameObject.SetActive(true);
    }

    private void OnPokemonHittedEventListener(OnPokemonHittedEvent e)
    {
        var nameSplitted = e.pokemonHitted.name.Split('_');
        var name = nameSplitted[1].Split('(');
        if (e.wasCaptured)
        {
            capturedText.text = name[0] + " captured!";
        }
        else
        {
            capturedText.text = name[0] + " escaped!";
        }

        capturedText.transform.parent.gameObject.SetActive(true);
        StartCoroutine(TextCounter());
    }

    IEnumerator TextCounter()
    {
        yield return new WaitForSeconds(2f);
        capturedText.transform.parent.gameObject.SetActive(false);
        EventManager.Instance.Trigger(new OnTextCounterDone());
    }
}
