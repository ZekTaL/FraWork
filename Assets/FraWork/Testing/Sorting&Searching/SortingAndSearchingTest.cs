using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FraWork.Utils.Extensions;
using UnityEngine.UI;
using System;

public class SortingAndSearchingTest : MonoBehaviour
{
    public GameObject sortingTab;
    public GameObject searchingTab;

    public Text unsortedListText;
    public Text sortedListText;

    public InputField searchInput;
    public InputField blockSizeInput;
    public Text searchingListText;
    public Text resultText;

    private int highscoreCount = 10;

    private List<Highscore> highscores = new List<Highscore>();

    // Start is called before the first frame update
    void Start()
    {
        OpenSortingTab();
    }

    public void OpenSortingTab()
    {
        if (sortingTab.activeInHierarchy)
            return;

        UseShuffle();

        sortingTab.SetActive(true);
        searchingTab.SetActive(false);
    }

    public void OpenSearchingTab()
    {
        if (searchingTab.activeInHierarchy)
            return;

        UseShuffle(true);

        searchingListText.text = GetHighscoresString(true);
        searchInput.text = "";
        blockSizeInput.text = "";
        resultText.text = "";

        sortingTab.SetActive(false);
        searchingTab.SetActive(true);
    }

    public void UseShuffle(bool _showIndexes = false)
    {
        highscores.Clear();

        for (int i = 0; i < highscoreCount; i++)
        {
            highscores.Add(new Highscore(UnityEngine.Random.Range(0, 1000), "Player" + i));
        }

        unsortedListText.text = GetHighscoresString();
        sortedListText.text = "";
    }

    private string GetHighscoresString(bool _showIndexes = false)
    {
        string formatted = "";
        int index = 0;

        foreach (Highscore highscore in highscores)
        {
            if (_showIndexes)
            {
                formatted += $"[{index++}] {highscore} \n";
            }
            else
                formatted += highscore.ToString() + "\n";
        }

        return formatted;
    }

    public void UseBubbleSort()
    {
        highscores.BubbleSort();
        sortedListText.text = GetHighscoresString();
    }

    public void UseMergeSort()
    {
        highscores.MergeSort();
        sortedListText.text = GetHighscoresString();
    }

    public void UseNaturalMergeSort()
    {
        highscores.NaturalMergeSort();
        sortedListText.text = GetHighscoresString();
    }

    public void UseLinearSearch()
    {
        bool success = int.TryParse(searchInput.text, out int valueToSearch);

        if (success)
        {
            int returnedIndex = highscores.LinearSearch(new Highscore(valueToSearch));
            resultText.text = returnedIndex == -1
                                ? "<b>Result:</b> Item not found!" 
                                : "<b>Result:</b> Item found at index " + returnedIndex.ToString();
        }
        else
        {
            resultText.text = "Input string is invalid!";
        }     
    }

    public void UseBinarySearch()
    {
        // Before searching I need to sort the list
        highscores.NaturalMergeSort();
        searchingListText.text = GetHighscoresString(true);

        bool success = int.TryParse(searchInput.text, out int valueToSearch);

        if (success)
        {
            int returnedIndex = highscores.BinarySearch(new Highscore(valueToSearch));
            resultText.text = returnedIndex == -1
                                ? "<b>Result:</b> Item not found!"
                                : "<b>Result:</b> Item found at index " + returnedIndex.ToString();
        }
        else
        {
            resultText.text = "Input string is invalid!";
        }
    }

    public void UseJumpSearch()
    {
        // Before searching I need to sort the list
        highscores.NaturalMergeSort();
        searchingListText.text = GetHighscoresString(true);

        int returnedIndex = -1;

        if (int.TryParse(searchInput.text, out int valueToSearch))
        {
            // check if the user put a custom blocksize
            if (String.IsNullOrEmpty(blockSizeInput.text))
            {
                returnedIndex = highscores.JumpSearch(new Highscore(valueToSearch));
            }
            else
            {
                if (int.TryParse(blockSizeInput.text, out int customBlockSize))
                {
                    Debug.Log(customBlockSize);
                    // check if the blocksize is bigger then the listCount
                    if (customBlockSize <= highscores.Count)
                    {
                        returnedIndex = highscores.JumpSearch(new Highscore(valueToSearch), customBlockSize);
                    }
                    else
                    {
                        resultText.text = "Custom BlockSize is bigger then the number of elements";
                        return;
                    }
                    
                }
                else
                {
                    Debug.Log("A");
                }
            }
            
            resultText.text = returnedIndex == -1
                                ? "<b>Result:</b> Item not found!"
                                : "<b>Result:</b> Item found at index " + returnedIndex.ToString();
        }
        else
        {
            resultText.text = "Input string is invalid!";
        }
    }
}
