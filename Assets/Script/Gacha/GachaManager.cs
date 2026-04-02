using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ガチャの抽選・PickUp管理・鍵消費・所持品保持を担うマネージャー。
/// シングルトンとして動作します。
/// </summary>
public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance { get; private set; }

    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("排出率（合計が1.0になるように設定）")]
    [SerializeField][Range(0f, 1f)] private float _ssrRate = 0.03f;
    [SerializeField][Range(0f, 1f)] private float _srRate = 0.15f;
    // R は 1 - SSR - SR で自動計算

    [Header("PickUp設定")]
    [Tooltip("PickUp選択時にPickUpキャラがSSRプール内で選ばれる確率（0.5 = 50%）")]
    [SerializeField][Range(0f, 1f)] private float _pickUpRate = 0.5f;

    [Header("鍵消費数")]
    [SerializeField] private int _singleCost = 1;
    [SerializeField] private int _tenCost = 10;

    [Header("ガチャプール（全アイテム）")]
    [SerializeField] private GachaItem[] _ssrItems;
    [SerializeField] private GachaItem[] _srItems;
    [SerializeField] private GachaItem[] _rItems;

    [Header("PickUp対象キャラクター（最大2体）")]
    [SerializeField] private GachaItem[] _pickUpCharacters;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    /// <summary>現在のPickUpキャラ（nullなら未選択）</summary>
    private GachaItem _selectedPickUp = null;

    /// <summary>所持アイテム（アイテム → 個数）</summary>
    private readonly Dictionary<GachaItem, int> _inventory = new();

    /// <summary>所持鍵の数</summary>
    private int _keyCount = 0;

    public int KeyCount => _keyCount;
    public GachaItem SelectedPickUp => _selectedPickUp;
    public IReadOnlyDictionary<GachaItem, int> Inventory => _inventory;

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
    // PickUp選択
    // -------------------------------------------------------

    /// <summary>
    /// PickUpキャラを選択します。nullを渡すと解除。
    /// </summary>
    public void SelectPickUp(GachaItem character)
    {
        _selectedPickUp = character;
        Debug.Log($"[GachaManager] PickUp選択: {character?.DisplayName ?? "なし"}");
    }

    // -------------------------------------------------------
    // 鍵管理
    // -------------------------------------------------------

    /// <summary>鍵を追加します。</summary>
    public void AddKey(int amount)
    {
        _keyCount += amount;
        Debug.Log($"[GachaManager] 鍵追加: {amount} → 残り{_keyCount}");
    }

    /// <summary>鍵が足りるか確認して消費します。足りない場合はfalseを返します。</summary>
    private bool TryConsumeKey(int amount)
    {
        if (_keyCount < amount)
        {
            Debug.LogWarning($"[GachaManager] 鍵が不足しています。必要: {amount}, 所持: {_keyCount}");
            return false;
        }
        _keyCount -= amount;
        return true;
    }

    // -------------------------------------------------------
    // ガチャ実行
    // -------------------------------------------------------

    /// <summary>
    /// 1回ガチャを実行します。鍵が足りない場合はnullを返します。
    /// </summary>
    public GachaItem DrawSingle()
    {
        if (!TryConsumeKey(_singleCost)) return null;

        var result = Draw();
        AddToInventory(result);
        return result;
    }

    /// <summary>
    /// 10連ガチャを実行します。鍵が足りない場合はnullを返します。
    /// </summary>
    public GachaItem[] DrawTen()
    {
        if (!TryConsumeKey(_tenCost)) return null;

        var results = new GachaItem[10];
        for (int i = 0; i < 10; i++)
        {
            results[i] = Draw();
            AddToInventory(results[i]);
        }
        return results;
    }

    // -------------------------------------------------------
    // 抽選ロジック
    // -------------------------------------------------------

    /// <summary>
    /// 1回分の抽選を行います。
    /// </summary>
    private GachaItem Draw()
    {
        var tier = DrawTier();
        return DrawItemFromTier(tier);
    }

    /// <summary>
    /// レアリティを抽選します。
    /// </summary>
    private ItemTier DrawTier()
    {
        float rand = Random.value;
        if (rand < _ssrRate) return ItemTier.SSR;
        if (rand < _ssrRate + _srRate) return ItemTier.SR;
        return ItemTier.R;
    }

    /// <summary>
    /// レアリティに応じてアイテムを1つ選びます。
    /// SSRかつPickUp選択中の場合、_pickUpRateの確率でPickUpキャラが選ばれます。
    /// </summary>
    private GachaItem DrawItemFromTier(ItemTier tier)
    {
        switch (tier)
        {
            case ItemTier.SSR:
                if (_selectedPickUp != null && Random.value < _pickUpRate)
                    return _selectedPickUp;
                return RandomFrom(_ssrItems);

            case ItemTier.SR:
                return RandomFrom(_srItems);

            default:
                return RandomFrom(_rItems);
        }
    }

    /// <summary>
    /// 配列からランダムに1つ選びます。
    /// </summary>
    private GachaItem RandomFrom(GachaItem[] items)
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogError("[GachaManager] アイテムプールが空です。");
            return null;
        }
        return items[Random.Range(0, items.Length)];
    }

    // -------------------------------------------------------
    // 所持品管理
    // -------------------------------------------------------

    /// <summary>
    /// アイテムを所持品に追加します。
    /// </summary>
    private void AddToInventory(GachaItem item)
    {
        if (item == null) return;

        if (_inventory.ContainsKey(item))
            _inventory[item]++;
        else
            _inventory[item] = 1;

        Debug.Log($"[GachaManager] 所持品追加: {item.DisplayName}（計{_inventory[item]}個）");
    }
}