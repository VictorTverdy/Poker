using UnityEngine;
using Hexagonal;
public class LoadManager : MonoBehaviour {

    public static LoadManager Instance;

    public LoadData SaveInfo;
    public bool needLoad;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            GetLoadInfo();
        }
        else
            DestroyImmediate(this);
    }

    void OnDisable()
    {
       SaveGame();
    }

    public void ClearSave()
    {
        needLoad = false;
        SaveInfo = new LoadData();
        PlayerPrefs.SetString("load", "");
    }

    public void SaveGame()
    {
        if (HexagonalGrid.Instance == null)
            return;
        needLoad = true;
        HexagonalCell[] cells =  HexagonalGrid.Instance.GetAllCells();
        int lenght = cells.Length;
        SaveInfo.Cards = new Cards.CardData[lenght]; 
        for (int i=0;i< lenght; i++)
        {
            SaveInfo.Cards[i] = cells[i].GetCard().Info;
        }

        HudUI score = FindObjectOfType<HudUI>();
        SaveInfo.Score = score.GetScore();
        SaveInfo.Turn = score.GetTurn();
        SaveInfo.untilClosed = ManagerClosedCard.Instance.currentProgress;

        string obj = JsonUtility.ToJson(SaveInfo);
        PlayerPrefs.SetString("load", obj);
    }

    private void GetLoadInfo()
    {
        string fromJson = PlayerPrefs.GetString("load");
        if (string.IsNullOrEmpty(fromJson))
        {
            needLoad = false;
        }
        else
        {
            needLoad = true;
            SaveInfo = JsonUtility.FromJson<LoadData>(fromJson);
        }       
    }

}
