using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.BankTransfer.Services.Interfaces
{
    public interface IRechargeKeyGenerator
    {
        string GenerateRechargeKey(string amount, string encryptionKey);
    }
}
