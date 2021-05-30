using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// Namespace that contains all the necessary classes for the implementation of an highscore table
/// </summary>
namespace FraWork.Highscore
{
    /// <summary>
    /// Enum that indicates is the data parsing was successful or had an error
    /// </summary>
    public enum SortingParseResult
    {
        NoErrors,
        IntParseFail,
        FloatParseFail,
        DateParseFail
    }

    /// <summary>
    /// Class that contains all the functionalities to initialise and update the highscore table
    /// </summary>
    public class HighscoreTable : MonoBehaviour
    {
        // list of all the texts objects in the highscore table
        private List<Text> tableTexts;
        // list of the GameObjects Stat Columns
        private List<GameObject> statColumns;
        // list of GameObjects stat titles
        private List<GameObject> statTitlesGO;
        // GameObject if no columns are visible
        private GameObject noVisibleColumns;
        // Gameobject Table
        private GameObject tableBackground;
        // list of the highscore items
        private List<HighscoreItem> highscoreItems = new List<HighscoreItem>();

        // Has the highscoreTable been initialised
        public static bool Initialised => instance != null;

        public HighscoreManager HighscoreManager { get; private set; }

        public static SortingParseResult parseResult = SortingParseResult.NoErrors;

        // Singleton reference instance
        private static HighscoreTable instance = null;

        private bool isAscending = false;

        /// <summary>
        /// If the system isn't already setup, this will instantiate the highscore table prefab and assign the static reference
        /// </summary>
        public static void Initialise()
        {
            // if the mobile input is already initialised, throw an exception to tell the user they dun goofed
            if (Initialised)
            {
                throw new System.InvalidOperationException("Highscore Table already initialised!");
            }

            // load the HighscoreTable prefab and instantiate it, setting the instance
            HighscoreTable prefabInstance = Resources.Load<HighscoreTable>("HighscoreTablePrefab");
            instance = Instantiate(prefabInstance);

            // changed the instantiated objects name and mark it to not be destroyed
            instance.gameObject.name = "HighscoreTable";
            DontDestroyOnLoad(instance.gameObject);

            instance.tableBackground = GameObject.FindGameObjectWithTag("Background");
            instance.tableTexts = instance.gameObject.GetComponentsInChildren<Text>(true).ToList();
            instance.statColumns = GameObject.FindGameObjectsWithTag("StatColumn").ToList();
            instance.statTitlesGO = GameObject.FindGameObjectsWithTag("StatTitle").ToList();
            instance.noVisibleColumns = GameObject.FindGameObjectWithTag("NoVisibleColumns");
            instance.noVisibleColumns.SetActive(false);
        }

        /// <summary>
        /// Function that keeps an updated list of the HighscoreItems on this class
        /// </summary>
        /// <param name="_items">Updated list of HighscoreItems</param>
        public static void AddItemsToList(List<HighscoreItem> _items)
        {
            instance.highscoreItems.AddRange(_items);
        }

        /// <summary>
        /// Function that returns the list of the highscore items in the table
        /// </summary>
        public static List<HighscoreItem> GetHighscoreItems() => instance.highscoreItems;

        /// <summary>
        /// Main method for updating the whole Highscore Table
        /// </summary>
        /// <param name="_highscoreManager">Reference to the HighscoreManager</param>
        /// <param name="_highscoreItems">Reference to the HighscoreItems list</param>
        public static void UpdateTable(HighscoreManager _highscoreManager, List<HighscoreItem> _highscoreItems)
        {
            if (!Initialised)
                return;

            // keep a reference of the highscore manager
            instance.HighscoreManager = _highscoreManager;

            // update background color
            UpdateTableBackground(_highscoreManager);

            // update stat titles background color
            UpdateStatTitlesColors(_highscoreManager);

            // update visible columns
            UpdateVisibleColumns(_highscoreManager);

            // update stat titles
            UpdateStatTitles(_highscoreManager);

            // update stat items
            UpdateStatItems(_highscoreManager, _highscoreItems);
        }

