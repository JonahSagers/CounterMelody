using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingText : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;
    public TextMeshProUGUI display;
    public void SliderText()
    {
        display.text = (slider.value / 100).ToString();
    }

    public void ToggleText()
    {
        if(toggle.isOn){
            display.text = "Enabled";
        } else {
            display.text = "Disabled";
        }
    }
}
