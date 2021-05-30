using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using FraWork.Highscore;

public class HighscoreTest : MonoBehaviour
{
    [SerializeField] private HighscoreManager highscoreManager;

    // Start is called before the first frame update
    void Start()
    {
        HighscoreTable.Initialise();

        List<HighscoreItem> his = new List<HighscoreItem>();
        System.Random gen = new System.Random();
        for (int i=0; i<10; i++)
        {          
            DateTime start = new DateTime(2000, 1, 1);
            int range = (DateTime.Today - start).Days;  
            DateTime randomDate = DateTime.Today.AddDays(-gen.Next(range));

            HighscoreItem hi = new HighscoreItem
            {
                stat1 = "Stat1" + i.ToString(),
                stat2 = UnityEngine.Random.Range(0, 100).ToString(),
                stat3 = UnityEngine.Random.Range(0f, 100f).ToString(),
                stat4 = randomDate.ToString("d"),
                stat5 = "Stat5" + i.ToString(),
            };
            his.Add(hi);
        }

        highscoreManager.AddToTable(his);
        HighscoreTable.UpdateTable(highscoreManager, his);
    }
}
