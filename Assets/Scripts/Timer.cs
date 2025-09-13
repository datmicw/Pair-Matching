using UnityEngine;

// lớp Timer dùng để đếm thời gian trong game
public class Timer : MonoBehaviour
{
    public static Timer Instance;   // singleton: chỉ có 1 instance duy nhất của Timer

    public GUIStyle ClockStyle;// style cho đồng hồ hiển thị trên màn hình

    private float _timer;// biến lưu tổng thời gian đã trôi qua
    private float _minutes;// số phút
    private float _seconds;// số giây

    private const float VirtualWidth = 480.0f;   // chiều rộng ảo để scale UI
    private const float VirtualHeight = 854.0f;  // chiều cao ảo để scale UI

    private bool _stopTimer;// biến kiểm tra có dừng timer không
    private Matrix4x4 _matrix;// ma trận để scale UI
    private Matrix4x4 _oldMatrix;// lưu lại ma trận cũ của GUI

    // trả về thời gian hiện tại của timer
    public float GetTime() => _timer;

    // dừng timer
    public void StopTimer() => _stopTimer = true;

    // reset timer về 0 và chạy lại
    public void ResetTimer() { _timer = 0; _stopTimer = false; }

    // hàm này chạy đầu tiên khi object được tạo
    void Awake()
    {
        // nếu chưa có instance nào thì gán instance này
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // nếu đã có thì hủy object này để đảm bảo chỉ có 1 Timer
        }
    }

    // hàm này chạy khi bắt đầu game
    void Start()
    {
        _stopTimer = false; // cho phép timer chạy
        // tạo ma trận scale UI theo kích thước màn hình
        _matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
            new Vector3(Screen.width / VirtualWidth, Screen.height / VirtualHeight, 1));
        _oldMatrix = GUI.matrix; // lưu lại ma trận cũ
    }

    // cập nhật timer mỗi frame
    void Update()
    {
        if (!_stopTimer)
        {
            _timer += Time.deltaTime; // tăng thời gian theo thời gian thực
        }
    }

    // vẽ đồng hồ lên màn hình
    private void OnGUI()
    {
        GUI.matrix = _matrix; // áp dụng ma trận scale UI

        _minutes = Mathf.Floor(_timer / 60); // tính số phút
        _seconds = Mathf.RoundToInt(_timer % 60); // tính số giây

        // vẽ label hiển thị thời gian lên màn hình
        GUI.Label(new Rect(Camera.main.rect.x + 20, 10, 120, 50),
            _minutes.ToString("00") + ":" + _seconds.ToString("00"), ClockStyle);

        GUI.matrix = _oldMatrix; // trả lại ma trận cũ
    }
}
