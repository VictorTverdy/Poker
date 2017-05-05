using UnityEngine;
using UnityEngine.UI;
public class EditorDev : MonoBehaviour
{
    public Button EditorButton;
    public Toggle DevModeToggle;
    public GameObject EditorPanel;
    public InputField startTurn,closed,bonus;

    public InputField collectCombinationPare;
    public InputField collectCombinationTwoPare;
    public InputField collectCombinationSet;
    public InputField collectCombinationStr;
    public InputField collectCombinationFlash;
    public InputField collectCombinationHouse;
    public InputField collectCombinationQuadro;
    public InputField collectCombinationStrFlash;
    public InputField collectCombinationRoyal;

    public InputField addPare;
    public InputField addTwoPare;
    public InputField addSet;
    public InputField addStr;
    public InputField addFlash;
    public InputField addHouse;
    public InputField addQuadro;
    public InputField addStrFlash; 
    public InputField addRoyal;

    void Start()
    {
        EditorButton.onClick.AddListener(OpenEditorPanel);
        DevModeToggle.onValueChanged.AddListener(EditorMode);
        startTurn.onEndEdit.AddListener(SetStartTurn);
        startTurn.placeholder.GetComponent<Text>().text = GlobalInfo.startTurn.ToString();
        closed.onEndEdit.AddListener(SetClosed);
        closed.placeholder.GetComponent<Text>().text = GlobalInfo.periodicityCreateClosed.ToString();
        bonus.onEndEdit.AddListener(SetBonus);
        bonus.placeholder.GetComponent<Text>().text = GlobalInfo.changeDropBonus.ToString();

        collectCombinationPare.onEndEdit.AddListener(SetCollectCombinationPare);
        collectCombinationPare.placeholder.GetComponent<Text>().text = GlobalInfo.collectPare.ToString();
        collectCombinationTwoPare.onEndEdit.AddListener(SetCollectCombinationTwoPare);
        collectCombinationTwoPare.placeholder.GetComponent<Text>().text = GlobalInfo.collectTwoPares.ToString();
        collectCombinationSet.onEndEdit.AddListener(SetCollectCombinationSet);
        collectCombinationSet.placeholder.GetComponent<Text>().text = GlobalInfo.collectSet.ToString();
        collectCombinationStr.onEndEdit.AddListener(SetCollectCombinationStr);
        collectCombinationStr.placeholder.GetComponent<Text>().text = GlobalInfo.collectStraight.ToString();
        collectCombinationFlash.onEndEdit.AddListener(SetCollectCombinationFlash);
        collectCombinationFlash.placeholder.GetComponent<Text>().text = GlobalInfo.collectFlash.ToString();
        collectCombinationHouse.onEndEdit.AddListener(SetCollectCombinationStrHouse);
        collectCombinationHouse.placeholder.GetComponent<Text>().text = GlobalInfo.collectHouse.ToString();
        collectCombinationQuadro.onEndEdit.AddListener(SetCollectCombinationQuadro);
        collectCombinationQuadro.placeholder.GetComponent<Text>().text = GlobalInfo.collectQuadro.ToString();
        collectCombinationStrFlash.onEndEdit.AddListener(SetCollectCombinationStrFlash);
        collectCombinationStrFlash.placeholder.GetComponent<Text>().text = GlobalInfo.collectStrFlash.ToString();
        collectCombinationRoyal.onEndEdit.AddListener(SetCollectCombinationRoyal);
        collectCombinationRoyal.placeholder.GetComponent<Text>().text = GlobalInfo.collectRoyal.ToString();

        addPare.onEndEdit.AddListener(SetAddPare);
        addPare.placeholder.GetComponent<Text>().text = GlobalInfo.turnPare.ToString();
        addTwoPare.onEndEdit.AddListener(SetAddTwoPare);
        addTwoPare.placeholder.GetComponent<Text>().text = GlobalInfo.turnTwoPares.ToString();
        addSet.onEndEdit.AddListener(SetAddSet);
        addSet.placeholder.GetComponent<Text>().text = GlobalInfo.turnSet.ToString();
        addStr.onEndEdit.AddListener(SetAddStr);
        addStr.placeholder.GetComponent<Text>().text = GlobalInfo.turnStraight.ToString();
        addHouse.onEndEdit.AddListener(SetAddHouse);
        addHouse.placeholder.GetComponent<Text>().text = GlobalInfo.turnHouse.ToString();
        addFlash.onEndEdit.AddListener(SetAddFlash);
        addFlash.placeholder.GetComponent<Text>().text = GlobalInfo.turnFlash.ToString();
        addQuadro.onEndEdit.AddListener(SetQuadro);
        addQuadro.placeholder.GetComponent<Text>().text = GlobalInfo.turnQuadro.ToString();
        addStrFlash.onEndEdit.AddListener(SetStrFlash);
        addStrFlash.placeholder.GetComponent<Text>().text = GlobalInfo.turnStrFlash.ToString();
        addRoyal.onEndEdit.AddListener(SetAddRoyal);
        addRoyal.placeholder.GetComponent<Text>().text = GlobalInfo.turnRoyal.ToString();
    }

    private void SetAddRoyal(string value)
    {
        if(!string.IsNullOrEmpty(value))
          GlobalInfo.turnRoyal = int.Parse(value);
    }

    private void SetStrFlash(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnStrFlash = int.Parse(value);
    }

    private void SetQuadro(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnQuadro = int.Parse(value);
    }

    private void SetAddHouse(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnHouse = int.Parse(value);
    }

    private void SetAddFlash(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnFlash = int.Parse(value);
    }

    private void SetAddStr(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnStraight = int.Parse(value);
    }

    private void SetAddSet(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnSet = int.Parse(value);
    }

    private void SetAddTwoPare(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnTwoPares = int.Parse(value);
    }

    private void SetAddPare(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.turnPare = int.Parse(value);
    }

    private void SetCollectCombinationRoyal(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectRoyal = int.Parse(value);
    }

    private void SetCollectCombinationStrFlash(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectStrFlash = int.Parse(value);
    }

    private void SetCollectCombinationQuadro(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectQuadro = int.Parse(value);
    }

    private void SetCollectCombinationStrHouse(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectHouse = int.Parse(value);
    }

    private void SetCollectCombinationFlash(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectFlash = int.Parse(value);
    }

    private void SetCollectCombinationStr(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectStraight = int.Parse(value);
    }

    private void SetCollectCombinationPare(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectPare = int.Parse(value);
    }

    private void SetCollectCombinationTwoPare(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectTwoPares = int.Parse(value);
    }

    private void SetCollectCombinationSet(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.collectSet = int.Parse(value);
    }

    private void SetStartTurn(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.startTurn = int.Parse(value) +1;
    }

    private void SetClosed(string value)
    {
        if (!string.IsNullOrEmpty(value))
            GlobalInfo.periodicityCreateClosed = int.Parse(value);
    }

    private void SetBonus(string value)
    {
        if (!string.IsNullOrEmpty(value)) 
            GlobalInfo.changeDropBonus = int.Parse(value);
    }

    private void OpenEditorPanel()
    {
        if (EditorPanel.activeInHierarchy)
            EditorPanel.SetActive(false);
        else
            EditorPanel.SetActive(true);
    }

    private void EditorMode(bool _value)
    {
        GlobalInfo.devMode = _value;
        EditorButton.gameObject.SetActive(_value);
    }
	
	
}
