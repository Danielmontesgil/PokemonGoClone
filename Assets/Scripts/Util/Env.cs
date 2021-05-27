using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Env
{
    ///Scenes
    public const string LOGIN_SCENE = "LogIn";
    public const string MENU_SCENE = "Menu";
    public const string GAME_SCENE = "Game";

    ///API response types
    public enum APIResponseType
    {
        POKEDEX_PAGE,
        POKEMON
    }
}
