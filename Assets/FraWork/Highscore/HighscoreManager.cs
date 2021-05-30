using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace FraWork.Highscore
{
    /// <summary>
    /// Scriptable object that can be created from the asset menu in order to make a customizable highscore table.
    /// </summary>
    [CreateAssetMenu(menuName = "FraWork/Highscore/HighscoreManager", fileName = "newHighscoreManager")]
    public class HighscoreManager : ScriptableObject
    {
        public List<HighscoreStat> statTitles = new List<HighscoreStat>();

        public Color backgroundColor = new Color32(0, 0, 0, 255);
        public Color titlesColor = new Color32(20, 20, 20, 255);
        public Color titlesFontColor = new Color32(200, 200, 200, 255);
        public Color firstRowColor = new Color32(180, 180, 180, 255);
        public Color firstRowFontColor = new Color32(0, 0, 0, 255);
        public Color secondRowColor = new Color32(100, 100, 100, 255);
        public Color secondRowFontColor = new Color32(0, 0, 0, 255);


        private List<HighscoreItem> items;

        private void Awake()
        {
            if (statTitles.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    HighscoreStat stat = new HighscoreStat(i+1, $"Stat Title {i + 1}");
                    statTitles.Add(stat);
                }
            }
        }

        private void OnEnable()
        {
            if (statTitles.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    HighscoreStat stat = new HighscoreStat(i+1, $"Stat Title {i + 1}");
                    statTitles.Add(stat);
                }
            }
        }

        /// <summary>
        /// Function that adds the list of stat items to the table.
        /// See also <see cref="HighscoreTable.AddItemsToList(List{HighscoreItem})"/> for more details.
        /// </summary>
        public void AddToTable(List<HighscoreItem> _list)
        {
            items.AddRange(_list);
            HighscoreTable.AddItemsToList(_list);
        }

        /// <summary>
        /// Call the highscore table function that updates the highscore table.
        /// See also <see cref="HighscoreTable.UpdateTable(HighscoreManager, List{HighscoreItem})"/> for more details.
        /// </summary>
        public void UpdateTable()
        {
            HighscoreTable.UpdateTable(this, items);
        }

        /// <summary>
        /// Call the highscore table function that adds the list of stat items to the table.
        /// See also <see cref="HighscoreTable.AddItemsToList(List{HighscoreItem})"/> for more details.
        /// </summary>
        public void UpdateTableBackground()
        {
            HighscoreTable.UpdateTableBackground(this);
        }

        /// <summary>
        /// Call the highscore table function that updates the stat titles text labels.
        /// See also <see cref="HighscoreTable.UpdateStatTitles(HighscoreManager)"/> for more details.
        /// </summary>
        public void UpdateStatTitles()
        {
            HighscoreTable.UpdateStatTitles(this);
        }

        /// <summary>
        /// Call the highscore table function that updates the stat titles color.
        /// See also <see cref="HighscoreTable.UpdateStatTitlesColors(HighscoreManager)"/> for more details.
        /// </summary>
        public void UpdateStatTitlesColors()
        {
            HighscoreTable.UpdateStatTitlesColors(this);
        }

        /// <summary>
        /// Call the highscore table function that updates the stat columns to show.
        /// See also <see cref="HighscoreTable.UpdateVisibleColumns(HighscoreManager)"/> for more details.
        /// </summary>
        public void UpdateVisibleColumns()
        {
            HighscoreTable.UpdateVisibleColumns(this);
        }

        /// <summary>
        /// Call the highscore table function that updates the stat items in the table.
        /// See also <see cref="HighscoreTable.UpdateStatItems(HighscoreManager, List{HighscoreItem}))"/> for more details.
        /// </summary>
        public void UpdateStatItems()
        {
            HighscoreTable.UpdateStatItems(this, items);
        }
    }
}
