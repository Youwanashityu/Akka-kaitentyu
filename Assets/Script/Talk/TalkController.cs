using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// ホーム画面の会話UIを管理するコントローラー。
/// テキスト表示・キャラクター画像・選択肢ボタンの制御を担います。
/// TalkPlayerから呼び出されます。
/// </summary>
public class TalkController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("キャラクター画像")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Image miniCharaImage;

    [Header("テキスト")]
    [SerializeField] private TMP_Text talkText;

    [Header("選択肢テキスト")]
    [SerializeField] private TMP_Text selectionTextA;
    [SerializeField] private TMP_Text selectionTextB;

    [Header("ボタン")]
    [SerializeField] private Button charaTalkButton;
    [SerializeField] private Button miniTalkButton;

    [Header("吹き出し")]
    [SerializeField] private GameObject talkBox;

    [Header("選択肢ボタン")]
    [SerializeField] private Button selectionButtonA;
    [SerializeField] private Button selectionButtonB;

    // -------------------------------------------------------
    // プロパティ
    // -------------------------------------------------------

    public Image CharacterImage => characterImage;
    public Image MiniCharaImage => miniCharaImage;
    public TMP_Text TalkText => talkText;
    public TMP_Text SelectionTextA => selectionTextA;
    public TMP_Text SelectionTextB => selectionTextB;
    public Button CharaTalkButton => charaTalkButton;
    public Button MiniTalkButton => miniTalkButton;
    public GameObject TalkBox => talkBox;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private CancellationTokenSource _cts;

    // -------------------------------------------------------
    // 初期化
    // -------------------------------------------------------

    /// <summary>
    /// キャラクター画像をセットしてUIを初期状態にします。
    /// </summary>
    public void Initialize(Sprite chara, Sprite mini)
    {
        _cts = _cts.Clear();
        characterImage.sprite = chara;
        miniCharaImage.sprite = mini;
        talkBox.SetActive(false);
        selectionButtonA.gameObject.SetActive(false);
        selectionButtonB.gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 選択肢表示
    // -------------------------------------------------------

    /// <summary>
    /// 選択肢ボタンを表示してプレイヤーの入力を待ちます。
    /// 空文字を渡したボタンは非表示になります。
    /// </summary>
    /// <returns>押されたボタンに対応するSelectionType</returns>
    public async UniTask<SelectionType> Question(string alphaText, string betaText, CancellationToken token)
    {
        _cts = _cts.Reset();
        var linkedToken = _cts.LinkedToken(token);

        selectionTextA.text = alphaText;
        selectionTextB.text = betaText;

        // 空文字ならボタンを非表示にする
        selectionButtonA.gameObject.SetActive(!string.IsNullOrEmpty(alphaText));
        selectionButtonB.gameObject.SetActive(!string.IsNullOrEmpty(betaText));

        var alpha = selectionButtonA.onClick.OnInvokeAsync(linkedToken);
        var beta = selectionButtonB.onClick.OnInvokeAsync(linkedToken);

        try
        {
            var index = await UniTask.WhenAny(alpha, beta);
            return index == 0 ? SelectionType.Alpha : SelectionType.Beta;
        }
        finally
        {
            selectionTextA.text = "-";
            selectionTextB.text = "-";
            selectionButtonA.gameObject.SetActive(false);
            selectionButtonB.gameObject.SetActive(false);
        }
    }
}