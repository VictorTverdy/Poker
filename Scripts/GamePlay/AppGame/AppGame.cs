using UnityEngine;

public class AppGame : MonoBehaviour
{
    public static AppGame Instance;

    public AppInfo Info;

	// Use this for initialization
	void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
        LoadInfo();
    }

    void OnDisable()
    {
        SaveInfo();
    }

    void LoadInfo()
    {
        string fromJson = PlayerPrefs.GetString("appGame");
        if (!string.IsNullOrEmpty(fromJson))
            Info = JsonUtility.FromJson<AppInfo>(fromJson);
    }

    void SaveInfo()
    {
      string toJson =  JsonUtility.ToJson(Info);
      PlayerPrefs.SetString("appGame",toJson);
    }


}
