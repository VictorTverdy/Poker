using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour {
    [SerializeField]
    private Button optionButton,exit;
    [SerializeField]
    private Toggle muteSound, muteAll;
    [SerializeField]
    private Slider sliderSound, sliderAll;
    [SerializeField]
    private GameObject optionPanel;

    // Use this for initialization
    void Start () {
        optionButton.onClick.AddListener(OpenOption);
        exit.onClick.AddListener(CloseOption);
        muteSound.onValueChanged.AddListener(MuteSound);
        muteAll.onValueChanged.AddListener(MuteAll);  
        sliderSound.onValueChanged.AddListener(SetVolumeBack);
        sliderAll.onValueChanged.AddListener(SetVolumeAll); 
        LoadPlayerPrefs();
    }

    private void SetVolumeBack(float volume)
    {
        SoundManager.Instance.SetVolumeBackSound(volume);
        PlayerPrefs.SetFloat("VolumeBack", volume);
    }

    private void SetVolumeAll(float volume)
    {
        SoundManager.Instance.SetVolumeAllSound(volume);
        PlayerPrefs.SetFloat("VolumeAll", volume);
    }

    private void MuteAll(bool isMute)
    {
        SoundManager.Instance.SetMuteAllSound(isMute);
        PlayerPrefs.SetString("MuteAll", isMute.ToString());
    }

    private void MuteSound(bool isMute)
    {
        SoundManager.Instance.SetMuteBackSound(isMute);
        PlayerPrefs.SetString("MuteSound", isMute.ToString());
    }

    private void LoadPlayerPrefs()
    {
        if (FirstEnter())
            return;

        sliderSound.value = PlayerPrefs.GetFloat("VolumeBack");
        sliderAll.value = PlayerPrefs.GetFloat("VolumeAll");
        if (PlayerPrefs.GetString("MuteSound") == "True")
        {
            muteSound.isOn = true;
        }
        else
        {
            muteSound.isOn = false;
        }
        if (PlayerPrefs.GetString("MuteAll") == "True")
        {
            muteAll.isOn = true;
        }
        else
        {
            muteAll.isOn = false;
        }
    }

    private bool FirstEnter()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("MuteSound")))
        {
            muteSound.isOn = false;
            muteAll.isOn = false;
            sliderSound.value = 1;
            sliderAll.value = 1;
            PlayerPrefs.SetString("MuteSound", muteSound.isOn.ToString());
            return true;
        }
        return false;
    }

    private void OpenOption()
    {
        SoundManager.Instance.PlayOpenOption();
        optionPanel.SetActive(true);
    }

    private void CloseOption()
    {
        optionPanel.SetActive(false);
    }
}
