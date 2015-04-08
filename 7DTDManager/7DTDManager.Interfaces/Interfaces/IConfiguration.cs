using System;
namespace _7DTDManager.Interfaces
{
    public interface IConfiguration
    {
        string ServerHost { get; }
        int ServerTelnetPort { get; }

        int CoinsPerMinute { get; }
        int CoinsPerZombiekill { get; }
        int CoinLossPerDeath { get; }
    }
}
