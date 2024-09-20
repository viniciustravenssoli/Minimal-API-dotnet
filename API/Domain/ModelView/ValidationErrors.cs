using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Domain.ModelView
{
    public struct ValidationErrors
    {
        public List<string> Mensagens { get; set; }
    }
}