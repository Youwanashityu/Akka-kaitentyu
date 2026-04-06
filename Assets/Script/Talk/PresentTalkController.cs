using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレゼントボタンの制御とアイテムに対応した会話の呼び出しを担うコントローラー。
/// アイテムごとに初回と2回目以降で会話を切り替えます。
/// </summary>
public class PresentTalkController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("ボタン")]
    [SerializeField] private Button _presentButton;

    [Header("ポップアップ")]
    [SerializeField] private PresentPopupController _presentPopup;

    [Header("会話")]
    [SerializeField] private TalkPlayer _talkPlayer;

    [Header("好感度")]
    [SerializeField] private HomeTalkController _homeTalkController;
    [SerializeField] private GameSettingScriptable _gameSetting;

    [Header("プレゼントCSVファイル名（拡張子なし）")]
    [SerializeField] private string _presentCsvName = "Lux_Present";

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Dictionary<string, TalkData> _presentTalkData;
    private bool _isTalking = false;

    /// <summary>既にプレゼントしたことがあるアイテムの記録</summary>
    private readonly HashSet<GachaItem> _presentedItems = new();

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _presentTalkData = TalkDataLoader.Load(_presentCsvName);
        _presentButton.onClick.AddListener(OnPresentButtonClicked);
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnPresentButtonClicked()
    {
        if (_isTalking) return;
        _presentPopup.Show(OnItemSelected);
    }

    private void OnItemSelected(GachaItem item)
    {
        PresentAsync(item, this.GetCancellationTokenOnDestroy()).Forget();
    }

    // -------------------------------------------------------
    // プレゼント処理
    // -------------------------------------------------------

    private async UniTaskVoid PresentAsync(GachaItem item, CancellationToken token)
    {
        _isTalking = true;

        // インベントリからアイテムを1つ消費
        GachaManager.Instance.ConsumeItem(item);

        // 初回か2回目以降かでTalkIDを切り替え
        string startID;
        bool isAgain = _presentedItems.Contains(item);

        if (isAgain && !string.IsNullOrEmpty(item.PresentTalkIDAgain))
        {
            startID = item.PresentTalkIDAgain;
        }
        else
        {
            startID = item.PresentTalkID;
            _presentedItems.Add(item);
        }

        if (string.IsNullOrEmpty(startID) || !_presentTalkData.ContainsKey(startID))
        {
            Debug.LogWarning($"[PresentTalkController] プレゼント会話が見つかりません: {startID}");
            _isTalking = false;
            return;
        }

        _talkPlayer.Setup(_presentTalkData);
        await _talkPlayer.Play(startID, token);

        // 好感度加算
        var loveAmount = _gameSetting.GetPresentLoveAmount(item);
        _homeTalkController.AddLovePoint(loveAmount);

        _isTalking = false;
    }
}