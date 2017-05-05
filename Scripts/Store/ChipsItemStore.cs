
using UnityEngine;
using UnityEngine.UI;
public class ChipsItemStore : MonoBehaviour {

    public Text myText;
	// Use this for initialization
	void Start () {
        gameObject.name = myText.text;
    }
	
}
