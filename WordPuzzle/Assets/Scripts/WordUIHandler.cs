using TMPro;
using UnityEngine;

public class WordUIHandler : Singleton<WordUIHandler>
{
    [SerializeField] TMP_Text questionText;

    public void ShowNextQuestion(int index, Word word)
    {
        string text = $"<color=yellow><u>{word.answer.Length} Character</u></color>\n{index}. {word.question}";
        SetQuestionText(text);
    }

    public void SetQuestionText(string value)
    {
        questionText.text = value;
    }
}
