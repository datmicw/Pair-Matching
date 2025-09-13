using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// lớp quản lý các thiết lập của game
public class GameSetting : MonoBehaviour
{
    // từ điển ánh xạ loại câu đố sang tên thư mục
    private readonly Dictionary<EPuzzleCategory, string> _puzzleCatDirectory = new Dictionary<EPuzzleCategory, string>();
    // biến đếm số lượng thiết lập đã được chọn
    private int _settings;
    // hằng số số lượng thiết lập cần thiết
    private const int SettingsNumber = 2;
    // biến lưu trạng thái tắt hiệu ứng âm thanh vĩnh viễn
    private bool _muteFxPermanently = false;

    // enum số lượng cặp
    public enum EPairNumber
    {
        NotSet = 0,
        E10Pairs = 10,
        E15Pairs = 15,
        E20Pairs = 20
    }
    // enum loại câu đố
    public enum EPuzzleCategory
    {
        NotSet = 0,
        Fruits,
        Vegetables
    }
    // struct lưu các thiết lập game
    public struct Settings
    {
        public EPairNumber PairsNumber;
        public EPuzzleCategory PuzzleCategory;
    }
    // biến lưu các thiết lập hiện tại của game
    private Settings _gameSettings;
    // biến static để truy cập instance duy nhất của class này
    public static GameSetting Instance;

    // hàm Awake khởi tạo instance duy nhất và không bị huỷ khi load scene mới
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
            SetPuzzleCatDirectory();
        }
        else
        {
            Destroy(this);
        }
    }
    // hàm Start khởi tạo giá trị mặc định cho các thiết lập
    void Start()
    {
        _gameSettings = new Settings();
        ResetGameSettings();
    }
    // hàm thiết lập ánh xạ loại câu đố sang tên thư mục
    private void SetPuzzleCatDirectory()
    {
        _puzzleCatDirectory.Add(EPuzzleCategory.Fruits, "Fruits");
        _puzzleCatDirectory.Add(EPuzzleCategory.Vegetables, "Vegetables");
    }
    // hàm thiết lập số lượng cặp
    public void SetPairsNumber(EPairNumber Number)
    {
        if (_gameSettings.PairsNumber == EPairNumber.NotSet)
        {
            _settings++;
        }
        _gameSettings.PairsNumber = Number;
    }
    // hàm thiết lập loại câu đố
    public void SetPuzzleCategory(EPuzzleCategory cat)
    {
        if (_gameSettings.PuzzleCategory == EPuzzleCategory.NotSet)
        {
            _settings++;
        }
        _gameSettings.PuzzleCategory = cat;
    }
    // hàm lấy số lượng cặp hiện tại
    public EPairNumber GetEPairNumber()
    {
        return _gameSettings.PairsNumber;
    }
    // hàm lấy loại câu đố hiện tại
    public EPuzzleCategory GetEPuzzleCategory()
    {
        return _gameSettings.PuzzleCategory;
    }
    // hàm reset lại các thiết lập về mặc định
    public void ResetGameSettings()
    {
        _settings = 0;
        _gameSettings.PuzzleCategory = EPuzzleCategory.NotSet;
        _gameSettings.PairsNumber = EPairNumber.NotSet;
    }
    // kiểm tra xem đã chọn đủ các thiết lập chưa
    public bool AllSettingsReady()
    {
        return _settings == SettingsNumber;
    }
    // trả về tên thư mục chứa material
    public string GetMaterialDirectoryName()
    {
        return "Materials/";
    }
    // trả về tên thư mục chứa texture của loại câu đố đã chọn
    public string GetPuzzleCategoryTextureDirectoryName()
    {
        if (_puzzleCatDirectory.ContainsKey(_gameSettings.PuzzleCategory))
        {
            return "Graphics/PuzzleCat/" + _puzzleCatDirectory[_gameSettings.PuzzleCategory] + "/";
        }
        else
        {
            Debug.LogError("Puzzle Category not set");
            return "";
        }
    }
    // thiết lập trạng thái tắt hiệu ứng âm thanh vĩnh viễn
    public void MuteSoundEffectPermanent(bool mute) { _muteFxPermanently = mute; }
    // kiểm tra trạng thái tắt hiệu ứng âm thanh vĩnh viễn
    public bool IsSoundEffectMutedPermanent() => _muteFxPermanently;
}
