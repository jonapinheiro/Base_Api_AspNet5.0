using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkW_Models.Models
{
    public class EntityUserLogin
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int Cod_Cadastro { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string MD5 { get; set; }
        public DateTime DataRecuperacao { get; set; }
        public int Type { get; set; }
        public int Tentativas { get; set; }
    }
}
