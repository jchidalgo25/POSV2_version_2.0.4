using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Control
{
    public class Monedero
    {
        TblPuntosCab _puntos;

        public decimal getSaldo()
        {
            if (_puntos != null)
            {
                return _puntos.Saldo;
            }
            return 0M;
        }                   
                 
    }
}
