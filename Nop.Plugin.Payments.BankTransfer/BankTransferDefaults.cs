using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.BankTransfer
{
    public class BankTransferDefaults
    {
        public static string UploadFileRouteName => "Plugin.Payments.BankTransfer.UploadFile";
        public static string PluginSystemName => "Payments.BankTransfer";

        /// <summary>
        /// One page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";
    }
}
