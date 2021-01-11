using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.DTOs
{
    public class TransactionToReturnDTO
    {
        public string TransactionId { get; set; }

        public string TransactionType { get; set; }

        public string TransactionStatus { get; set; }

        public string TransactionCurrency { get; set; }

        public decimal Amount { get; set; }

        public string WalletId { get; set; }
    }
}
