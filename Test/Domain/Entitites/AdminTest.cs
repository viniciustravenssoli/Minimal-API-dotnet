using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.Entities;

namespace Test.Domain.Entitites
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var admin = new Admin();
            string email = "test@test.com";
            string senha = "senhateste";
            var perfil = minimalApi.Domain.Enums.Perfil.Admin;

            // Act

            admin.Id = 1;
            admin.Email = "test@test.com";
            admin.Senha = "senhateste";
            admin.Perfil = minimalApi.Domain.Enums.Perfil.Admin;


            // Assert

            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual(email, admin.Email);
            Assert.AreEqual(senha, admin.Senha);
            Assert.AreEqual(perfil, admin.Perfil);
        }
    }
}