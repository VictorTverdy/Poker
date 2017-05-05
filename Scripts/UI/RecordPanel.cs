using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecordPanel : MonoBehaviour
{
    public GameObject prefabItem;
    public Transform content;
    public Button buttonDelete;

    private List<GameObject> items = new List<GameObject>();   

    // Use this for initialization
    void Start()
    {
        buttonDelete.onClick.AddListener(Clear);

        var info = RecordManager.GetRecordInfo();
        if (info.Info == null)
            return;
        foreach (RecordData _record in info.Info) { 
            GameObject clone = Instantiate(prefabItem, content) as GameObject;
            items.Add(clone);
            clone.GetComponent<RecordItem>().TextScore.text = _record.score.ToString();
        }        
	}

    public void Clear()
    {
        for (int i=0;i< items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();
        RecordManager.ClearSaves();
    }

}
