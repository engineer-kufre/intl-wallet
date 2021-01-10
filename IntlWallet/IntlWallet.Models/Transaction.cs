using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntlWallet.Models
{
    public class Transaction
    {
        [Key]
        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string TransactionType { get; set; }

        [Required]
        public string TransactionStatus { get; set; } = "approved";

        [Required]
        public string TransactionCurrency { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
