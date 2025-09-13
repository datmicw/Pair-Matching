using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// quản lý toàn bộ logic sinh hình, di chuyển, kiểm tra và tính điểm
public class PictureManager : MonoBehaviour
{
    public Picture PicturePrefab;// prefab hình mẫu
    public Transform PicSpawnPosition;// vị trí sinh ban đầu

    [Space]
    [Header("End Game UI")]
    public GameObject EndGamePanel;// panel kết thúc
    public Text NewBestScoreText;// điểm cao nhất
    public Text YourScoreText;// điểm hiện tại
    public Text EndTimeText;// thời gian hoàn thành
    public Text LastScoreText;// điểm lần trước

    // trạng thái game
    public enum GameState { NoAction, DeletingPuzzle, FlipBack, GameEnd }
    public enum PuzzleState { PuzzleRotating, CanRotate }

    [HideInInspector] public GameState CurrentGameState;
    public PuzzleState CurrentPuzzleState;

    [HideInInspector] public List<Picture> PicturesList = new List<Picture>();

    // offset cho từng số lượng cặp
    private Vector2 _offset10Pairs = new Vector2(1.5f, 1.52f);
    private Vector2 _offset15Pairs = new Vector2(1.08f, 1.22f);
    private Vector2 _offset20Pairs = new Vector2(1.08f, 1.0f);
    private Vector3 _newScaleDown = new Vector3(0.9f, 0.9f, 0.001f);

    // lưu material / texture
    private List<Material> _materialsList = new List<Material>();
    private List<string> _texturePathList = new List<string>();
    private Material _firstMaterial;
    private string _firstTexturePath;

    // lưu các hình đã chọn
    private Picture _firstRevealedPicObj;
    private Picture _secondRevealedPicObj;
    private int _pairNumber;
    private int _removePair;

    void Start()
    {
        // khởi tạo trạng thái ban đầu
        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
        _removePair = 0;
        _pairNumber = (int)GameSetting.Instance.GetEPairNumber();

        LoadMaterial(); // tải các material và texture

        // sinh hình theo số cặp trong setting
        if (GameSetting.Instance.GetEPairNumber() == GameSetting.EPairNumber.E10Pairs)
        {
            SpawnPictureMesh(4, 5, _offset10Pairs, false);
            MovePicture(4, 5, _offset10Pairs);
        }
        else if (GameSetting.Instance.GetEPairNumber() == GameSetting.EPairNumber.E15Pairs)
        {
            SpawnPictureMesh(5, 6, _offset15Pairs, false);
            MovePicture(5, 6, _offset15Pairs);
        }
        else if (GameSetting.Instance.GetEPairNumber() == GameSetting.EPairNumber.E20Pairs)
        {
            SpawnPictureMesh(5, 8, _offset20Pairs, true);
            MovePicture(5, 8, _offset20Pairs);
        }

        // hiển thị điểm lần chơi trước nếu có
        if (LastScoreText != null && PlayerPrefs.HasKey("LastScore"))
        {
            float lastScore = PlayerPrefs.GetFloat("LastScore");
            LastScoreText.text = $"{lastScore:F1}";
        }
    }

    // khi lật 1 hình
    public void NotifyRevealed(Picture pic)
    {
        // nếu chưa có hình nào được chọn thì lưu lại hình này
        if (_firstRevealedPicObj == null)
        {
            _firstRevealedPicObj = pic;
        }
        // nếu đã có 1 hình được chọn và hình này khác hình trước thì lưu lại hình này
        else if (_secondRevealedPicObj == null && pic != _firstRevealedPicObj)
        {
            _secondRevealedPicObj = pic;
            CurrentPuzzleState = PuzzleState.PuzzleRotating;
            StartCoroutine(CheckAfterDelay(0.5f)); // đợi 0.5s rồi kiểm tra
        }
    }

