using UnityEngine;
using UnityEngine.UI;

// script này dùng để điều khiển nút tắt/mở hiệu ứng âm thanh
[RequireComponent(typeof(Button))] // đảm bảo đối tượng luôn có component Button
public class MuteSoundEffectButton : MonoBehaviour
{
    public Sprite UnMutedFxSprite;// sprite khi hiệu ứng âm thanh đang bật
    public Sprite MutedFxSprite;// sprite khi hiệu ứng âm thanh đang tắt

    private Button _button;// tham chiếu tới component Button
    private SpriteState _state; // lưu trạng thái sprite của button

    // hàm này được gọi khi bắt đầu chạy script
    void Start()
    {
        _button = GetComponent<Button>(); // lấy component Button trên đối tượng
        if (GameSetting.Instance.IsSoundEffectMutedPermanent()) // kiểm tra nếu hiệu ứng âm thanh đang tắt
        {
            _state.pressedSprite = MutedFxSprite;// đặt sprite khi nhấn là sprite tắt
            _state.highlightedSprite = MutedFxSprite;// đặt sprite khi hover là sprite tắt
            _button.GetComponent<Image>().sprite = MutedFxSprite; // đổi hình ảnh button thành sprite tắt
        }
        else // nếu hiệu ứng âm thanh đang bật
        {
            _state.pressedSprite = UnMutedFxSprite; // đặt sprite khi nhấn là sprite bật
            _state.highlightedSprite = UnMutedFxSprite;// đặt sprite khi hover là sprite bật
            _button.GetComponent<Image>().sprite = UnMutedFxSprite; // đổi hình ảnh button thành sprite bật
        }
    }

    // hàm này được gọi mỗi frame để cập nhật giao diện
    void OnGUI()
    {
        if (GameSetting.Instance.IsSoundEffectMutedPermanent()) // nếu hiệu ứng âm thanh đang tắt
        {
            _button.GetComponent<Image>().sprite = MutedFxSprite; // đổi hình ảnh button thành sprite tắt
        }
        else // nếu hiệu ứng âm thanh đang bật
        {
            _button.GetComponent<Image>().sprite = UnMutedFxSprite; // đổi hình ảnh button thành sprite bật
        }
    }

    // hàm này được gọi khi người dùng nhấn nút để chuyển trạng thái hiệu ứng âm thanh
    public void ToggleFxIcon()
    {
        if (GameSetting.Instance.IsSoundEffectMutedPermanent()) // nếu đang tắt thì bật lên
        {
            _state.pressedSprite = UnMutedFxSprite;// đặt sprite khi nhấn là sprite bật
            _state.highlightedSprite = UnMutedFxSprite;// đặt sprite khi hover là sprite bật
            GameSetting.Instance.MuteSoundEffectPermanent(false); // bật hiệu ứng âm thanh
        }
        else // nếu đang bật thì tắt đi
        {
            _state.pressedSprite = MutedFxSprite;// đặt sprite khi nhấn là sprite tắt
            _state.highlightedSprite = MutedFxSprite;// đặt sprite khi hover là sprite tắt
            GameSetting.Instance.MuteSoundEffectPermanent(true); // tắt hiệu ứng âm thanh
        }
    }
}
