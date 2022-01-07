using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    private readonly Dictionary<EPuzzleCategories, string> _puzzleCatDirectory = new Dictionary<EPuzzleCategories, string>();
    private int _settings;
    private const int SettingsNumber = 2; //    1-> Kart sayýsý__2 Kategori
    private bool _muteFxPermanently = false;
    public enum EPairNumber
    {
        NotSet = 0,
        E10Pairs = 10,
        E15Pairs = 15,
        E20Pairs = 20,
    }
    public enum EPuzzleCategories
    {
        NotSet,
        Card1,
        Card2,
    }
    public struct Settings
    {
        public EPairNumber PairNumber;
        public EPuzzleCategories PuzzleCategory;
    };

    private Settings _gameSettings;

    public static GameSettings Instance;

    void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SetPuzzleCatDirectory();
        _gameSettings = new Settings();
        ResetGameSettings();

    }

    private void SetPuzzleCatDirectory()
    {
        _puzzleCatDirectory.Add(EPuzzleCategories.Card1, "Card1");
        _puzzleCatDirectory.Add(EPuzzleCategories.Card2, "Card2");
    }
    public  void SetPairNumber(EPairNumber Number)
    {
        if (_gameSettings.PairNumber == EPairNumber.NotSet)
            _settings++;

        _gameSettings.PairNumber = Number;
            }

    public void SetPuzzleCategories(EPuzzleCategories cat)
    {
        if (_gameSettings.PuzzleCategory == EPuzzleCategories.NotSet)
            _settings++;

        _gameSettings.PuzzleCategory = cat;
    }

    public EPairNumber GetPairNumber()
    { 
        return _gameSettings.PairNumber;
    }

    public EPuzzleCategories GetPuzzleCategories()
    {
        return _gameSettings.PuzzleCategory;
    }

    public void ResetGameSettings()
    {
        _settings = 0;
        _gameSettings.PuzzleCategory = EPuzzleCategories.NotSet;
        _gameSettings.PairNumber = GameSettings.EPairNumber.NotSet;
    }
    public bool AllSettingsReady()
    {
        return _settings == SettingsNumber;
    }

    public string GetMaterialDirectoryame()
    {
        return "Materials/";
    }

    public string GetPuzzleCategoryTextureDirectoryName()
    {
        if (_puzzleCatDirectory.ContainsKey(_gameSettings.PuzzleCategory))
        {
            return "Graphics/PuzzleCat/" + _puzzleCatDirectory[_gameSettings.PuzzleCategory] + "/";
        }
        else
        {
            Debug.LogError("ERROR: CANNOT GET DIRECTORY NAME");
            return "";
        }

    }
    public void MuteSoundEffectPermanently(bool muted)
    { 
        _muteFxPermanently = muted;
    }

    public bool IsSoundEffectMutedPermanently()
    {
        return _muteFxPermanently;
    }
}
