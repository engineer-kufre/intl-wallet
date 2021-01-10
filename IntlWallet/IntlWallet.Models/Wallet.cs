using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntlWallet.Models
{
    public class Wallet
    {
        [Key]
        [Required]
        public string WalletId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string WalletCurrency { get; set; }

        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
