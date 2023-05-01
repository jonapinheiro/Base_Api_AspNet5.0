using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkW_Models.Dto
{
    public class LoginDto
    {
        public int Cod_Cadastro { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public int Type { get; set; }
    }
}
