using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.Entities;

namespace minimalApi.Domain.Interfaces
{
    public interface IVeiculoService
    {
        List<Veiculo> Todos(int? pagina = 1, string nome = null, string marca = null);
        Veiculo? BuscarPorId(int id);
        void Salvar(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);
    }
}