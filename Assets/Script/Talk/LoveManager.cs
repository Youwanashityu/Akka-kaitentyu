using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 好感度・チュートリアル済みフラグ・プレゼント済みフラグを管理するマネージャー。
/// シングルトンとして動作しシーンをまたいでデータを保持します。
/// プレゼント済みフラグはキャラクターごとに管理します。
/// </summary>
public class LoveManager : MonoBehaviour
{
    public static LoveManager Instance { get; private set; }

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    /// <summary>キャラクターごとの好感度（キー：CharacterName）</summary>
    private readonly Dictionary<string, int> _lovePoints = new();

    /// <summary>キャラクターごとのチュートリアル済みフラグ</summary>
    private readonly HashSet<string> _tutorialDoneCharacters = new();

    /// <summary>キャラクターごとのプレゼント済みアイテム（キー：CharacterName）</summary>
    private readonly Dictionary<string, HashSet<GachaItem>> _presentedItems = new();

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------------------------------------------------------
    // 好感度
    // -------------------------------------------------------

    public int GetLovePoint(string characterName)
    {
        return _lovePoints.TryGetValue(characterName, out var point) ? point : 0;
    }

    public void AddLovePoint(string characterName, int amount)
    {
        if (!_lovePoints.ContainsKey(characterName))
            _lovePoints[characterName] = 0;
        _lovePoints[characterName] += amount;
        Debug.Log($"[LoveManager] {characterName} 好感度: {_lovePoints[characterName]}");
    }

    // -------------------------------------------------------
    // チュートリアル済みフラグ
    // -------------------------------------------------------

    public bool IsTutorialDone(string characterName)
    {
        return _tutorialDoneCharacters.Contains(characterName);
    }

    public void SetTutorialDone(string characterName)
    {
        _tutorialDoneCharacters.Add(characterName);
    }

    // -------------------------------------------------------
    // プレゼント済みフラグ（キャラクターごと）
    // -------------------------------------------------------

    /// <summary>
    /// 指定キャラクターに指定アイテムをプレゼント済みか返します。
    /// </summary>
    public bool IsPresented(string characterName, GachaItem item)
    {
        if (!_presentedItems.TryGetValue(characterName, out var items)) return false;
        return items.Contains(item);
    }

    /// <summary>
    /// 指定キャラクターに指定アイテムをプレゼント済みにします。
    /// </summary>
    public void SetPresented(string characterName, GachaItem item)
    {
        if (!_presentedItems.ContainsKey(characterName))
            _presentedItems[characterName] = new HashSet<GachaItem>();
        _presentedItems[characterName].Add(item);
    }
}