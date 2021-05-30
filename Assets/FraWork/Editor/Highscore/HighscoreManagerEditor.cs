using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace FraWork.Highscore
{
    /// <summary>
    /// Editor class that creates a custom inspector GUI for the scriptable object <see cref="HighscoreManager"/>.
    /// </summary>
    [CustomEditor(typeof(HighscoreManager))]
    public class HighscoreManagerEditor : Editor
    {
        private HighscoreManager highscoreManager;

        private SerializedProperty backgroundColor;
        private SerializedProperty titlesColor;
        private SerializedProperty titlesFontColor;
        private SerializedProperty firstRowColor;
        private SerializedProperty firstRowFontColor;
        private SerializedProperty secondRowColor;
        private SerializedProperty secondRowFontColor;

        private VisualElement rootElement;
        private VisualElement statTitlesList;

        private bool showTablePreview = true;

        private void OnEnable()
        {
            highscoreManager = target as HighscoreManager;
            rootElement = new VisualElement();

            backgroundColor = serializedObject.FindProperty("backgroundColor");
            titlesColor = serializedObject.FindProperty("titlesColor");
            titlesFontColor = serializedObject.FindProperty("titlesFontColor");
            firstRowColor = serializedObject.FindProperty("firstRowColor");
            firstRowFontColor = serializedObject.FindProperty("firstRowFontColor");
            secondRowColor = serializedObject.FindProperty("secondRowColor");
            secondRowFontColor = serializedObject.FindProperty("secondRowFontColor");
        }

        /// <summary>
        /// Function that actually create the custom inspector GUI using also an UXML and CSS files.
        /// </summary>
        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();

            // adding UXML
            VisualTreeAsset uxmlTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FraWork/Editor/Highscore/HighscoreManagerEditor.uxml");
            rootElement = uxmlTemplate.CloneTree();

            // adding USS
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FraWork/Editor/Highscore/HighscoreManagerEditor.uss");
            rootElement.styleSheets.Add(stylesheet);

            // -- HIGHSCORE TABLE --
            // display backgroundColor and callback
            PropertyField backgroundColorPF = rootElement.Q<PropertyField>("backgroundColor");
            rootElement.Q<Box>("boxContainer").style.backgroundColor = backgroundColor.colorValue;
            backgroundColorPF.RegisterCallback<ChangeEvent<Color>>(e => 
            {
                highscoreManager.UpdateTableBackground();
                rootElement.Q<Box>("boxContainer").style.backgroundColor = e.newValue;
            });

            // display titlesColor and callback
            PropertyField titlesColorPF = rootElement.Q<PropertyField>("titlesColor");
            rootElement.Q<Image>("previewStatTitles").style.backgroundColor = titlesColor.colorValue;
            titlesColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatTitlesColors();
                rootElement.Q<Image>("previewStatTitles").style.backgroundColor = e.newValue;
            });

            // display titlesFontColor and callback
            PropertyField titlesFontColorPF = rootElement.Q<PropertyField>("titlesFontColor");
            rootElement.Q<Label>("previewLabelStatTitle1").style.color = titlesFontColor.colorValue;
            rootElement.Q<Label>("previewLabelStatTitle2").style.color = titlesFontColor.colorValue;
            rootElement.Q<Label>("previewLabelStatTitle3").style.color = titlesFontColor.colorValue;
            titlesFontColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatTitlesColors();
                rootElement.Q<Label>("previewLabelStatTitle1").style.color = e.newValue;
                rootElement.Q<Label>("previewLabelStatTitle2").style.color = e.newValue;
                rootElement.Q<Label>("previewLabelStatTitle3").style.color = e.newValue;
            });

            // display firstRowColor and callback
            PropertyField firstRowColorPF = rootElement.Q<PropertyField>("firstRowColor");
            rootElement.Q<Image>("previewFirstRow").style.backgroundColor = firstRowColor.colorValue;
            firstRowColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatItems();
                rootElement.Q<Image>("previewFirstRow").style.backgroundColor = e.newValue;
            });

            // display firstRowFontColor and callback
            PropertyField firstRowFontColorPF = rootElement.Q<PropertyField>("firstRowFontColor");
            rootElement.Q<Label>("firstRowItem11").style.color = firstRowFontColor.colorValue;
            rootElement.Q<Label>("firstRowItem12").style.color = firstRowFontColor.colorValue;
            rootElement.Q<Label>("firstRowItem13").style.color = firstRowFontColor.colorValue;
            firstRowFontColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatItems();
                rootElement.Q<Label>("firstRowItem11").style.color = e.newValue;
                rootElement.Q<Label>("firstRowItem12").style.color = e.newValue;
                rootElement.Q<Label>("firstRowItem13").style.color = e.newValue;
            });

            // display secondRowColor and callback
            PropertyField secondRowColorPF = rootElement.Q<PropertyField>("secondRowColor");
            rootElement.Q<Image>("previewSecondRow").style.backgroundColor = secondRowColor.colorValue;
            secondRowColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatItems();
                rootElement.Q<Image>("previewSecondRow").style.backgroundColor = e.newValue;
            });

            // display secondRowFontColor and callback
            PropertyField secondRowFontColorPF = rootElement.Q<PropertyField>("secondRowFontColor");
            rootElement.Q<Label>("secondRowItem21").style.color = secondRowFontColor.colorValue;
            rootElement.Q<Label>("secondRowItem22").style.color = secondRowFontColor.colorValue;
            rootElement.Q<Label>("secondRowItem23").style.color = secondRowFontColor.colorValue;
            secondRowFontColorPF.RegisterCallback<ChangeEvent<Color>>(e =>
            {
                highscoreManager.UpdateStatItems();
                rootElement.Q<Label>("secondRowItem21").style.color = e.newValue;
                rootElement.Q<Label>("secondRowItem22").style.color = e.newValue;
                rootElement.Q<Label>("secondRowItem23").style.color = e.newValue;
            });

            // -- PREVIEW TABLE
            Button previewTableButton = rootElement.Q<Button>("buttonPreview");
            previewTableButton.clickable.clicked += PreviewTable;
            
            // -- STATS --
            statTitlesList = rootElement.Q<VisualElement>("statTitlesList");
            UpdateStats();

            // update the table and apply the changes!
            highscoreManager.UpdateTable();
            serializedObject.ApplyModifiedProperties();

            return rootElement;
        }

        /// <summary>
        /// Function called OnClick to show/hide the preview table in the inspector.
        /// </summary>
        private void PreviewTable()
        {
            showTablePreview = !showTablePreview;

            VisualElement tablePreview = rootElement.Q<VisualElement>("tablePreview");
            tablePreview.style.display = showTablePreview ? DisplayStyle.Flex : DisplayStyle.None;

        }

        /// <summary>
        /// Function that will create and update the highscore stats on the inspector based on the <see cref="HighscoreStatEditor"/> editor class.
        /// </summary>
        public void UpdateStats()
        {
            statTitlesList.Clear();

            foreach (HighscoreStat stat in highscoreManager.statTitles)
            {
                HighscoreStatEditor highscoreStatEditor = new HighscoreStatEditor(this, stat);
                statTitlesList.Add(highscoreStatEditor);
            }
        }

        /// <summary>
        /// Reference to the function <see cref="HighscoreManager.UpdateVisibleColumns"/>. 
        /// See the link for more details.
        /// </summary>
        public void UpdateVisibleColumns()
        {
            highscoreManager.UpdateVisibleColumns();
        }

        /// <summary>
        /// Reference to the function <see cref="HighscoreManager.UpdateStatTitles"/>. 
        /// See the link for more details.
        /// </summary>
        public void UpdateStatTitles()
        {
            highscoreManager.UpdateStatTitles();
        }
    }
}
