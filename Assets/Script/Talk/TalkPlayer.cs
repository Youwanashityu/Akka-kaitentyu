using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// TalkDataを受け取ってテキスト表示・画像切り替え・ボイス再生・選択肢表示を行います。
/// </summary>
public class TalkPlayer : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [SerializeField] private TalkController _talkController;
    [SerializeField] private CharacterSoundPlayer _soundPlayer;

    [Header("SE名")]
    [SerializeField] private string _clickSEName = "SE_click";
    [SerializeField] private string _endSEName = "SE_likeability_up";

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Dictionary<string, TalkData> _talkDataDict;
    private string _lastImageType;

    // -------------------------------------------------------
    // 初期化
    // -------------------------------------------------------

    public void Setup(Dictionary<string, TalkData> talkDataDict)
    {
        _talkDataDict = talkDataDict;
        _lastImageType = string.Empty;
    }

    // -------------------------------------------------------
    // 会話再生
    // -------------------------------------------------------

    public async UniTask Play(string startTalkID, CancellationToken token)
    {
        _talkController.TalkBox.SetActive(true);

        var currentID = startTalkID;

        while (!string.IsNullOrEmpty(currentID))
        {
            if (!_talkDataDict.TryGetValue(currentID, out var data))
            {
                Debug.LogError($"[TalkPlayer] TalkIDが見つかりません: {currentID}");
                break;
            }

            // 画像切り替え（空欄なら前の画像を引き継ぐ）
            if (!string.IsNullOrEmpty(data.ImageType))
            {
                _lastImageType = data.ImageType;
                _soundPlayer.SetImage(data.ImageType);
            }

            // ボイス再生（空欄なら無音）
            if (!string.IsNullOrEmpty(data.VoiceType))
            {
                _soundPlayer.PlayVoice(data.VoiceType);
            }

            // テキスト表示
            _talkController.TalkText.text = data.Text;

            // 選択肢あり
            if (data.HasChoice)
            {
                var selection = await _talkController.Question(data.ChoiceA, data.ChoiceB, token);

                if (selection == SelectionType.Alpha)
                {
                    if (!string.IsNullOrEmpty(data.VoiceOnA))
                        _soundPlayer.PlayVoice(data.VoiceOnA);
                    currentID = data.NextOnA;
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.VoiceOnB))
                        _soundPlayer.PlayVoice(data.VoiceOnB);
                    currentID = data.NextOnB;
                }
            }
            else
            {
                // タップSEを鳴らしてから次へ
                await _talkController.CharaTalkButton.OnClickAsync(token);
                PlaySE(_clickSEName);
                currentID = data.NextOnA;
            }
        }

        // 会話終了SE
        PlaySE(_endSEName);

        _talkController.TalkBox.SetActive(false);
    }

    // -------------------------------------------------------
    // SE再生
    // -------------------------------------------------------

    private void PlaySE(string seName)
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[TalkPlayer] SoundManager が見つかりません。");
            return;
        }
        SoundManager.Instance.PlaySE(seName);
    }
}