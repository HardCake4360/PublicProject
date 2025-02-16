using UnityEngine;
using UnityEngine.UIElements;

public class UIdoc : MonoBehaviour
{
    [Header("Refference Gameobject")]
    [SerializeField] PlayerScript _playerScript;

    private Button hpUp;
    private Button hpDown;

    private ProgressBar hpBar;
    private ProgressBar spBar;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        hpUp = uiDocument.rootVisualElement.Q("hpUp") as Button;
        hpDown = uiDocument.rootVisualElement.Q("hpDown") as Button;

        hpBar = uiDocument.rootVisualElement.Q("hpSlider") as ProgressBar;
        spBar = uiDocument.rootVisualElement.Q("spSlider") as ProgressBar;

        hpUp.RegisterCallback<ClickEvent>(onClickUp);
        hpDown.RegisterCallback<ClickEvent>(onClickDown);

        
    }

    private void OnDisable()
    {
        hpUp.UnregisterCallback<ClickEvent>(onClickUp);
        hpDown.UnregisterCallback<ClickEvent>(onClickDown);
    }
    private void Update()
    {
        hpBar.value = _playerScript.Hp;
    }

    private void onClickUp(ClickEvent evt)
    {
        _playerScript.HpSum(10);
    }
    private void onClickDown(ClickEvent evt)
    {
        _playerScript.HpSum(-10);
    }
}
