using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FraWork.Highscore
{
    /// <summary>
    /// Enum that contains the different data type that you can use for sorting the stats
    /// </summary>
    public enum StatType
    {
        String,
        Int,
        Float,
        Date
    }

    /// <summary>
    /// Class that contains the data of a single stat in the table.
    /// </summary>
    [System.Serializable]
    public class HighscoreStat
    {
        public string statTitle;
        public bool isVisible;
        public int statID;
        public StatType statType;
        public bool isBeingSorted;

        /// <summary>
        /// Constructor of this Highscore stat class.
        /// </summary>
        /// <param name="_statID">ID of the stat.</param>
        /// <param name="_statTitle">Title of the stat.</param>
        public HighscoreStat(int _statID, string _statTitle)
        {
            statID = _statID;
            statTitle = _statTitle;
            isVisible = true;
            statType = StatType.String;
            isBeingSorted = false;
        }
    }
}
