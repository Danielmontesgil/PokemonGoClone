using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateAccountController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField username;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private TMP_InputField confirmPassword;
    [SerializeField]
    private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(()=> {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = ""
            });
            CheckFields();
        });
    }

    private void CheckFields()
    {
        if(username.text == "" || password.text == "" || confirmPassword.text == "")
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = "All the fields are required"
            });
            return;
        }

        if (password.text.Length <= 6)
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = "Password hould have at least 6 characters"
            });
            return;
        }

        if(password.text != confirmPassword.text)
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = "Password does not match"
            });
            return;
        }

        PlayFabManager.Instance.RegisterPlayFabUser(username.text, password.text,()=>
        {
            LoadSceneManager.Instance.LoadScene(Env.GAME_SCENE);
        },(messageError)=>
        {
            EventManager.Instance.Trigger(new OnAccountInfoErrorEvent
            {
                message = messageError
            });
        });
    }
}