        /// <summary>
        /// Function that updates the background color of the highscore table
        /// </summary>
        /// <param name="_highscoreManager">Reference to the highscore scriptable object</param>
        public static void UpdateTableBackground(HighscoreManager _highscoreManager)
        {
            if (!Initialised)
                return;

            // change colors
            Image backgroundImage = instance.tableBackground.GetComponent<Image>();
            backgroundImage.color = _highscoreManager.backgroundColor;
        }

        /// <summary>
        /// Function that updates the background and font color of the highscore stat titles
        /// </summary>
        /// <param name="_highscoreManager">Reference to the highscore scriptable object</param>
        public static void UpdateStatTitlesColors(HighscoreManager _highscoreManager)
        {
            if (!Initialised)
                return;

            // change colors
            foreach (GameObject title in instance.statTitlesGO)
            {
                title.GetComponent<Image>().color = _highscoreManager.titlesColor;

                // I have the arrow and the title text
                foreach (Text titleText in title.GetComponentsInChildren<Text>())
                    titleText.color = _highscoreManager.titlesFontColor;
            }
        }

        /// <summary>
        /// Function that updates which stat columns are going to be visible in the highscore table
        /// </summary>
        /// <param name="_highscoreManager">Reference to the highscore scriptable object</param>
        public static void UpdateVisibleColumns(HighscoreManager _highscoreManager)
        {
            if (!Initialised)
                return;

            int visibleColumns = 0;
            for (int i = 0; i < 5; i++)
            {
                GameObject statColumn = instance.statColumns.FirstOrDefault(x => x.name == $"Stat{i + 1}");
                if (_highscoreManager.statTitles[i].isVisible)
                {
                    statColumn.SetActive(true);
                    visibleColumns++;
                }
                else
                {
                    statColumn.SetActive(false);
                    //continue;
                }
            }

            instance.noVisibleColumns.SetActive(visibleColumns == 0);
        }

        /// <summary>
        /// Function that updates the text labels of the stat titles
        /// </summary>
        /// <param name="_highscoreManager">Reference to the highscore scriptable object</param>
        public static void UpdateStatTitles(HighscoreManager _highscoreManager)
        {
            if (!Initialised)
                return;

            for (int i = 0; i < 5; i++)
            {
                string statTitleTextName = $"StatName{i + 1}";
                Text statTitleText = instance.tableTexts.FirstOrDefault(t => t.name == statTitleTextName);
                if (statTitleText == null)
                    continue;

                statTitleText.text = _highscoreManager.statTitles[i].statTitle;
            }
        }

