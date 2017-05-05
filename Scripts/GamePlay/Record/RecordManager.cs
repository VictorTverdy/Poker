using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class RecordManager  {

    public static void SaveRecord(int newScore)
    {
        if (newScore < 0)
            return;
        int maxRecord = 9;
        RecordInfo myInfo =new RecordInfo();
        List<RecordData> recordData = new List<RecordData>();
        string fromJson = PlayerPrefs.GetString("record");
        if (string.IsNullOrEmpty(fromJson))
        {
            RecordData item = new RecordData();
            item.score = newScore;
            recordData.Add(item);
        }
        else
        {
            myInfo = JsonUtility.FromJson<RecordInfo>(fromJson);
            recordData = myInfo.Info.ToList();
            if (recordData.Count < maxRecord || myInfo.Info == null)
            {

                RecordData item = new RecordData();
                item.score = newScore;
                recordData.Add(item);
            }
            else
            {
                for (int i = 0; i < recordData.Count; i++)
                {
                    if (i >= maxRecord)
                        break;
                    if (recordData[i].score < newScore)
                    {
                        RecordData item = new RecordData();
                        item.score = newScore;
                        recordData.RemoveAt(recordData.Count - 1);
                        recordData.Add(item);
                        break;
                    }
                }
            }
        } 
        myInfo.Info = new RecordData[recordData.Count];
        myInfo.Info = recordData.ToArray();
        myInfo.Info = myInfo.Info.OrderByDescending(x => x.score).ToArray();
        string toJson = JsonUtility.ToJson(myInfo);
        PlayerPrefs.SetString("record", toJson);
    }

    public static void ClearSaves()
    {
        PlayerPrefs.DeleteKey("record");
    }

    public static RecordInfo GetRecordInfo()
    {
        RecordInfo myInfo = new RecordInfo(); ;
        string fromJson = PlayerPrefs.GetString("record");
        if (!string.IsNullOrEmpty(fromJson))         
        myInfo = JsonUtility.FromJson<RecordInfo>(fromJson);
        return myInfo;
    }
}
