using UnityEngine;

public class MobileInput : MonoBehaviour {

    public GameObject panelOption, panelHelp;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panelOption.activeInHierarchy && !panelHelp.activeInHierarchy)
                Exit();
            if (panelOption.activeInHierarchy)
                panelOption.SetActive(false);
            if (panelHelp.activeInHierarchy)
                panelHelp.SetActive(false);          
        }
	}

    public void Exit()
    {
        Application.Quit();
    }
}