    // coroutine kiểm tra sau 1 khoảng delay
    private IEnumerator CheckAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckPicture();
    }

    // kiểm tra 2 hình vừa lật có giống nhau không
    private void CheckPicture()
    {
        if (_firstRevealedPicObj == null || _secondRevealedPicObj == null) return;

        // nếu giống nhau thì xóa
        if (_firstRevealedPicObj.GetIndex() == _secondRevealedPicObj.GetIndex())
        {
            StartCoroutine(DestroyAfterDelay(0.5f));
        }
        // nếu khác thì lật lại
        else
        {
            StartCoroutine(FlipBackWithDelay(0.8f));
        }
    }

    // coroutine xóa 2 hình sau 1 khoảng delay
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _firstRevealedPicObj.Deactivate();
        _secondRevealedPicObj.Deactivate();

        _removePair++; // tăng số cặp đã xóa
        ResetCheck(); // reset trạng thái kiểm tra
        CheckGameEnd(); // kiểm tra kết thúc game chưa
    }

    // coroutine lật lại 2 hình sau 1 khoảng delay
    private IEnumerator FlipBackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _firstRevealedPicObj.FlipBack();
        _secondRevealedPicObj.FlipBack();

        ResetCheck(); // reset trạng thái kiểm tra
    }

    // reset lại các biến kiểm tra hình
    private void ResetCheck()
    {
        _firstRevealedPicObj = null;
        _secondRevealedPicObj = null;

        CurrentGameState = GameState.NoAction;
        CurrentPuzzleState = PuzzleState.CanRotate;
    }

    // tải các material và texture cho hình
    private void LoadMaterial()
    {
        var materialFilePath = GameSetting.Instance.GetMaterialDirectoryName();
        var textureFilePath = GameSetting.Instance.GetPuzzleCategoryTextureDirectoryName();
        var pairNumber = (int)GameSetting.Instance.GetEPairNumber();
        const string matBaseName = "Pic";
        var firstMaterialName = "Back";

        for (var index = 1; index <= pairNumber; index++)
        {
            var currentFilePath = materialFilePath + matBaseName + index;
            Material mat = Resources.Load(currentFilePath, typeof(Material)) as Material;
            if (mat != null) _materialsList.Add(mat);

            var currentTextureFilePath = textureFilePath + matBaseName + index;
            _texturePathList.Add(currentTextureFilePath);
        }

        _firstTexturePath = textureFilePath + firstMaterialName;
        _firstMaterial = Resources.Load(materialFilePath + firstMaterialName, typeof(Material)) as Material;
    }

    // sinh các hình ra màn hình
    private void SpawnPictureMesh(int rows, int columns, Vector2 offset, bool scaleDown)
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                var tempPicture = Instantiate(PicturePrefab, PicSpawnPosition.position, PicturePrefab.transform.rotation);
                if (scaleDown) tempPicture.transform.localScale = _newScaleDown;
                PicturesList.Add(tempPicture);
            }
        }
        ApplyTextures(); // gán texture cho các hình
    }

    // gán texture cho các hình, đảm bảo mỗi cặp chỉ xuất hiện 2 lần
    private void ApplyTextures()
    {
        var rndMatIndex = UnityEngine.Random.Range(0, _materialsList.Count);
        var AppliedTimes = new int[_materialsList.Count];

        foreach (var o in PicturesList)
        {
            var randPrevios = rndMatIndex;
            var counter = 0;
            var forceMat = false;

            // tìm chỉ số material chưa đủ 2 lần
            while (AppliedTimes[rndMatIndex] >= 2 || rndMatIndex == randPrevios && !forceMat)
            {
                rndMatIndex = UnityEngine.Random.Range(0, _materialsList.Count);
                counter++;
                if (counter > 100)
                {
                    for (var j = 0; j < _materialsList.Count; j++)
                    {
                        if (AppliedTimes[j] < 2)
                        {
                            rndMatIndex = j;
                            forceMat = true;
                        }
                    }
                }
            }

            o.SetFirstMaterial(_firstMaterial, _firstTexturePath); // gán mặt sau
            o.ApplyFirstMaterial();

            o.SetSecondMaterial(_materialsList[rndMatIndex], _texturePathList[rndMatIndex]); // gán mặt trước
            o.SetIndex(rndMatIndex); // lưu chỉ số
            o.Revealed = false;
            AppliedTimes[rndMatIndex]++;
        }
    }

    // di chuyển các hình đến vị trí lưới
    private void MovePicture(int rows, int columns, Vector2 offset)
    {
        int index = 0;

        float totalWidth = (rows - 1) * offset.x;
        float totalHeight = (columns - 1) * offset.y;

        Vector2 startPos = new Vector2(-totalWidth / 2, totalHeight / 2);

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                var targetPosition = new Vector3(
                    (startPos.x + (offset.x * row)),
                    (startPos.y - (offset.y * col)),
                    0.0f);

                StartCoroutine(MoveToPosition(targetPosition, PicturesList[index]));
                index++;
            }
        }
    }

    // coroutine di chuyển hình đến vị trí đích
    private IEnumerator MoveToPosition(Vector3 target, Picture obj)
    {
        var moveSpeed = 7f;
        while (obj.transform.position != target)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    {
        // kiểm tra trạng thái game để thực hiện hành động tương ứng
        switch (CurrentGameState)
        {
            case GameState.DeletingPuzzle:
                if (CurrentPuzzleState == PuzzleState.CanRotate)
                {
                    StartCoroutine(DestroyAfterDelay(0.5f));
                    CurrentGameState = GameState.NoAction;
                }
                break;

            case GameState.FlipBack:
                StartCoroutine(FlipBackWithDelay(0.8f));
                CurrentGameState = GameState.NoAction;
                break;
        }
    }

    // kiểm tra điều kiện kết thúc game
    private bool CheckGameEnd()
    {
        if (_removePair == _pairNumber && CurrentGameState != GameState.GameEnd)
        {
            Timer.Instance.StopTimer(); // dừng đồng hồ
            float playTime = Timer.Instance.GetTime();
            int score = Mathf.RoundToInt(playTime);

            PlayerPrefs.SetFloat("LastScore", playTime); // lưu điểm lần này
            PlayerPrefs.Save();

            EndGamePanel.SetActive(true); // hiện panel kết thúc
            if (YourScoreText != null) YourScoreText.text = $"{score}";
            if (EndTimeText != null) EndTimeText.text = $"{playTime:F1}";

            float bestScore = PlayerPrefs.GetFloat("BestScore", float.MaxValue);

            // cập nhật điểm cao nhất nếu cần
            if (playTime < bestScore)
            {
                bestScore = playTime;
                PlayerPrefs.SetFloat("BestScore", bestScore);
                PlayerPrefs.Save();
            }

            if (NewBestScoreText != null && bestScore < float.MaxValue)
                NewBestScoreText.text = $"{bestScore:F1}";

            CurrentGameState = GameState.GameEnd;
            return true;
        }
        return false;
    }
}
