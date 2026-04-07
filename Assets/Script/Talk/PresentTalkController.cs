using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレゼントボタンの制御とアイテムに対応した会話の呼び出しを担うコントローラー。
/// アイテムごと・キャラクターごとに初回と2回目以降で会話を切り替えます。
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

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Dictionary<string, TalkData> _presentTalkData;
    private bool _isTalking = false;
    private string _characterName;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _presentButton.onClick.AddListener(OnPresentButtonClicked);
    }

    // -------------------------------------------------------
    // キャラクター切り替え
    // -------------------------------------------------------

    public void SetCharacter(string characterName)
    {
        _characterName = characterName;
        _presentTalkData = TalkDataLoader.Load($"{characterName}_Present");
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

        GachaManager.Instance.ConsumeItem(item);

        // キャラクターごとに初回か2回目以降かを判定
        string startID;
        bool isPresented = LoveManager.Instance.IsPresented(_characterName, item);

        if (isPresented && !string.IsNullOrEmpty(item.PresentTalkIDAgain))
            startID = item.PresentTalkIDAgain;
        else
        {
            startID = item.PresentTalkID;
            LoveManager.Instance.SetPresented(_characterName, item);
        }

        if (string.IsNullOrEmpty(startID) || _presentTalkData == null || !_presentTalkData.ContainsKey(startID))
        {
            Debug.LogWarning($"[PresentTalkController] プレゼント会話が見つかりません: {startID}");
            _isTalking = false;
            return;
        }

        _talkPlayer.Setup(_presentTalkData);
        await _talkPlayer.Play(startID, token);

        var loveAmount = _gameSetting.GetPresentLoveAmount(item);
        _homeTalkController.AddLovePoint(loveAmount);

        _isTalking = false;
    }
}