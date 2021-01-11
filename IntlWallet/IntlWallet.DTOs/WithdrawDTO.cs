using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.DTOs
{
    public class WithdrawDTO
    {
        public string TransactionCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
