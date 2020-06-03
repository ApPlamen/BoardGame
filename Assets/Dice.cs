using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour {

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int whosTurn = 1;
    private bool coroutineAllowed = true;
    private FileUtils _utils;
    private Question _question = new Question();

	// Use this for initialization
	void Start () {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        rend.sprite = diceSides[5];
		_utils = new FileUtils();;
	}

    //Detect if a click occurs
    public void OnMouseDown()
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        DialogWindow();
    }

    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        GameControl.diceSideThrown = randomDiceSide + 1;
        if (whosTurn == 1)
        {
            GameControl.MovePlayer(1);
        } else if (whosTurn == -1)
        {
            GameControl.MovePlayer(2);
        }
        whosTurn *= -1;
        coroutineAllowed = true;
    }

    private void DialogWindow()
    {
        _question = _utils.GetRandomQuestion();
		PopupHelper.CreatePopup(_question, this);
    }

    public void AnswerCallBack(bool result) { 
		if (result)
        {
            if (!GameControl.gameOver && coroutineAllowed)
                StartCoroutine("RollTheDice");
        }
        else
        {
            whosTurn *= -1;
        }
	}
}
