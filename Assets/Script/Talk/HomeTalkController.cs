using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ホーム画面のキャラクタータップによる会話を管理するコントローラー。
/// 好感度レベルに応じてランダムに会話を選択してTalkPlayerに渡します。
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

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private int _lovePoint = 0;
    private bool _isTalking = false;
    private bool _tutorialDone = false;
    private string _characterName;

    private Dictionary<int, List<string>> _talkStartIDs = new();
    private Dictionary<int, Dictionary<string, TalkData>> _talkDataByLevel = new();
    private Dictionary<string, TalkData> _tutorialTalkData;

    private CancellationTokenSource _cts;

    public int LovePoint => _lovePoint;

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

    /// <summary>
    /// 表示キャラクターが切り替わったときに呼びます。
    /// CSVを読み直して会話データを更新します。
    /// </summary>
    public void SetCharacter(string characterName)
    {
        _characterName = characterName;
        _tutorialDone = false;
        _lovePoint = 0;
        _talkStartIDs.Clear();
        _talkDataByLevel.Clear();
        LoadAllCsv();
        _lovePointBar.UpdateBar(_lovePoint);
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

        if (!_tutorialDone)
        {
            _tutorialDone = true;
            talkData = _tutorialTalkData;
            startID = GetTutorialStartID();
        }
        else
        {
            var level = _gameSetting.GetLikeLevel(_lovePoint);
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

    /// <summary>
    /// 好感度を加算してバーを更新します。MAXに達したらエンディングを表示します。
    /// </summary>
    public void AddLovePoint(int amount)
    {
        _lovePoint += amount;
        _lovePointBar.UpdateBar(_lovePoint);

        Debug.Log($"[HomeTalkController] 好感度: {_lovePoint}（Lv{_gameSetting.GetLikeLevel(_lovePoint)}）");

        if (_gameSetting.IsMax(_lovePoint))
        {
            _endingPopup.Show();
        }
    }
}