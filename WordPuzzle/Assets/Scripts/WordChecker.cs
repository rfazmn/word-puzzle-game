using System.Collections.Generic;

public class WordChecker : Singleton<WordChecker>
{
    public List<Word> wordList;
    public int wordIndex;
    bool completed;

    void Start()
    {
        if (wordList.Count == 0)
            return;

        wordIndex = 0;
        WordUIHandler.Instance.ShowNextQuestion(wordIndex + 1, wordList[wordIndex]);
    }

    public void CheckWord(SinglyLinkedList charLinkedList)
    {
        string word = "";

        List<Cell> charList = new List<Cell>();
        while (charLinkedList.root != null)
        {
            charList.Add(charLinkedList.root.cell);
            charLinkedList.root = charLinkedList.root.prev;
        }

        charList.Reverse();

        for (int i = 0; i < charList.Count; i++)
        {
            word += charList[i].character;
            charList[i].SetSelected(false);

        }

        bool found = word == wordList[wordIndex].answer;

        if (!found)
        {
            for (int i = 0; i < charList.Count; i++)
            {
                charList[i].Tickle();
            }

            TouchHandler.Instance.ResetLine();
        }
        else
        {
            wordIndex++;
            if (wordIndex < wordList.Count)
                WordUIHandler.Instance.ShowNextQuestion(wordIndex + 1, wordList[wordIndex]);
            else
            {
                WordUIHandler.Instance.SetQuestionText("");
                completed = true;
            }
        }

        TouchHandler.Instance.SetProcessTouch(true);
    }

    public bool GetCompleted()
    {
        return completed;
    }
}
