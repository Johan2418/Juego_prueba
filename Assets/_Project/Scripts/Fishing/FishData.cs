using UnityEngine;

public enum FishRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2
}

[CreateAssetMenu(menuName = "CozyGameplay/Fishing/Fish Data", fileName = "FishData")]
public class FishData : ItemData
{
    [SerializeField] private FishRarity rarity = FishRarity.Common;
    [SerializeField] private int sellPrice = 1;
    [SerializeField] private float catchWeight = 1f;

    public FishRarity Rarity => rarity;
    public int SellPrice => Mathf.Max(0, sellPrice);
    public float CatchWeight => Mathf.Max(0.01f, catchWeight);

    protected override void OnValidate()
    {
        base.OnValidate();
        catchWeight = Mathf.Max(0.01f, catchWeight);
        sellPrice = Mathf.Max(0, sellPrice);
    }
}
