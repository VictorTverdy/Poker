using UnityEngine;
using Hexagonal;

public class FlashingAnimation : MonoBehaviour {

    private HexagonalCell myCell;

	// Use this for initialization
	void Awake ()
    {
        myCell = transform.GetComponentInParent<HexagonalCell>();
    }

    public void ChangeColor()
    {
        myCell.ChangeColor();
    }
}
