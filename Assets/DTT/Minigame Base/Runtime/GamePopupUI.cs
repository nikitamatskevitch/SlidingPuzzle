using System;
using System.Reflection;
using DTT.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    public enum GameLanguage
    {
        Russian = 0,
        English = 1,
        Turkish = 2
    }

    public static class GameLanguageStorage
    {
        public const string PlayerPrefsKey = "game_language";

        private const string YandexGamesTypeName = "YG.YG2, Assembly-CSharp";
        private const string YandexGamesLanguageFieldName = "lang";

        public static GameLanguage CurrentLanguage
        {
            get => (GameLanguage)PlayerPrefs.GetInt(PlayerPrefsKey, (int)GameLanguage.Russian);
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKey, (int)value);
                PlayerPrefs.Save();
            }
        }

        public static GameLanguage StartupLanguage
        {
            get
            {
                if (TryGetPlatformLanguage(out GameLanguage platformLanguage))
                    return platformLanguage;

                return CurrentLanguage;
            }
        }

        public static bool TryGetPlatformLanguage(out GameLanguage language)
        {
            language = GameLanguage.Russian;

            Type yandexGamesType = Type.GetType(YandexGamesTypeName);
            FieldInfo languageField = yandexGamesType?.GetField(
                YandexGamesLanguageFieldName,
                BindingFlags.Public | BindingFlags.Static);

            string languageCode = languageField?.GetValue(null) as string;
            if (string.IsNullOrWhiteSpace(languageCode))
                return false;

            language = ToGameLanguage(languageCode);
            return true;
        }

        private static GameLanguage ToGameLanguage(string languageCode)
        {
            string normalizedLanguageCode = languageCode.Trim().ToLowerInvariant();
            int separatorIndex = normalizedLanguageCode.IndexOfAny(new[] { '-', '_' });
            if (separatorIndex >= 0)
                normalizedLanguageCode = normalizedLanguageCode.Substring(0, separatorIndex);

            return normalizedLanguageCode switch
            {
                "ru" => GameLanguage.Russian,
                "tr" => GameLanguage.Turkish,
                _ => GameLanguage.English
            };
        }
    }

    /// <summary>
    /// Handles the standardized popup in the game.
    /// </summary>
    public class GamePopupUI : MonoBehaviour
    {
        /// <summary>
        /// Called when next level button pressed.
        /// </summary>
        public event Action NextLevelButtonPressed;

        /// <summary>
        /// Called when the resume button is pressed.
        /// </summary>
        public event Action ResumeButtonPressed;
        
        /// <summary>
        /// Called when the restart button is pressed.
        /// </summary>
        public event Action RestartButtonPressed;
        
        /// <summary>
        /// Called when the home button is pressed.
        /// </summary>
        public event Action HomeButtonPressed;

        [SerializeField]
        private Button _nextLevelButton;
        
        /// <summary>
        /// The text object for the title.
        /// </summary>
        [SerializeField]
        private Text _titleText;
        
        /// <summary>
        /// The text object for the backdrop of the title.
        /// </summary>
        [SerializeField]
        private Text _titleBackdropText;

        /// <summary>
        /// The button for resuming.
        /// </summary>
        [SerializeField]
        private Button _resumeButton;
        
        /// <summary>
        /// The button for restarting.
        /// </summary>
        [SerializeField]
        private Button _restartButton;
        
        /// <summary>
        /// The button for returning to home.
        /// </summary>
        [SerializeField]
        private Button _homeButton;

        /// <summary>
        /// Canvas group of the entire popup.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// The animation of showing the popup.
        /// </summary>
        private Coroutine _showAnimation;

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _homeButton.onClick.AddListener(OnHomeButtonClicked);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _homeButton.onClick.RemoveListener(OnHomeButtonClicked);
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }

        private void OnNextLevelButtonClicked() =>
            NextLevelButtonPressed?.Invoke();
        
        public void EnableNextLevelButton(bool isEnabled) =>
            _nextLevelButton.gameObject.SetActive(isEnabled);

        /// <summary>
        /// Called when the resume button is pressed.
        /// </summary>
        private void OnResumeButtonClicked() => ResumeButtonPressed?.Invoke();

        /// <summary>
        /// Called when the restart button is pressed.
        /// </summary>
        private void OnRestartButtonClicked() => RestartButtonPressed?.Invoke();

        /// <summary>
        /// Called when the home button is pressed.
        /// </summary>
        private void OnHomeButtonClicked() => HomeButtonPressed?.Invoke();

        /// <summary>
        /// Sets the title for the paused state.
        /// </summary>
        public void SetTitleToPaused()
        {
            string title = GameLanguageStorage.CurrentLanguage switch
            {
                GameLanguage.English => "PAUSE",
                GameLanguage.Turkish => "DURAKLAT",
                _ => "ПАУЗА"
            };

            _titleText.text = title;
            _titleBackdropText.text = title;
        }

        /// <summary>
        /// Sets the title for the finished state.
        /// </summary>
        public void SetTitleToFinished()
        {
            string title = GameLanguageStorage.CurrentLanguage switch
            {
                GameLanguage.English => "VICTORY!",
                GameLanguage.Turkish => "ZAFER!",
                _ => "ПОБЕДА!"
            };

            _titleText.text = title;
            _titleBackdropText.text = title;
        }

        /// <summary>
        /// Enables the resume button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableResumeButton(bool isEnabled) => _resumeButton.gameObject.SetActive(isEnabled);
        
        /// <summary>
        /// Enables the restart button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableRestartButton(bool isEnabled) => _restartButton.gameObject.SetActive(isEnabled);
        
        /// <summary>
        /// Enables the home button, so it's shown to the user.
        /// </summary>
        /// <param name="isEnabled">Whether to enable or disable</param>
        public void EnableHomeButton(bool isEnabled) => _homeButton.gameObject.SetActive(isEnabled);

        /// <summary>
        /// Shows the popup based on the state.
        /// </summary>
        /// <param name="state">Whether to show the popup.</param>
        public void Show(bool state)
        {
            if(_showAnimation != null)
                StopCoroutine(_showAnimation);

            _canvasGroup.interactable = state;
            _canvasGroup.blocksRaycasts = state;
            
            DTTween.Value(_canvasGroup.alpha, state ? 1f : 0f, 0.6f, Easing.EASE_IN_OUT_SINE,
                alpha => _canvasGroup.alpha = alpha);
        }
    }
}
