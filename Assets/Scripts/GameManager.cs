using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GameManager : MonoBehaviour
{
    public GameObject ground;
    public PoolManager poolManager;
    public PlaneFinderBehaviour planeFinder;

    private ContentPositioningBehaviour contentPositioningBehaviour = null;
    private bool isPokemonSpawned;
    public List<string> capturedPokemon = new List<string>();
    public GameObject instatiatedPokemon = null;

    private void Start()
    {
        EventManager.Instance.AddListener<OnPokemonHittedEvent>(OnPokemonHittedEventListener);
        EventManager.Instance.AddListener<OnTextCounterDone>(OnTextCounterDoneListener);
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnPokemonHittedEvent>(OnPokemonHittedEventListener);
            EventManager.Instance.RemoveListener<OnTextCounterDone>(OnTextCounterDoneListener);
        }
    }

    private void spawnPokemon(HitTestResult result)
    {
        if (isPokemonSpawned)
        {
            return;
        }


        if (VuforiaManager.Instance.ARCameraTransform != null)
        {
            this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
            contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            instatiatedPokemon = poolManager.GetObject(Env.POKEMON_PATH, true, true);
            instatiatedPokemon.transform.SetParent(ground.transform);
            instatiatedPokemon.transform.localPosition = Vector3.zero;
            var lookAtPosition = VuforiaManager.Instance.ARCameraTransform.position - instatiatedPokemon.transform.position;
            lookAtPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookAtPosition);
            instatiatedPokemon.transform.rotation = rotation;
            isPokemonSpawned = true;
        }
    }

    private void OnPokemonHittedEventListener(OnPokemonHittedEvent e)
    {
        if (e.wasCaptured)
        {
            capturedPokemon.Add(e.pokemonHitted.name);
        }
        poolManager.ReleaseObject(Env.POKEMON_PATH, e.pokemonHitted, !e.wasCaptured);
    }

    private void OnTextCounterDoneListener(OnTextCounterDone e)
    {
        isPokemonSpawned = false;
    }
}
