using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LoginController : MonoBehaviour {

    private VisualElement LoginPanel;

    private VisualElement LoginButton;
    private VisualElement ExitButton;
    private TextField PasswordField;
    private TextField UsernameField;

    private void OnEnable() {
        LoginPanel = GetComponent<UIDocument>().rootVisualElement.Q("LoginPanel");
        InitUI();
    }

    void Start() {
        Core.NetworkClient.ChangeServer("127.0.0.1", 6900);
        Core.NetworkClient.HookPacket(AC.ACCEPT_LOGIN3.HEADER, this.OnLoginResponse);
    }

    private void InitUI() {
        LoginButton = LoginPanel.Q("LoginButton");
        ExitButton = LoginPanel.Q("ExitButton");
        PasswordField = LoginPanel.Q("PasswordField") as TextField;
        UsernameField = LoginPanel.Q("UsernameField") as TextField;

        LoginButton.RegisterCallback<ClickEvent>(OnLoginClicked);
        ExitButton.RegisterCallback<ClickEvent>(OnExitClicked);
    }

    void Update() {
        TabBehaviour();
    }

    private void TabBehaviour() {
        EventSystem currentEvent = EventSystem.current;

        if (currentEvent.currentSelectedGameObject == null || !Input.GetKeyDown(KeyCode.Tab))
            return;

        Selectable current = currentEvent.currentSelectedGameObject.GetComponent<Selectable>();
        if (current == null)
            return;
 
        bool up = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();
        next = current == next || next == null ? Selectable.allSelectablesArray[0] : next;
        currentEvent.SetSelectedGameObject(next.gameObject);
    }

    public void OnLoginClicked(ClickEvent evt) {
        var username = UsernameField.text;
        var password = PasswordField.text;

        if (username.Length == 0 || password.Length == 0) {
            return;
        }

        new CA.LOGIN(username, password, 10, 10).Send();
    }

    public void OnExitClicked(ClickEvent evt) {

    }

    private void OnLoginResponse(ushort cmd, int size, InPacket packet) {
        if (packet is AC.ACCEPT_LOGIN3) {
            var pkt = packet as AC.ACCEPT_LOGIN3;

            Core.NetworkClient.State.LoginInfo = pkt;
            SceneManager.LoadSceneAsync("CharServerSelectionScene");
        }
    }
}
