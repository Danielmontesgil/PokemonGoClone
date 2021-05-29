using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogInController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(() => {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = ""
            });
            CheckFields();
        });
    }

    private void CheckFields()
    {
        if (username.text == "" || password.text == "")
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = "All the fields are required"
            });
            return;
        }

        PlayFabManager.Instance.LoginWithPlayFab(username.text, password.text, () =>
        {
            LoadSceneManager.Instance.LoadScene(Env.GAME_SCENE);
        }, (messageError) =>
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = messageError
            });
        });
    }
}
