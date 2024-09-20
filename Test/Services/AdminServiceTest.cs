using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Services;
using minimalApi.Infra.Db;

namespace Test.Services
{
    [TestClass]
    public class AdminServiceTest
    {

        private AppDbContext CreateContextTest()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new AppDbContext(configuration);
        }

        [TestMethod]
        public void TesteSalvarAdmin()
        {
            // Arrange
            var adm = new Admin();

            adm.Email = "testEmailComBandoParaTest";
            adm.Senha = "testSenhaComBandoParaTest";

            // Act

            var context = CreateContextTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");

            var adminService = new AdminService(context);

            adminService.Criar(adm);

            // Assert

            Assert.AreEqual(1, adminService.Todos(1).Count());
        }
    }
}