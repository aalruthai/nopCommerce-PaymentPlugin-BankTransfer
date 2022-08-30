using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.BankTransfer.Models
{
    public class BankTransferSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a description text
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable products are required in order to display this payment method during checkout
        /// </summary>
        public bool ShippableProductRequired { get; set; }

        /// <summary>
        /// Gets or sets allowed file extensions for receipts
        /// </summary>
        public string AllowedFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets Recharge code encryption key
        /// </summary>
        public string RechargeCodeEncryptionKey { get; set; }

        /// <summary>
        /// Maximum file upload size in (Kb)
        /// </summary>
        public int MaxFileSize { get; set; }
    }
}
