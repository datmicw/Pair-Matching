using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// script này dùng để thiết lập các tuỳ chọn cho nút bấm trong game
public class SetGameButton : MonoBehaviour
{
    // enum xác định loại nút bấm
    public enum EButtonType
    {
        NotSet,// chưa thiết lập loại nút
        PairsNumbertn,// nút chọn số cặp
        PuzzleCategoryBtn,// nút chọn loại chủ đề
    };

    // loại nút bấm, có thể thiết lập trong inspector
    [SerializeField] public EButtonType ButtonType = EButtonType.NotSet;

    // giá trị số cặp, chỉ dùng khi là nút chọn số cặp
    [HideInInspector] public GameSetting.EPairNumber PairsNumber = GameSetting.EPairNumber.NotSet;

    // giá trị chủ đề, chỉ dùng khi là nút chọn chủ đề
    [HideInInspector] public GameSetting.EPuzzleCategory PuzzleCategory = GameSetting.EPuzzleCategory.NotSet;

    // hàm này được gọi khi script bắt đầu, hiện tại không làm gì
    void Start()
    {
        
    }

    // hàm này được gọi khi nhấn nút, truyền vào tên scene sẽ chuyển đến
    public void SetGameOption(string GameSceneName)
    {
        // lấy component SetGameButton trên game object
        var comp = gameObject.GetComponent<SetGameButton>();

        // kiểm tra loại nút và thiết lập giá trị tương ứng trong GameSetting
        switch (comp.ButtonType)
        {
            case SetGameButton.EButtonType.PairsNumbertn:
                // nếu là nút chọn số cặp thì thiết lập số cặp
                GameSetting.Instance.SetPairsNumber(comp.PairsNumber);
                break;
            case SetGameButton.EButtonType.PuzzleCategoryBtn:
                // nếu là nút chọn chủ đề thì thiết lập chủ đề
                GameSetting.Instance.SetPuzzleCategory(comp.PuzzleCategory);
                break;
        }

        // nếu đã thiết lập đủ các tuỳ chọn thì chuyển scene
        if(GameSetting.Instance.AllSettingsReady())
        {
            SceneManager.LoadScene(GameSceneName);
        }
    }
}
