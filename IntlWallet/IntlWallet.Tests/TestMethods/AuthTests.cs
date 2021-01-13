using IntlWallet.Data.Services;
using IntlWallet.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.Tests.TestMethods
{
    public class AuthTests
    {
        //private SqlServerConnection sqlServerConnection;

        //private Mock<IUserMainCurrencyRepository> userMainCurrencyRepository;
        //private List<ApplicationUser> users;

        [SetUp]
        public void Setup()
        {
            //userMainCurrencyRepository = new Mock<IUserMainCurrencyRepository>();
            //users = new List<ApplicationUser>();
            //users.Add(new ApplicationUser()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    LastName = "Jonny",
            //    FirstName = "Bravo",
            //    Email = "jonny@gmail.com",
            //    PhoneNumber = "+2348938758938",
            //    UserName = "jonny@gmail.com",
            //    Wallets = new List<Wallet>()
            //    {
            //        new Wallet()
            //        {
            //            WalletId = Guid.NewGuid().ToString(),
            //            WalletCurrency = "USD"
            //        }
            //    },
            //    UserMainCurrencyDetails = new UserMainCurrencyDetail()
            //    {
            //        MainCurrency = "USD"
            //    }
            //});

            //IServiceCollection serviceCollection = new ServiceCollection();

            //sqlServerConnection = new SqlServerConnection("DataSource=:memory:");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
