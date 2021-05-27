using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : Singleton<PlayFabManager>
{
    public void RegisterPlayFabUser(string username, string password, Action onSuccess, Action<string> onFailure)
    {
        var request = new RegisterPlayFabUserRequest
        {
            TitleId = PlayFabSettings.TitleId,
            Username = username,
            Password = password,
            DisplayName = username,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
                GetUserAccountInfo = true
            },
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
        {
            onSuccess?.Invoke();
        }, (error) =>
        {
            onFailure?.Invoke(error.ErrorMessage);
        });
    }
    public void LoginWithPlayFab(string username, string password, Action onSuccess, Action<string> onFailure)
    {
        var request = new LoginWithPlayFabRequest
        {
            TitleId = PlayFabSettings.TitleId,
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, (result) =>
        {
            onSuccess?.Invoke();
        }, (error) =>
         {
             onFailure?.Invoke(error.ErrorMessage);
         });
    }
}
