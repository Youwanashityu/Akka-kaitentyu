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

    [Header("鍵初期所持数")]
    [SerializeField] private int _initialKeyCount = 100;

    [Header("ガチャプール（全アイテム）")]
    [SerializeField] private GachaItem[] _ssrItems;
    [SerializeField] private GachaItem[] _srItems;
    [SerializeField] private GachaItem[] _rItems;

    [Header("PickUp対象キャラクター（最大2体）")]
    [SerializeField] private GachaItem[] _pickUpCharacters;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private GachaItem _selectedPickUp = null;
    private readonly Dictionary<GachaItem, int> _inventory = new();
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

        _keyCount = _initialKeyCount;
    }

    // -------------------------------------------------------
    // PickUp選択
    // -------------------------------------------------------

    public void SelectPickUp(GachaItem character)
    {
        _selectedPickUp = character;
        Debug.Log($"[GachaManager] PickUp選択: {character?.DisplayName ?? "なし"}");
    }

    // -------------------------------------------------------
    // 鍵管理
    // -------------------------------------------------------

    public void AddKey(int amount)
    {
        _keyCount += amount;
        Debug.Log($"[GachaManager] 鍵追加: {amount} → 残り{_keyCount}");
    }

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

    public GachaItem DrawSingle()
    {
        if (!TryConsumeKey(_singleCost)) return null;
        var result = Draw();
        AddToInventory(result);
        return result;
    }

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

    private GachaItem Draw()
    {
        var tier = DrawTier();
        return DrawItemFromTier(tier);
    }

    private ItemTier DrawTier()
    {
        float rand = Random.value;
        if (rand < _ssrRate) return ItemTier.SSR;
        if (rand < _ssrRate + _srRate) return ItemTier.SR;
        return ItemTier.R;
    }

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