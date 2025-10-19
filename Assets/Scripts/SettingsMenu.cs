using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider scrollSpeed;
    public Toggle measureLine;
    public Toggle metronome;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void RefreshSettings()
    {
        Debug.Log("Updating Settings" + (scrollSpeed.value / 100));
        PlayerData.scrollSpeed = scrollSpeed.value / 100;
        PlayerData.measureLine = measureLine.isOn;
        PlayerData.metronome = metronome.isOn;
    }
}
