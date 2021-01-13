using IntlWallet.Data.Services;
using IntlWallet.Models;
using IntlWallet.Utils;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.Tests.TestMethods
{
    public class UtilsTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void InvalidCurrencyThrowsArgumentException()
        {
            //Arrange
            var transactionCurrency = "EUA";
            var mainCurrency = "NGN";
            var amount = 1;

            //Assert
            Assert.Throws<AggregateException>(() => CurrencyConverter.ConvertCurrency(transactionCurrency, mainCurrency, amount).Wait());
        }

        [Test]
        public void InvalidCurrencyInputReturnsFalse()
        {
            //Arrange
            var currency = "USA";
            var expected = false;

            //Act
            var actual = CurrencyConverter.ValidateCurrencyInput(currency);

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
