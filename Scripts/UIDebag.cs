
using UnityEngine;
using UnityEngine.UI;
public class UIDebag : MonoBehaviour {
    public static UIDebag Instance;
    public Text textDebage;
    // Use this for initialization

    void Start () {
        Instance = this;
    }

}
