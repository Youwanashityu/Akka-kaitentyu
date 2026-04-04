using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

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

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Dictionary<string, TalkData> _talkDataDict;
    private string _lastImageType;

    // -------------------------------------------------------
    // 初期化
    // -------------------------------------------------------

    /// <summary>
    /// 会話データをセットします。
    /// </summary>
    public void Setup(Dictionary<string, TalkData> talkDataDict)
    {
        _talkDataDict = talkDataDict;
        _lastImageType = string.Empty;
    }

    // -------------------------------------------------------
    // 会話再生
    // -------------------------------------------------------

    /// <summary>
    /// 指定したTalkIDから会話を開始します。
    /// </summary>
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
                    // 選択肢Aのボイス再生
                    if (!string.IsNullOrEmpty(data.VoiceOnA))
                        _soundPlayer.PlayVoice(data.VoiceOnA);

                    currentID = data.NextOnA;
                }
                else
                {
                    // 選択肢Bのボイス再生
                    if (!string.IsNullOrEmpty(data.VoiceOnB))
                        _soundPlayer.PlayVoice(data.VoiceOnB);

                    currentID = data.NextOnB;
                }
            }
            else
            {
                // 選択肢なし：タップで次へ
                await _talkController.CharaTalkButton.OnClickAsync(token);
                currentID = data.NextOnA;
            }
        }

        _talkController.TalkBox.SetActive(false);
    }
}