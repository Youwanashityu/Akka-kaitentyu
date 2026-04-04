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

    [Header("CSVファイル名（拡張子なし）")]
    [SerializeField] private string _like1CsvName = "Lux_Like1";
    [SerializeField] private string _like2CsvName = "Lux_Like2";
    [SerializeField] private string _like3CsvName = "Lux_Like3";
    [SerializeField] private string _tutorialCsvName = "Lux_Tutorial";

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    /// <summary>好感度ポイント</summary>
    private int _lovePoint = 0;

    /// <summary>会話中かどうか</summary>
    private bool _isTalking = false;

    /// <summary>チュートリアル済みかどうか</summary>
    private bool _tutorialDone = false;

    /// <summary>好感度レベルごとの会話開始IDリスト</summary>
    private Dictionary<int, List<string>> _talkStartIDs = new();

    /// <summary>好感度レベルごとのCSVデータ</summary>
    private Dictionary<int, Dictionary<string, TalkData>> _talkDataByLevel = new();

    /// <summary>チュートリアルCSVデータ</summary>
    private Dictionary<string, TalkData> _tutorialTalkData;

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        LoadAllCsv();
        _characterButton.onClick.AddListener(OnCharacterTapped);
    }

    private void OnDestroy()
    {
        _cts = _cts.Clear();
    }

    // -------------------------------------------------------
    // CSV読み込み
    // -------------------------------------------------------

    private void LoadAllCsv()
    {
        _tutorialTalkData = TalkDataLoader.Load(_tutorialCsvName);

        var like1Data = TalkDataLoader.Load(_like1CsvName);
        var like2Data = TalkDataLoader.Load(_like2CsvName);
        var like3Data = TalkDataLoader.Load(_like3CsvName);

        _talkDataByLevel[1] = like1Data;
        _talkDataByLevel[2] = like2Data;
        _talkDataByLevel[3] = like3Data;

        // 各レベルの会話開始IDを収集（NextOnA/Bで参照されないIDが開始ID）
        _talkStartIDs[1] = GetStartIDs(like1Data);
        _talkStartIDs[2] = GetStartIDs(like2Data);
        _talkStartIDs[3] = GetStartIDs(like3Data);
    }

    /// <summary>
    /// 他の会話から参照されていないIDを開始IDとして返します。
    /// </summary>
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
            // チュートリアル会話
            _tutorialDone = true;
            talkData = _tutorialTalkData;
            startID = GetTutorialStartID();
        }
        else
        {
            // 好感度に応じたランダム会話
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

        // 会話後に好感度を加算
        AddLovePoint(_gameSetting.TalkLoveAmount);

        _isTalking = false;
    }

    // -------------------------------------------------------
    // 会話ID取得
    // -------------------------------------------------------

    private string GetTutorialStartID()
    {
        if (_tutorialTalkData == null || _tutorialTalkData.Count == 0) return null;
        // チュートリアルは最初のIDから開始
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
    /// 好感度を加算します。
    /// </summary>
    public void AddLovePoint(int amount)
    {
        _lovePoint += amount;
        Debug.Log($"[HomeTalkController] 好感度: {_lovePoint}（Lv{_gameSetting.GetLikeLevel(_lovePoint)}）");
    }

    public int LovePoint => _lovePoint;
}