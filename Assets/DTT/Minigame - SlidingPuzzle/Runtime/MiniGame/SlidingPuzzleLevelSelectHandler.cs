using DTT.MinigameBase.UI;
using UnityEngine;
using DTT.MinigameBase.LevelSelect;
using UnityEngine.UI;
using System;

namespace DTT.MiniGame.SlidingPuzzle
{
    public class SlidingPuzzleLevelSelectHandler : LevelSelectHandler<LevelData, SlidingPuzzleResult, SlidingPuzzleManager>
    {
        [SerializeField] private LevelData[] _configs;
        [Header("Autocomplete localized sprites")]
        [SerializeField] private Image _autocompleteButtonImage;
        [SerializeField] private Sprite _autocompleteRussianSprite;
        [SerializeField] private Sprite _autocompleteEnglishSprite;
        [SerializeField] private Sprite _autocompleteTurkishSprite;

        [Header("Level select title")]
        [SerializeField] private Text _chooseLevelText;

        private void Start()
        {
            GameUI gameUI = FindObjectOfType<GameUI>();
            if (gameUI != null)
                gameUI.NextLevelRequested += LoadNextLevel;

            SetupLanguageButtons();
            ApplyLanguage(GameLanguageStorage.StartupLanguage);
        }

        private void SetupLanguageButtons()
        {
            SetupButton("RuLang", GameLanguage.Russian);
            SetupButton("EnLang", GameLanguage.English);
            SetupButton("TrLang", GameLanguage.Turkish);
        }

        private void SetupButton(string buttonName, GameLanguage language)
        {
            GameObject buttonObject = GameObject.Find(buttonName);
            if (buttonObject == null)
                return;

            Button button = buttonObject.GetComponent<Button>();
            if (button == null)
                return;

            button.onClick.AddListener(() => ApplyLanguage(language));
        }

        private void ApplyLanguage(GameLanguage language)
        {
            GameLanguageStorage.CurrentLanguage = language;
            ApplyChooseLevelText(language);
            ApplyAutocompleteButtonSprite(language);
        }

        private void ApplyChooseLevelText(GameLanguage language)
        {
            if (_chooseLevelText == null)
                _chooseLevelText = FindChooseLevelText();

            if (_chooseLevelText == null)
                return;

            _chooseLevelText.text = language switch
            {
                GameLanguage.English => "Choose level",
                GameLanguage.Turkish => "Seviye seçin",
                _ => "Выберите уровень"
            };
        }


        private Text FindChooseLevelText()
        {
            Transform knownPath = transform.root.Find("Level Select/Container/Banner/Text");
            if (knownPath != null && knownPath.TryGetComponent(out Text knownText))
                return knownText;

            Text[] allTexts = FindObjectsOfType<Text>(true);
            foreach (Text text in allTexts)
            {
                if (IsChooseLevelText(text.text))
                    return text;
            }

            return null;
        }

        private bool IsChooseLevelText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string normalized = value.Trim();

            return normalized.Equals("Выберите уровень", StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("ВЫБЕРИТЕ УРОВЕНЬ", StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("Choose level", StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("Seviye seçin", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyAutocompleteButtonSprite(GameLanguage language)
        {
            if (_autocompleteButtonImage == null)
            {
                GameObject autocompleteButton = GameObject.Find("Autocomplete Button");
                if (autocompleteButton != null)
                    _autocompleteButtonImage = autocompleteButton.GetComponent<Image>();
            }

            if (_autocompleteButtonImage == null)
                return;

            Sprite sprite = language switch
            {
                GameLanguage.English => _autocompleteEnglishSprite,
                GameLanguage.Turkish => _autocompleteTurkishSprite,
                _ => _autocompleteRussianSprite
            };

            if (sprite != null)
                _autocompleteButtonImage.sprite = sprite;
        }

        private void LoadNextLevel()
        {
            int nextLevelNumber = CurrentLevel + 1;
            if (nextLevelNumber > _configs.Length)
            {
                ShowLevelSelect();
                return;
            }

            var minigame = FindObjectOfType<SlidingPuzzleManager>();
            if (minigame == null) return;

            SetCurrentLevel(nextLevelNumber);
            minigame.ClearLevel();
            minigame.StartGame(GetConfig(nextLevelNumber));
        }

        protected override float CalculateScore(SlidingPuzzleResult result) => result.score;

        protected override LevelData GetConfig(int levelNumber) => _configs[(levelNumber - 1) % _configs.Length];
    }
}
