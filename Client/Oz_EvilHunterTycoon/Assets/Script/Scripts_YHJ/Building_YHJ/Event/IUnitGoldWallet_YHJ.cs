public interface IUnitGoldWallet_YHJ
{
    int Gold { get; }
    bool TrySpendGold(int amount);
    void AddGold(int amount);
}
