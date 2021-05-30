using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

namespace FraWork.Highscore
{
    /// <summary>
    /// VisualElement class that creates a part of the custom inspector GUI related to the stats <see cref="HighscoreStat"/>.
    /// </summary>
    public class HighscoreStatEditor : VisualElement
    {
        private HighscoreManagerEditor highscoreManagerEditor;
        private HighscoreStat highscoreStat;

        private bool showParseErrorLabel = false;

        /// <summary>
        /// Constructor of this VisualElement class. 
        /// </summary>
        /// <param name="_highscoreManagerEditor">Reference to the highscore manager editor class.</param>
        /// <param name="_highscoreStat">Reference to the highscore stat class</param>
        public HighscoreStatEditor(HighscoreManagerEditor _highscoreManagerEditor, HighscoreStat _highscoreStat)
        {
            highscoreManagerEditor = _highscoreManagerEditor;
            highscoreStat = _highscoreStat;

            VisualElement ui = new VisualElement();

            // adding UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FraWork/Editor/Highscore/HighscoreStatEditor.uxml");
            ui = visualTree.CloneTree();

            // adding USS
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FraWork/Editor/Highscore/HighscoreStatEditor.uss");
            ui.styleSheets.Add(stylesheet);

            #region Fields

            // String Stat Number
            Label statNumber = ui.Q<Label>("statNumber");
            statNumber.text = highscoreStat.statID.ToString();

            // String StatTitle
            TextField statTitle = ui.Q<TextField>("statTitle");
            statTitle.label += $" {highscoreStat.statID.ToString()}";
            statTitle.value = highscoreStat.statTitle;
            statTitle.tooltip = "This is the name of the stat in the table.";
            statTitle.RegisterCallback<ChangeEvent<string>>(e =>
            {
                highscoreStat.statTitle = e.newValue;
                highscoreManagerEditor.UpdateStatTitles();
            });

            // Toggle IsVisible
            Toggle isVisible = ui.Q<Toggle>("isVisible");
            isVisible.value = highscoreStat.isVisible;
            isVisible.tooltip = "Toggle ON if you want to display this stat in the table, OFF otherwise.";
            isVisible.RegisterCallback<ChangeEvent<bool>>(e =>
            {
                highscoreStat.isVisible = e.newValue;
                highscoreManagerEditor.UpdateVisibleColumns();
            });

            EnumField statTypeField = ui.Q<EnumField>("statTypeEnum");
            statTypeField.value = highscoreStat.statType;
            statTypeField.tooltip = "StatType will try to cast the items to the selected type when sorting.";
            statTypeField.RegisterCallback<ChangeEvent<Enum>>(e =>
            {
                highscoreStat.statType = (StatType)e.newValue;

                List<HighscoreItem> items = HighscoreTable.GetHighscoreItems();

                //if (highscoreStat.isBeingSorted && HighscoreTable.Initialised)
                if (HighscoreTable.Initialised)
                {
                    switch (highscoreStat.statID)
                    {
                        case 1:
                            if (highscoreStat.isBeingSorted)
                            {
                                HighscoreTable.StaticSortStat1();
                            }
                            else
                            {
                                HighscoreTable.StaticCheckStatType1();
                                ShowParseErrorLabel(ref ui);
                            }
                            break;

                        case 2:

                            if (highscoreStat.isBeingSorted)
                            {
                                HighscoreTable.StaticSortStat2();
                            }
                            else
                            {
                                HighscoreTable.StaticCheckStatType2();
                                ShowParseErrorLabel(ref ui);
                            }
                            break;

                        case 3:
                            if (highscoreStat.isBeingSorted)
                            {
                                HighscoreTable.StaticSortStat3();
                            }
                            else
                            {
                                HighscoreTable.StaticCheckStatType3();
                                ShowParseErrorLabel(ref ui);
                            }
                            break;

                        case 4:
                            if (highscoreStat.isBeingSorted)
                            {
                                HighscoreTable.StaticSortStat4();
                            }
                            else
                            {
                                HighscoreTable.StaticCheckStatType4();
                                ShowParseErrorLabel(ref ui);
                            }

                            break;

                        case 5:
                            if (highscoreStat.isBeingSorted)
                            {
                                HighscoreTable.StaticSortStat5();
                            }
                            else
                            {
                                HighscoreTable.StaticCheckStatType5();
                                ShowParseErrorLabel(ref ui);
                            }

                            break;
                    }
                }
            });

            if (HighscoreTable.Initialised)
            {
                ShowParseErrorLabel(ref ui);
            }
            else
            {
                ui.Q<Label>("parseErrorLabel").style.display = DisplayStyle.None;
            }

            #endregion

            Add(ui);
        }

        /// <summary>
        /// Function that shows a label under the dropdown stat type if there is an error in the parsing to that specific datatype
        /// </summary>
        /// <param name="ui">Reference to this stat VisualElement</param>
        public void ShowParseErrorLabel(ref VisualElement ui)
        {
            switch (HighscoreTable.parseResult)
            {
                case SortingParseResult.NoErrors:
                    ui.Q<Label>("parseErrorLabel").style.display = DisplayStyle.None;
                    break;
                case SortingParseResult.IntParseFail:
                    ui.Q<Label>("parseErrorLabel").text = "Int parse failed!\nString type used";
                    ui.Q<Label>("parseErrorLabel").style.display = DisplayStyle.Flex;
                    break;
                case SortingParseResult.FloatParseFail:
                    ui.Q<Label>("parseErrorLabel").text = "Float parse failed!\nString type used";
                    ui.Q<Label>("parseErrorLabel").style.display = DisplayStyle.Flex;
                    break;
                case SortingParseResult.DateParseFail:
                    ui.Q<Label>("parseErrorLabel").text = "Date parse failed!\nString type used";
                    ui.Q<Label>("parseErrorLabel").style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}