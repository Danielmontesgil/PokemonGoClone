using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Button createAccountBtn;
    [SerializeField]
    private GameObject createAccountContainer;
    [SerializeField]
    private Button logInButton;
    [SerializeField]
    private GameObject logInContainer;
    [SerializeField]
    private TextMeshProUGUI errorText;

    private Button clickedButton;
    private GameObject activeContainer;

    public delegate void SetErrorMessage(string message);

    private void Start()
    {
        EventManager.Instance.AddListener<OnAccountInfoErrorEvent>(OnAccountInfoErrorEventListener);

        ClickedButtonState(createAccountBtn);
        EnableContainer(createAccountContainer);

        createAccountBtn.onClick.AddListener(() =>
        {
            SwitchButton(createAccountBtn);
            SwitchContainers(createAccountContainer);
            CleanErrorText();
        });

        logInButton.onClick.AddListener(() =>
        {
            SwitchButton(logInButton);
            SwitchContainers(logInContainer);
            CleanErrorText();
        });
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnAccountInfoErrorEvent>(OnAccountInfoErrorEventListener);
        }
    }

    private void SwitchButton(Button clickedBtn)
    {
        NormalButtonState(clickedButton);
        ClickedButtonState(clickedBtn);
    }

    private void ClickedButtonState(Button clickedBtn)
    {
        this.clickedButton = clickedBtn;
        clickedButton.interactable = false;
    }

    private void NormalButtonState(Button releasedBtn)
    {
        releasedBtn.interactable = true;
    }

    private void SwitchContainers(GameObject container)
    {
        DisableContainer(this.activeContainer);
        EnableContainer(container);
    }

    private void EnableContainer(GameObject container)
    {
        container.SetActive(true);
        this.activeContainer = container;
    }

    private void DisableContainer(GameObject container)
    {
        container.SetActive(false);
    }

    private void CleanErrorText()
    {
        errorText.text = "";
    }

    private void OnAccountInfoErrorEventListener(OnAccountInfoErrorEvent e)
    {
        errorText.text = e.message;
    }
}
