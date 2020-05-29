using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour {

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int whosTurn = 1;
    private bool coroutineAllowed = true;
    private FileUtils _utils = new FileUtils();
    private Question _question = new Question();

	// Use this for initialization
	private void Start () {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        rend.sprite = diceSides[5];
	}
    void OnMouseDown()
    {
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
        string[] answers = new[] {"", "", "", "", "", "", "", "", ""};
        int i = 0;
        foreach (var answer in _question.Answers)
        {
            answers[i] = answer.Description;
            i++;
        }

        Popup.Create(_question.Title, _question.Description, 
            AnswerCallback, "Dialogue", answers[0], answers[1], answers[2], 
            answers[3], answers[4], answers[5], answers[6], answers[7], answers[8]);
    }
    
    void AnswerCallback(int answerIndex)
    {
        if (answerIndex == -1)
        {
            Popup.Create("Exited answer", "You have exited the question. Your turn is now automatically skipped!",
                OKButtonCallback, "PopUp", "Okay");
            whosTurn *= -1;
        }

        if (answerIndex != -1)
        {
            Answer answer = getAnswer(answerIndex);
        
            if (answer != null && answer.IsRight)
            {
                if (!GameControl.gameOver && coroutineAllowed)
                    // pop up questions 
                    StartCoroutine("RollTheDice");
            }
            else
            {
                Popup.Create("Wrong answer", "You have answered the question wrong. Your turn is now skipped!",
                    OKButtonCallback, "PopUp", "Okay");
                int playerNumber = whosTurn == 1 ? 1 : 2;
                GameControl.DisablePlayer(playerNumber);
                whosTurn *= -1;
            }
        }
    }

    private Answer getAnswer(int id)
    {
        Answer result = null;
        foreach (var answer in _question.Answers)
        {
            if (answer.Id == id)
            {
                result = answer;
                break;
            }
        }

        return result;
    }

    private void OKButtonCallback(int result) { }
}
