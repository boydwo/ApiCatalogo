using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.DTOs
{
    public class UsuarioDTO
    {
        public string Email { get; set; }
        public string Passoword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
