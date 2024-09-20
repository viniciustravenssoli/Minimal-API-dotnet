using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Domain.DTOs
{
    public record VeiculoDTO
    {   
        public string Nome { get; set; } = default!;
        public string Marca { get; set; } = default!;
        public string Ano { get; set; } = default!;
    }
}