        /// <summary>
        /// Function that updates all the stat items in the table, including text labels, background and font color
        /// </summary>
        /// <param name="_highscoreManager">Reference to the highscore scriptable object</param>
        /// <param name="_highscoreItems">Reference to the list of all stat items</param>
        public static void UpdateStatItems(HighscoreManager _highscoreManager, List<HighscoreItem> _highscoreItems)
        {
            if (!Initialised)
                return;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < _highscoreItems.Count; j++)
                {
                    string statTextName = $"Stat{i + 1}Item{j + 1}";

                    Text statText = instance.tableTexts.FirstOrDefault(t => t.transform.parent.name == statTextName);
                    if (statText == null)
                        continue;

                    switch (i)
                    {
                        case 0: statText.text = _highscoreItems[j].stat1; break;
                        case 1: statText.text = _highscoreItems[j].stat2; break;
                        case 2: statText.text = _highscoreItems[j].stat3; break;
                        case 3: statText.text = _highscoreItems[j].stat4; break;
                        case 4: statText.text = _highscoreItems[j].stat5; break;
                    }

                    // Alternate color rows
                    Image rowColor = statText.transform.parent.GetComponentInChildren<Image>();
                    rowColor.color = j % 2 == 0 ? _highscoreManager.firstRowColor : _highscoreManager.secondRowColor;
                    statText.color = j % 2 == 0 ? _highscoreManager.firstRowFontColor : _highscoreManager.secondRowFontColor;

                    statText.transform.parent.gameObject.SetActive(true);
                }
            }
        }

        #region StaticSortStats

        /// <summary>
        /// Static function for sorting stat1.
        /// See <see cref="SortStat1"/> for more details.
        /// </summary>
        public static void StaticSortStat1()
        {
            instance.isAscending = !instance.isAscending;
            instance.SortStat1();
        }

        /// <summary>
        /// Static helper function for sorting stat1.
        /// See <see cref="CheckStatType1"/> for more details.
        /// </summary>
        public static void StaticCheckStatType1()
        {
            instance.CheckStatType1();
        }

        /// <summary>
        /// Static function for sorting stat2.
        /// See <see cref="SortStat2"/> for more details.
        /// </summary>
        public static void StaticSortStat2()
        {
            instance.isAscending = !instance.isAscending;
            instance.SortStat2();
        }


        /// <summary>
        /// Static helper function for sorting stat2.
        /// See <see cref="CheckStatType2"/> for more details.
        /// </summary>
        public static void StaticCheckStatType2()
        {
            instance.CheckStatType2();
        }

        /// <summary>
        /// Static function for sorting stat3.
        /// See <see cref="SortStat3"/> for more details.
        /// </summary>
        public static void StaticSortStat3()
        {
            instance.isAscending = !instance.isAscending;
            instance.SortStat3();
        }


        /// <summary>
        /// Static helper function for sorting stat3.
        /// See <see cref="CheckStatType3"/> for more details.
        /// </summary>
        public static void StaticCheckStatType3()
        {
            instance.CheckStatType3();
        }

        /// <summary>
        /// Static function for sorting stat4.
        /// See <see cref="SortStat4"/> for more details.
        /// </summary>
        public static void StaticSortStat4()
        {
            instance.isAscending = !instance.isAscending;
            instance.SortStat4();
        }


        /// <summary>
        /// Static helper function for sorting stat4.
        /// See <see cref="CheckStatType4"/> for more details.
        /// </summary>
        public static void StaticCheckStatType4()
        {
            instance.CheckStatType4();
        }

        /// <summary>
        /// Static function for sorting stat5.
        /// See <see cref="SortStat5"/> for more details.
        /// </summary>
        public static void StaticSortStat5()
        {
            instance.isAscending = !instance.isAscending;
            instance.SortStat5();
        }


        /// <summary>
        /// Static helper function for sorting stat5.
        /// See <see cref="CheckStatTyp5"/> for more details.
        /// </summary>
        public static void StaticCheckStatType5()
        {
            instance.CheckStatType5();
        }

        #endregion

        #region SortStats

        /// <summary>
        /// Function for sorting the highscore table ordered by stat1.
        /// </summary>
        public void SortStat1()
        {
            CheckStatType1();

            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow1").text = isAscending ? "▲" : "▼";
            HighscoreManager.statTitles[0].isBeingSorted = true;

            // reset the other arrows
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow2").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow3").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow4").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow5").text = "";

            HighscoreManager.statTitles[1].isBeingSorted = false;
            HighscoreManager.statTitles[2].isBeingSorted = false;
            HighscoreManager.statTitles[3].isBeingSorted = false;
            HighscoreManager.statTitles[4].isBeingSorted = false;

            isAscending = !isAscending;
            UpdateTable(HighscoreManager, highscoreItems);
        }

        /// <summary>
        /// Helper function for sorting stat1 according to the selected stat type.
        /// </summary>
        public void CheckStatType1()
        {
            bool parseError = false;
            parseResult = SortingParseResult.NoErrors;

            switch (HighscoreManager.statTitles[0].statType)
            {
                case StatType.String:
                    highscoreItems = isAscending
                      ? highscoreItems.OrderBy(item => item.stat1).ToList()
                      : highscoreItems.OrderByDescending(item => item.stat1).ToList();
                    break;
                case StatType.Int:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !int.TryParse(item.stat1, out int result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.IntParseFail;
                        Debug.LogError("Int Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat1).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat1).ToList();
                    }
                    else
                    {
                        // order with parse int
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => int.Parse(item.stat1)).ToList()
                          : highscoreItems.OrderByDescending(item => int.Parse(item.stat1)).ToList();
                    }
                    break;

                case StatType.Float:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !float.TryParse(item.stat1, out float result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.FloatParseFail;
                        Debug.LogError("Float Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat1).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat1).ToList();
                    }
                    else
                    {
                        // order with parse float
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => float.Parse(item.stat1)).ToList()
                          : highscoreItems.OrderByDescending(item => float.Parse(item.stat1)).ToList();
                    }
                    break;

                case StatType.Date:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !DateTime.TryParse(item.stat1, out DateTime result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.DateParseFail;
                        Debug.LogError("DateTime Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat1).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat1).ToList();
                    }
                    else
                    {
                        // order with parse date
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => DateTime.Parse(item.stat1)).ToList()
                          : highscoreItems.OrderByDescending(item => DateTime.Parse(item.stat1)).ToList();
                    }
                    break;
            }
        }

        /// <summary>
        /// Function for sorting the highscore table ordered by stat2.
        /// </summary>
        public void SortStat2()
        {
            CheckStatType2();

            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow2").text = isAscending ? "▲" : "▼";
            HighscoreManager.statTitles[1].isBeingSorted = true;

            // reset the other arrows
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow1").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow3").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow4").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow5").text = "";

            HighscoreManager.statTitles[0].isBeingSorted = false;
            HighscoreManager.statTitles[2].isBeingSorted = false;
            HighscoreManager.statTitles[3].isBeingSorted = false;
            HighscoreManager.statTitles[4].isBeingSorted = false;

            isAscending = !isAscending;
            UpdateTable(HighscoreManager, highscoreItems);
        }

        /// <summary>
        /// Helper function for sorting stat2 according to the selected stat type.
        /// </summary>
        public void CheckStatType2()
        {
            bool parseError = false;
            parseResult = SortingParseResult.NoErrors;

            switch (HighscoreManager.statTitles[1].statType)
            {
                case StatType.String:
                    highscoreItems = isAscending
                      ? highscoreItems.OrderBy(item => item.stat2).ToList()
                      : highscoreItems.OrderByDescending(item => item.stat2).ToList();
                    break;
                case StatType.Int:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !int.TryParse(item.stat2, out int result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.IntParseFail;
                        Debug.LogError("Int Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat2).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat2).ToList();
                    }
                    else
                    {
                        // order with parse int
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => int.Parse(item.stat2)).ToList()
                          : highscoreItems.OrderByDescending(item => int.Parse(item.stat2)).ToList();
                    }
                    break;

                case StatType.Float:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !float.TryParse(item.stat2, out float result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.FloatParseFail;
                        Debug.LogError("Float Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat2).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat2).ToList();
                    }
                    else
                    {
                        // order with parse float
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => float.Parse(item.stat2)).ToList()
                          : highscoreItems.OrderByDescending(item => float.Parse(item.stat2)).ToList();
                    }
                    break;

                case StatType.Date:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !DateTime.TryParse(item.stat2, out DateTime result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.DateParseFail;
                        Debug.LogError("DateTime Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat2).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat2).ToList();
                    }
                    else
                    {
                        // order with parse date
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => DateTime.Parse(item.stat2)).ToList()
                          : highscoreItems.OrderByDescending(item => DateTime.Parse(item.stat2)).ToList();
                    }
                    break;
            }
        }

        /// <summary>
        /// Function for sorting the highscore table ordered by stat3.
        /// </summary>
        public void SortStat3()
        {
            CheckStatType3();

            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow3").text = isAscending ? "▲" : "▼";
            HighscoreManager.statTitles[2].isBeingSorted = false;

            // reset the other arrows
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow1").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow2").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow4").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow5").text = "";

            HighscoreManager.statTitles[0].isBeingSorted = false;
            HighscoreManager.statTitles[1].isBeingSorted = false;
            HighscoreManager.statTitles[3].isBeingSorted = false;
            HighscoreManager.statTitles[4].isBeingSorted = false;

            isAscending = !isAscending;
            UpdateTable(HighscoreManager, highscoreItems);
        }

        /// <summary>
        /// Helper function for sorting stat3 according to the selected stat type.
        /// </summary>
        public void CheckStatType3()
        {
            bool parseError = false;
            parseResult = SortingParseResult.NoErrors;

            switch (HighscoreManager.statTitles[2].statType)
            {
                case StatType.String:
                    highscoreItems = isAscending
                      ? highscoreItems.OrderBy(item => item.stat3).ToList()
                      : highscoreItems.OrderByDescending(item => item.stat3).ToList();
                    break;
                case StatType.Int:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !int.TryParse(item.stat3, out int result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.IntParseFail;
                        Debug.LogError("Int Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat3).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat3).ToList();
                    }
                    else
                    {
                        // order with parse int
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => int.Parse(item.stat3)).ToList()
                          : highscoreItems.OrderByDescending(item => int.Parse(item.stat3)).ToList();
                    }
                    break;

                case StatType.Float:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !float.TryParse(item.stat3, out float result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.FloatParseFail;
                        Debug.LogError("Float Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat3).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat3).ToList();
                    }
                    else
                    {
                        // order with parse float
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => float.Parse(item.stat3)).ToList()
                          : highscoreItems.OrderByDescending(item => float.Parse(item.stat3)).ToList();
                    }
                    break;

                case StatType.Date:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !DateTime.TryParse(item.stat3, out DateTime result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.DateParseFail;
                        Debug.LogError("DateTime Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat3).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat3).ToList();
                    }
                    else
                    {
                        // order with parse date
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => DateTime.Parse(item.stat3)).ToList()
                          : highscoreItems.OrderByDescending(item => DateTime.Parse(item.stat3)).ToList();
                    }
                    break;
            }
        }

        /// <summary>
        /// Function for sorting the highscore table ordered by stat4.
        /// </summary>
        public void SortStat4()
        {
            CheckStatType4();

            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow4").text = isAscending ? "▲" : "▼";
            HighscoreManager.statTitles[3].isBeingSorted = true;

            // reset the other arrows
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow1").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow2").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow3").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow5").text = "";

            HighscoreManager.statTitles[0].isBeingSorted = false;
            HighscoreManager.statTitles[1].isBeingSorted = false;
            HighscoreManager.statTitles[2].isBeingSorted = false;
            HighscoreManager.statTitles[4].isBeingSorted = false;

            isAscending = !isAscending;
            UpdateTable(HighscoreManager, highscoreItems);
        }

        /// <summary>
        /// Helper function for sorting stat4 according to the selected stat type.
        /// </summary>
        public void CheckStatType4()
        {
            bool parseError = false;
            parseResult = SortingParseResult.NoErrors;

            switch (HighscoreManager.statTitles[3].statType)
            {
                case StatType.String:
                    highscoreItems = isAscending
                      ? highscoreItems.OrderBy(item => item.stat4).ToList()
                      : highscoreItems.OrderByDescending(item => item.stat4).ToList();
                    break;
                case StatType.Int:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !int.TryParse(item.stat4, out int result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.IntParseFail;
                        Debug.LogError("Int Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat4).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat4).ToList();
                    }
                    else
                    {
                        // order with parse int
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => int.Parse(item.stat4)).ToList()
                          : highscoreItems.OrderByDescending(item => int.Parse(item.stat4)).ToList();
                    }
                    break;

                case StatType.Float:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !float.TryParse(item.stat4, out float result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.FloatParseFail;
                        Debug.LogError("Float Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat4).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat4).ToList();
                    }
                    else
                    {
                        // order with parse float
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => float.Parse(item.stat4)).ToList()
                          : highscoreItems.OrderByDescending(item => float.Parse(item.stat4)).ToList();
                    }
                    break;

                case StatType.Date:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !DateTime.TryParse(item.stat4, out DateTime result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.DateParseFail;
                        Debug.LogError("DateTime Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat4).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat4).ToList();
                    }
                    else
                    {
                        // order with parse date
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => DateTime.Parse(item.stat4)).ToList()
                          : highscoreItems.OrderByDescending(item => DateTime.Parse(item.stat4)).ToList();
                    }
                    break;
            }
        }

        /// <summary>
        /// Function for sorting the highscore table ordered by stat5.
        /// </summary>
        public void SortStat5()
        {
            CheckStatType5();

            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow5").text = isAscending ? "▲" : "▼";
            HighscoreManager.statTitles[4].isBeingSorted = true;

            // reset the other arrows
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow1").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow2").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow3").text = "";
            instance.tableTexts.FirstOrDefault(t => t.name == "Arrow4").text = "";

            HighscoreManager.statTitles[0].isBeingSorted = false;
            HighscoreManager.statTitles[1].isBeingSorted = false;
            HighscoreManager.statTitles[2].isBeingSorted = false;
            HighscoreManager.statTitles[3].isBeingSorted = false;

            isAscending = !isAscending;
            UpdateTable(HighscoreManager, highscoreItems);
        }

        /// <summary>
        /// Helper function for sorting stat5 according to the selected stat type.
        /// </summary>
        public void CheckStatType5()
        {
            bool parseError = false;
            parseResult = SortingParseResult.NoErrors;

            switch (HighscoreManager.statTitles[4].statType)
            {
                case StatType.String:
                    highscoreItems = isAscending
                      ? highscoreItems.OrderBy(item => item.stat5).ToList()
                      : highscoreItems.OrderByDescending(item => item.stat5).ToList();
                    break;
                case StatType.Int:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !int.TryParse(item.stat5, out int result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.IntParseFail;
                        Debug.LogError("Int Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat5).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat5).ToList();
                    }
                    else
                    {
                        // order with parse int
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => int.Parse(item.stat5)).ToList()
                          : highscoreItems.OrderByDescending(item => int.Parse(item.stat5)).ToList();
                    }
                    break;

                case StatType.Float:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !float.TryParse(item.stat5, out float result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.FloatParseFail;
                        Debug.LogError("Float Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat5).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat5).ToList();
                    }
                    else
                    {
                        // order with parse float
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => float.Parse(item.stat5)).ToList()
                          : highscoreItems.OrderByDescending(item => float.Parse(item.stat5)).ToList();
                    }
                    break;

                case StatType.Date:
                    foreach (HighscoreItem item in highscoreItems)
                    {
                        parseError = !DateTime.TryParse(item.stat5, out DateTime result);
                    }

                    if (parseError)
                    {
                        // order as normal strings if parse fail
                        parseResult = SortingParseResult.DateParseFail;
                        Debug.LogError("DateTime Parse Fail! Order as normal strings!");
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => item.stat5).ToList()
                          : highscoreItems.OrderByDescending(item => item.stat5).ToList();
                    }
                    else
                    {
                        // order with parse date
                        highscoreItems = isAscending
                          ? highscoreItems.OrderBy(item => DateTime.Parse(item.stat5)).ToList()
                          : highscoreItems.OrderByDescending(item => DateTime.Parse(item.stat5)).ToList();
                    }
                    break;
            }
        }

        #endregion

    }
}
