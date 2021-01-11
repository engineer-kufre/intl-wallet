using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.DTOs
{
    public class WalletToReturnDTO
    {
        public string WalletId { get; set; }

        public string ApplicationUserId { get; set; }

        public string WalletCurrency { get; set; }

        public decimal Balance { get; set; }
    }
}
