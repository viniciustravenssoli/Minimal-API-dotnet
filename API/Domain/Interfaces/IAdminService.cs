using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;

namespace minimalApi.Domain.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(LoginDTO login);
        Admin? Criar(Admin requestCreate);
        List<Admin> Todos(int? page);
    }
}