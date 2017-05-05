using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudUI : MonoBehaviour {

    [SerializeField]
    private Text textScore;
    [SerializeField]
    private Text textTurn;
    [SerializeField]
    private Text textStartClosed;
    [SerializeField]
    private Text textInfo;

    private int myScore;
    private int myTurn;
   

    private void Start()
    {
        myTurn = GlobalInfo.startTurn +1;      
        if (LoadManager.Instance.needLoad)
        {
            myTurn = LoadManager.Instance.SaveInfo.Turn + 1;
            textTurn.text = myTurn.ToString() + " turn";
            myScore = LoadManager.Instance.SaveInfo.Score;
            textScore.text = myScore.ToString() + " score";
        }
    } 

    public int GetScore()
    {
        return myScore;
    }

    public int GetTurn()
    {
        return myTurn;
    }   

    public void SetTextInfo(string value)
    {
        textInfo.text = value;
        StartCoroutine("StartTextInfo");
    }

    private IEnumerator StartTextInfo()
    {
        Color a = textInfo.color;
        Vector3 _scale = new Vector3(); 
        for (float i = 0.3f; i <= 1; i += 0.01f)
        {
            _scale.x = i;
            _scale.y = i;
            _scale.x = i;
            a.a = i;
            textInfo.color = a;
            textInfo.transform.localScale = _scale;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.3f);
        for (float i = 1; i >= 0; i -= 0.02f)
        {
            a.a = i;
            textInfo.color = a;
            yield return new WaitForSeconds(0.01f);
        }
        textInfo.text = "";
    } 

    public void SetTurn(int _turn)
    {
        myTurn = _turn;
        textTurn.text = myTurn.ToString() + " turn";
    }

    public void SetScore(int _score)
    {
        myScore = _score;
        textScore.text = string.Format("{0} score", myScore);
    }

    public void AddTurn(int _turn)
    {        
        myTurn += _turn;
        if(myTurn <= 0)
        {
            Lose();
        }
        textTurn.text = myTurn.ToString()+" turn"; 
    }

    public void SetTextClosedCard(int until)
    {
        textStartClosed.text = string.Format("{0} until closed", until);
    }

    public void Lose()
    {
        SoundManager.Instance.PlayEndGame();
        AppGame.Instance.Info.Gold += myScore;
        RecordManager.SaveRecord(myScore);
        FindObjectOfType<MenuGame>().panelLose.SetActive(true);
    }    

    public void AddScore(int _score)
    {
        StartCoroutine("StepsAddScores", _score);       
    }

    private IEnumerator StepsAddScores(int _score)
    {
        textScore.color = Color.yellow;
        int maxScore = myScore + _score;
        int step = 3;

        if (maxScore % 2 == 0)
            step = 2;
        for (int i = myScore; i <= maxScore; i += step)
        {
            myScore = i;
            textScore.text = string.Format("{0} score", myScore);
            yield return new WaitForSeconds(0.01f);
        }
        textScore.color = Color.white;
    }
}
