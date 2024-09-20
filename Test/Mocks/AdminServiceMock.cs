using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Interfaces;

namespace Test.Mocks
{
    public class AdminServiceMock : IAdminService
    {

        private static List<Admin> admins = new List<Admin>();
        public Admin? Criar(Admin requestCreate)
        {
            requestCreate.Id = admins.Count() + 1;
            admins.Add(requestCreate);

            return requestCreate;
        }

        public Admin? Login(LoginDTO login)
        {
             return admins.Find(a => a.Email == login.Email && a.Senha == login.Senha);
        }

        public List<Admin> Todos(int? page)
        {
            return admins;
        }
    }
}