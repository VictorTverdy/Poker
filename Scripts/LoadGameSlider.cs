using UnityEngine;
using UnityEngine.UI;

public class LoadGameSlider : MonoBehaviour
{
    Slider mySlider;
    float _value;

	// Use this for initialization
	void Start ()
    {
        mySlider = GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        _value += Time.deltaTime;
        mySlider.value = _value;
        if (mySlider.value ==  mySlider.maxValue)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
