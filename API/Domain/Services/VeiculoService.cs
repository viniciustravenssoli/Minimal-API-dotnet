using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Interfaces;
using minimalApi.Infra.Db;

namespace minimalApi.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly AppDbContext _context;

        public VeiculoService(AppDbContext context)
        {
            _context = context;
        }

        public void Apagar(Veiculo veiculo)
        {
            _context.Veiculos.Remove(veiculo);
            _context.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            _context.SaveChanges();
        }

        public Veiculo? BuscarPorId(int id)
        {
            return _context.Veiculos.Find(id);
        }

        public void Salvar(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
        }

        public List<Veiculo> Todos(int? pagina = 1, string nome = null, string marca = null)
        {
            int itensPorPagina = 10;
            var query = _context.Veiculos.AsQueryable();

            // Filtro por nome, utilizando StringComparison.OrdinalIgnoreCase para ignorar maiúsculas/minúsculas
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(x => x.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por marca, utilizando StringComparison.OrdinalIgnoreCase para ignorar maiúsculas/minúsculas
            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(x => x.Marca.Contains(marca, StringComparison.OrdinalIgnoreCase));
            }

            if (pagina != null)
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
        }
    }
}