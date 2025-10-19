using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    public Slider scrollSpeed;
    public Toggle measureLine;
    public Toggle metronome;
    private PlayerControls playerControls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.UI.Enable();
        playerControls.UI.Cancel.performed += ExitMenu;
    }

    private void OnDestroy()
    {
        playerControls.UI.Cancel.performed -= ExitMenu;
    }

    private void ExitMenu(InputAction.CallbackContext context)
    {
        ExitMenu(); // just call the parameterless version
    }

    public void ExitMenu()
    {
        Debug.Log("Closing Menu");
        playerControls.Player.Disable();
        transform.parent.gameObject.SetActive(false);
    }

    public void RefreshSettings()
    {
        PlayerData.scrollSpeed = scrollSpeed.value / 100;
        PlayerData.measureLine = measureLine.isOn;
        PlayerData.metronome = metronome.isOn;
    }
}
