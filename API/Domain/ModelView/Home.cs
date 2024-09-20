using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Domain.ModelView
{
    public struct Home
    {
        public string Mensagem { get => "Bem vindo a Minnimal API de veiculos"; }
        public string Doc { get => "swagger"; }
    }
}