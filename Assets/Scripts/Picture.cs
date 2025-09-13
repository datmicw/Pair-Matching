using System.Collections;
using UnityEngine;

// lớp Picture đại diện cho một hình ảnh trong game
public class Picture : MonoBehaviour
{
    public AudioClip PressSound; // âm thanh khi nhấn vào hình
    private Material _firstMaterial; // vật liệu mặt trước
    private Material _secondMaterial; // vật liệu mặt sau
    private Quaternion _currentRotation; // lưu lại góc quay hiện tại

    [HideInInspector] public bool Revealed = false; // trạng thái đã lật hay chưa
    private PictureManager _pictureManager; // tham chiếu đến PictureManager
    private bool _clicked = false; // kiểm tra đã click chưa

    private int _index; // chỉ số của hình
    private AudioSource _audio; // component phát âm thanh

    // thiết lập chỉ số cho hình
    public void SetIndex(int id) { _index = id; }
    // lấy chỉ số của hình
    public int GetIndex() => _index;

    // hàm khởi tạo
    void Start()
    {
        Revealed = false; // chưa lật
        _clicked = false; // chưa click
        // lấy tham chiếu đến PictureManager
        _pictureManager = GameObject.Find("[PictureManager]").GetComponent<PictureManager>();
        // lưu lại góc quay ban đầu
        _currentRotation = gameObject.transform.rotation;
        // lấy component AudioSource và gán âm thanh
        _audio = GetComponent<AudioSource>();
        _audio.clip = PressSound;
    }

    // sự kiện khi nhấn chuột vào hình
    private void OnMouseDown()
    {
        // nếu chưa click và trạng thái cho phép lật hình
        if (!_clicked && _pictureManager.CurrentPuzzleState == PictureManager.PuzzleState.CanRotate)
        {
            // nếu không tắt âm thanh hiệu ứng
            if (!GameSetting.Instance.IsSoundEffectMutedPermanent())
            {
                _audio.Play(); // phát âm thanh
            }
            // bắt đầu coroutine quay hình
            StartCoroutine(LoopRotation(45, false));
            _clicked = true; // đánh dấu đã click
        }
    }

    // hàm lật lại hình về trạng thái ban đầu
    public void FlipBack()
    {
        if (gameObject.activeSelf)
        {
            Revealed = false; // đánh dấu chưa lật
            if (!GameSetting.Instance.IsSoundEffectMutedPermanent())
            {
                _audio.Play(); // phát âm thanh
            }
            // bắt đầu coroutine quay hình về mặt trước
            StartCoroutine(LoopRotation(45, true));
        }
    }

    // coroutine xử lý hiệu ứng quay hình
    IEnumerator LoopRotation(float angle, bool FirstMat)
    {
        // nếu quay về mặt trước
        if (FirstMat)
        {
            var step = Time.deltaTime * 720.0f; // tốc độ quay
            transform.Rotate(new Vector3(0, 2, 0), step);
            // khi quay đủ góc thì đổi vật liệu về mặt trước
            ApplyFirstMaterial();
            yield return null;
        }
        else // quay về mặt sau
        {
            while (angle > 0)
            {
                float step = Time.deltaTime * 360.0f; // tốc độ quay
                transform.Rotate(new Vector3(0, 2, 0), step);
                angle -= step;
                yield return null;
            }
        }

        // đặt lại góc quay ban đầu
        transform.rotation = _currentRotation;

        if (!FirstMat)
        {
            Revealed = true; // đánh dấu đã lật
            ApplySecondMaterial(); // đổi vật liệu về mặt sau

            _pictureManager.NotifyRevealed(this); // thông báo đã lật cho PictureManager
        }
        else
        {
            Revealed = false; // đánh dấu chưa lật
        }

        _clicked = false; // cho phép click lại
    }

    // thiết lập vật liệu mặt trước và nạp texture từ đường dẫn
    public void SetFirstMaterial(Material mat, string texturePath)
    {
        _firstMaterial = mat;
        _firstMaterial.mainTexture = Resources.Load(texturePath, typeof(Texture2D)) as Texture2D;
    }

    // thiết lập vật liệu mặt sau và nạp texture từ đường dẫn
    public void SetSecondMaterial(Material mat, string texturePath)
    {
        _secondMaterial = mat;
        _secondMaterial.mainTexture = Resources.Load(texturePath, typeof(Texture2D)) as Texture2D;
    }

    // áp dụng vật liệu mặt trước cho hình
    public void ApplyFirstMaterial()
    {
        GetComponent<Renderer>().material = _firstMaterial;
    }

    // áp dụng vật liệu mặt sau cho hình
    public void ApplySecondMaterial()
    {
        GetComponent<Renderer>().material = _secondMaterial;
    }

    // ẩn hình khỏi màn hình
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
