using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Interfaces;
using minimalApi.Infra.Db;

namespace minimalApi.Domain.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _dbContext;

        public AdminService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Admin? Criar(Admin admin)
        {
            _dbContext.Admins.Add(admin);
            _dbContext.SaveChanges();
            return admin;
        }

        public Admin? Login(LoginDTO login)
        {
            return _dbContext.Admins.Where(a => a.Email == login.Email && a.Senha == login.Senha).FirstOrDefault();
        }

        public List<Admin> Todos(int? page)
        {
            int itensPorPagina = 10;
            var query = _dbContext.Admins.AsQueryable();

            if (page != null)
                query = query.Skip(((int)page - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
        }
    }
}