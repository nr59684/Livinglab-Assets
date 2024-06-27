using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

//Script f�r das Optionsmenu. Bietet funktionen f�r die verschiedenen Einstellungen
public class OptionsMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;       //Verkn�pfung zum AudioMixer, dieser Verwaltet alle Sounds
    public Slider volumeSlider;         //Slider f�r die Lautst�rke

    public Dropdown resolutionDropdown; //Dropdown f�r die verschiedenen Aufl�sungen
    private Resolution[] resolutions;   //Liste der Aufl�sungen

    // Awake is called before Start, L�d die verschiedenen Anfangswerte
    void Awake()
    {

        float currentValue;                                     //VolumeSlider wird auf ausgansposition gebracht
        audioMixer.GetFloat("Background", out currentValue);
        volumeSlider.value = currentValue;

        resolutions = Screen.resolutions;                       //m�gliche Resolutions werden geladen
        resolutionDropdown.ClearOptions();                      //resolutionDropdown wird geleert...

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)            //...und wieder mit den aktuellen M�glichkeiten gef�llt
        {

            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz"; //So wird die Aufl�sung angezeigt
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

   
    public void SetVolumeBackground(float volume)   //Hintergrundlautst�rke wird angepasst
    {

        audioMixer.SetFloat("Background", volume);
    }

    public void SetVolumeFX(float volume)           //Effektlautst�rke wird angepasst
    {

        audioMixer.SetFloat("FX", volume);
    }

    public void SetVolumeSpeech(float volume)       //Sprachlautst�rke wird angepasst (aktuell nicht verf�gbar)
    {

        audioMixer.SetFloat("Speech", volume);
    }

    public void SetFullscreen(bool isFullscreen)    //Vollbildmodus wird aktiviert/deaktiviert
    {

        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)  //Aufl�sung wird angepasst
    {

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
