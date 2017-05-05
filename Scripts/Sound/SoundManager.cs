using UnityEngine;
using Cards;
public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    [SerializeField]
    private AudioSource backSound;
    [SerializeField]
    private AudioSource allSound;

    public AudioClip[] sounds; 

    // Use this for initialization
    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }else
        {
            DestroyImmediate(this);
        }
    }

    public void PlayBonusStart(eSpecialCard _bonus, bool isStart)
    {
        string path = "Audio/";
        string bonus = _bonus.ToString();
        if (isStart)
        {
            path += "Start/";
        }
        else
        {
            path += "Go/";
        }

        if (bonus.Contains("upend"))
        {
            bonus = "upend";
        }
        else if (bonus.Contains("wind"))
        {
            bonus = "wind";
        }
        else if(bonus.Contains("bullet"))
        {
            bonus = "bullet";
        }
        else if(bonus.Contains("fireworks"))
        {
            bonus = "fireworks";
        }
        AudioClip _clip = Resources.Load<AudioClip>(path + bonus);
        if (_clip != null)
        {
            allSound.PlayOneShot(_clip);
        }
    }
    public void PlayMoney()
    {
        allSound.PlayOneShot(sounds[9]);
    }
    public void PlayPauseOff()
    {
        allSound.PlayOneShot(sounds[8]);
    }
    public void PlayPauseOn()
    {
        allSound.PlayOneShot(sounds[7]);
    }


    public void PlayQuestion()
    {
        allSound.PlayOneShot(sounds[6]);
    }

    public void PlayGold()
    {
        allSound.PlayOneShot(sounds[5]);
    }

    public void PlayOpenOption() 
    {
        allSound.PlayOneShot(sounds[4]);
    }

    public void PlayDeleteCombo()
    {
        allSound.PlayOneShot(sounds[3]);
    }

    public void PlayEndGame()
    {
        allSound.PlayOneShot(sounds[2]);
    }

    public void PlayDealingCard()
    {
        allSound.PlayOneShot(sounds[1]);
    }

    public void PlayMixStart()
    {
        allSound.PlayOneShot(sounds[0]);
    }
	
	public void SetMuteBackSound(bool isMute)
    {
        backSound.mute = isMute;
    }

    public void SetVolumeBackSound(float volume)
    {
        backSound.volume = volume;
    }

    public void SetMuteAllSound(bool isMute)
    {
        allSound.mute = isMute;
    }

    public void SetVolumeAllSound(float volume)
    {
        allSound.volume = volume;
    }
}
