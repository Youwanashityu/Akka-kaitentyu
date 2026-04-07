using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ホーム画面のキャラクタータップによる会話を管理するコントローラー。
/// チュートリアルキャラとガチャキャラで会話の分岐を切り替えます。
/// </summary>
public class HomeTalkController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("会話再生")]
    [SerializeField] private TalkPlayer _talkPlayer;

    [Header("キャラクタータップボタン")]
    [SerializeField] private Button _characterButton;

    [Header("ゲーム設定")]
    [SerializeField] private GameSettingScriptable _gameSetting;

    [Header("好感度バー")]
    [SerializeField] private LovePointBarController _lovePointBar;

    [Header("エンディングポップアップ")]
    [SerializeField] private EndingPopupController _endingPopup;

    [Header("チュートリアル2回目以降の開始TalkID")]
    [SerializeField] private string _tutorialAgainStartID = "TutorialAgain";

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private bool _isTalking = false;
    private string _characterName;
    private bool _isTutorialCharacter;

    /// <summary>会話中かどうかを外部から参照できます。</summary>
    public bool IsTalking => _isTalking;

    /// <summary>キャラクターごとのエンディング表示済みフラグ</summary>
    private readonly HashSet<string> _endingShownCharacters = new();

    private Dictionary<int, List<string>> _talkStartIDs = new();
    private Dictionary<int, Dictionary<string, TalkData>> _talkDataByLevel = new();
    private Dictionary<string, TalkData> _tutorialTalkData;

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _characterButton.onClick.AddListener(OnCharacterTapped);
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }

    // -------------------------------------------------------
    // キャラクター切り替え
    // -------------------------------------------------------

    public void SetCharacter(string characterName, bool isTutorialCharacter)
    {
        _characterName = characterName;
        _isTutorialCharacter = isTutorialCharacter;
        _talkStartIDs.Clear();
        _talkDataByLevel.Clear();
        LoadAllCsv();
        _lovePointBar.UpdateBar(LoveManager.Instance.GetLovePoint(_characterName));
    }

    // -------------------------------------------------------
    // CSV読み込み
    // -------------------------------------------------------

    private void LoadAllCsv()
    {
        _tutorialTalkData = TalkDataLoader.Load($"{_characterName}_Tutorial");

        for (int level = 1; level <= 3; level++)
        {
            var data = TalkDataLoader.Load($"{_characterName}_Like{level}");
            _talkDataByLevel[level] = data;
            _talkStartIDs[level] = GetStartIDs(data);
        }
    }

    private List<string> GetStartIDs(Dictionary<string, TalkData> data)
    {
        var referencedIDs = new HashSet<string>();

        foreach (var talk in data.Values)
        {
            if (!string.IsNullOrEmpty(talk.NextOnA)) referencedIDs.Add(talk.NextOnA);
            if (!string.IsNullOrEmpty(talk.NextOnB)) referencedIDs.Add(talk.NextOnB);
        }

        var startIDs = new List<string>();
        foreach (var id in data.Keys)
        {
            if (!referencedIDs.Contains(id)) startIDs.Add(id);
        }

        return startIDs;
    }

    // -------------------------------------------------------
    // キャラクタータップ
    // -------------------------------------------------------

    private void OnCharacterTapped()
    {
        if (_isTalking) return;
        TalkAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid TalkAsync(CancellationToken token)
    {
        _isTalking = true;

        string startID;
        Dictionary<string, TalkData> talkData;

        var lovePoint = LoveManager.Instance.GetLovePoint(_characterName);
        var tutorialDone = LoveManager.Instance.IsTutorialDone(_characterName);

        if (_isTutorialCharacter)
        {
            if (!tutorialDone)
            {
                LoveManager.Instance.SetTutorialDone(_characterName);
                talkData = _tutorialTalkData;
                startID = GetTutorialStartID();
            }
            else
            {
                talkData = _tutorialTalkData;
                startID = _tutorialAgainStartID;
            }
        }
        else
        {
            var level = _gameSetting.GetLikeLevel(lovePoint);
            talkData = _talkDataByLevel[level];
            startID = GetRandomStartID(level);
        }

        if (string.IsNullOrEmpty(startID))
        {
            Debug.LogWarning("[HomeTalkController] 会話の開始IDが見つかりません。");
            _isTalking = false;
            return;
        }

        _talkPlayer.Setup(talkData);
        await _talkPlayer.Play(startID, token);

        AddLovePoint(_gameSetting.TalkLoveAmount);

        _isTalking = false;
    }

    // -------------------------------------------------------
    // 会話ID取得
    // -------------------------------------------------------

    private string GetTutorialStartID()
    {
        if (_tutorialTalkData == null || _tutorialTalkData.Count == 0) return null;
        foreach (var id in _tutorialTalkData.Keys) return id;
        return null;
    }

    private string GetRandomStartID(int level)
    {
        if (!_talkStartIDs.TryGetValue(level, out var ids) || ids.Count == 0) return null;
        return ids[Random.Range(0, ids.Count)];
    }

    // -------------------------------------------------------
    // 好感度管理
    // -------------------------------------------------------

    public void AddLovePoint(int amount)
    {
        LoveManager.Instance.AddLovePoint(_characterName, amount);
        var lovePoint = LoveManager.Instance.GetLovePoint(_characterName);
        _lovePointBar.UpdateBar(lovePoint);

        // エンディングは1キャラにつき1回だけ表示
        if (_gameSetting.IsMax(lovePoint) && !_endingShownCharacters.Contains(_characterName))
        {
            _endingShownCharacters.Add(_characterName);
            _endingPopup.Show();
        }
    }
}