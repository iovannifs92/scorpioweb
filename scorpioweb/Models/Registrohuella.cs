using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Registrohuella
    {
        public int IdregistroHuella { get; set; }
        public byte[] FingerPrint { get; set; }
        public int? PersonaIdPersona { get; set; }
       

    }
}
