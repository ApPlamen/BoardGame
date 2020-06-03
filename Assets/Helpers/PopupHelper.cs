
namespace UnityEngine
{
    public class PopupHelper: MonoBehaviour
    {
        
        public static void CreatePopup(Question question, Dice dice)
        {
            GRP_MultichoiceManager _multichoiceManager = (GRP_MultichoiceManager) FindObjectOfType(typeof(GRP_MultichoiceManager));
            
            _multichoiceManager.ResetButtons();
            _multichoiceManager.Initialize(question.Title, question.Description);
            _multichoiceManager.returnPosition = ReturnPosition.Last;
            _multichoiceManager.buttonDirection = ButtonDirection.Vertical;

            _multichoiceManager.buttonSeparator = 50;
            _multichoiceManager.SetReturn("Отказ",
                () =>
                {
                    CreateInfoPrompt("Пропусна въпрос!",
                        "Ти пропусна върпос! Съжалявам, но не можеш да хвърлиш зарчето.", "Разбрах", false, dice);
                });
            
            foreach (var answer in question.Answers)
            {
                _multichoiceManager.AddButton(answer.Description, () => { HandleAnswer(answer.IsRight, question.AdditionalInfo, dice); });
            }
            
            _multichoiceManager.Create();
        }
        
        private static void CreateInfoPrompt(string title, string text, string buttonTitle, bool result, Dice dice)
        {
            GRP_MultichoiceManager _multichoiceManager = (GRP_MultichoiceManager) FindObjectOfType(typeof(GRP_MultichoiceManager));
            
            _multichoiceManager.ResetButtons();
            _multichoiceManager.Initialize(title, text);
            _multichoiceManager.returnPosition = ReturnPosition.Last;
            _multichoiceManager.buttonDirection = ButtonDirection.Horizontal;

            _multichoiceManager.buttonSeparator = 50;
            _multichoiceManager.SetReturn(buttonTitle, () => dice.AnswerCallBack(result));

            _multichoiceManager.Create();
        }

        private static void HandleAnswer(bool isRight, string additionalInfo, Dice dice)
        {
            if (isRight)
            {
                CreateInfoPrompt("Браво!", "Браво! Ти даде верен отговор. Да видим какво ще покаже зарчето! \nИнтересен факт:\n " + additionalInfo, "Ура", true, dice);
            }
            else
            {
                CreateInfoPrompt("Съжалявам!", "Съжалявам, но ти даде грешен отговор. Пропускаш ход! \nИнтересен факт:\n " + additionalInfo, "Добре", false, dice);
            }
        }
        
        private static void HandleOk(string text)
        {
            Debug.Log(text);
        }
    }
}