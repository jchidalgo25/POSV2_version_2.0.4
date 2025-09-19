using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POS
{
    public class ValidarIdentificador
    {
        private static Dictionary<string, Dictionary<string, object>> Atributos_identificacion = new Dictionary<string, Dictionary<string, object>>();

        public ValidarIdentificador()
        {
        }

        private static void Init()
        {
            if (Atributos_identificacion.Count == 0)
            {
                Dictionary<string, object> cedula = new Dictionary<string, object>();
                cedula.Add("coeficiente", new List<int> { 2, 1, 2, 1, 2, 1, 2, 1, 2 });
                cedula.Add("modulo", 10);
                cedula.Add("sumar_digitos", true);
                cedula.Add("ultimosdigitos", "001");
                cedula.Add("3digito", new List<int> { 0, 1, 2, 3, 4, 5 });
                cedula.Add("nroProvincias", 24);
                Atributos_identificacion.Add("cedula", cedula);

                Dictionary<string, object> rucpublica = new Dictionary<string, object>();
                rucpublica.Add("coeficiente", new List<int> { 3, 2, 7, 6, 5, 4, 3, 2 });
                rucpublica.Add("modulo", 11);
                rucpublica.Add("sumar_digitos", true);
                rucpublica.Add("ultimosdigitos", "0001");
                rucpublica.Add("3digito", new List<int> { 6 });
                Atributos_identificacion.Add("rucpublica", rucpublica);

                Dictionary<string, object> rucprivada = new Dictionary<string, object>();
                rucprivada.Add("coeficiente", new List<int> { 4, 3, 2, 7, 6, 5, 4, 3, 2 });
                rucprivada.Add("modulo", 11);
                rucprivada.Add("sumar_digitos", false);
                rucprivada.Add("ultimosdigitos", "001");
                rucprivada.Add("3digito", new List<int> { 9 });
                Atributos_identificacion.Add("rucprivada", rucprivada);
            }
        }


        private static bool isValidDigitoVerificador(string value, List<int> coeficientes, int modulo, bool sumar_digitos = false)
        {
            var numeroCedula = 0;
            var numeroCedulaValido = int.TryParse(value, out numeroCedula);
            numeroCedulaValido = (numeroCedula > 0);
            if (numeroCedulaValido)
            {
                int verificador = 0;
                for (int i = 0; i < value.Length - 1; i++)
                {
                    var producto = int.Parse(value[i].ToString()) * coeficientes[i];
                    if (sumar_digitos && producto > 9)
                        producto = producto - 9;
                    verificador += producto;
                }
                verificador = verificador % modulo;
                if (verificador != 0)
                    verificador = modulo - verificador;
                if (verificador != int.Parse(value[value.Length - 1].ToString()))
                    return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidarCedula(string input)
        {
            var val = SoloDigitos(input);
            if (val.Length != 10 || !EsProvinciaValida(val.Substring(0, 2)))
                return false;

            //// Tercer dígito < 6 para personas naturales
            //if (!Entre(val[2], '0', '5'))
            //    return false;

            return Mod10_Cedula(val);
        }
        public static bool ValidarRUCNatural(string input)
        {
            var val = SoloDigitos(input);
            if (val.Length != 13 || !EsProvinciaValida(val.Substring(0, 2)))
                return false;

            //// Tercer dígito < 6 (persona natural)
            //if (!Entre(val[2], '0', '5'))
            //    return false;

            // Los primeros 10 deben ser cédula válida
            if (!ValidarCedula(val.Substring(0, 10)))
                return false;

            // Establecimiento: últimos 3 dígitos >= 001
            if (!EstablecimientoValido(val.Substring(10, 3)))
                return false;

            return true;
        }

        public static bool ValidarRUCPrivada(string input)
        {
            var val = SoloDigitos(input);
            if (val.Length != 13 || !EsProvinciaValida(val.Substring(0, 2)))
                return false;

            // Tercer dígito = 9 (sociedad privada / SAS)
            if (val[2] != '9')
                return false;

            // Dígito verificador (posición 10) con módulo 11 usando 9 coeficientes
            // Coeficientes oficiales (pos 1..9): 4,3,2,7,6,5,4,3,2
            //int[] coef = { 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            //if (!Mod11_Verifica(val, 9, coef, indiceDV: 9))
            //    return false;

            //jchid swtich para validación de digito verificador 
            string numeroSecuencial = val.Substring(3, 7);

            if (TieneNumeroSecuencialLargo(numeroSecuencial))
            {
                return true;
            }
            else
            {
                int[] coeficientes = { 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                if (!Mod11_Verifica(val, 9, coeficientes, indiceDV: 9))
                    return false;
            }

            // Establecimiento: últimos 3 dígitos >= 001
            if (!EstablecimientoValido(val.Substring(10, 3)))
                return false;

            return true;
        }

        public static bool ValidarRUCPublica(string input)
        {
            var val = SoloDigitos(input);
            if (val.Length != 13 || !EsProvinciaValida(val.Substring(0, 2)))
                return false;

            //// Tercer dígito = 6 (sector público)
            //if (val[2] != '6')
            //    return false;

            // Dígito verificador (posición 9) con módulo 11 usando 8 coeficientes
            // Coeficientes oficiales (pos 1..8): 3,2,7,6,5,4,3,2
            int[] coef = { 3, 2, 7, 6, 5, 4, 3, 2 };
            if (!Mod11_Verifica(val, 8, coef, indiceDV: 8))
                return false;

            // Sufijo: último establecimiento público debe terminar en "0001"
            if (!val.EndsWith("0001"))
                return false;

            return true;
        }


        // ---------------------------
        // Helpers
        // ---------------------------

        private static string SoloDigitos(string s)
            => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

        private static bool EsProvinciaValida(string dosDigitos)
        {
            if (dosDigitos.Length != 2 || !dosDigitos.All(char.IsDigit))
                return false;
            int prov = int.Parse(dosDigitos);
            // Provincias 01..24
            return prov >= 1 && prov <= 24;
        }

        private static bool Entre(char c, char min, char max) => c >= min && c <= max;

        private static bool EstablecimientoValido(string tresDigitos)
        {
            if (tresDigitos.Length != 3 || !tresDigitos.All(char.IsDigit))
                return false;
            // Reglas SRI: 001..999 (se invalida "000")
            return int.Parse(tresDigitos) >= 1;
        }

        // Cédula: módulo 10 sobre los primeros 9 dígitos; el 10º es DV
        private static bool Mod10_Cedula(string diezDigitos)
        {
            if (diezDigitos.Length != 10 || !diezDigitos.All(char.IsDigit))
                return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int d = diezDigitos[i] - '0';
                // Posiciones impares (1,3,5,7,9) -> i = 0,2,4,6,8: x2 y resta 9 si > 9
                if ((i % 2) == 0)
                {
                    int prod = d * 2;
                    if (prod > 9) prod -= 9;
                    suma += prod;
                }
                else
                {
                    suma += d;
                }
            }

            int decenaSuperior = ((suma + 9) / 10) * 10;
            int dv = decenaSuperior - suma;
            if (dv == 10) dv = 0;

            int dvReal = diezDigitos[9] - '0';
            return dv == dvReal;
        }

        // RUC privada/pública: módulo 11
        // longitudBase = 9 (privada) o 8 (pública)
        // coeficientes: ver cada caso
        // indiceDV: índice cero-based del dígito verificador (9 para privada, 8 para pública)
        private static bool Mod11_Verifica(string val, int longitudBase, int[] coeficientes, int indiceDV)
        {
            if (!val.All(char.IsDigit) || coeficientes.Length != longitudBase)
                return false;

            int suma = 0;
            for (int i = 0; i < longitudBase; i++)
            {
                int d = val[i] - '0';
                suma += d * coeficientes[i];
            }

            int residuo = suma % 11;
            int dv = 11 - residuo;
            if (dv == 11) dv = 0;
            if (dv == 10) return false; // inválido

            int dvReal = val[indiceDV] - '0';
            return dv == dvReal;
        }

        //JCHID excepcion cuando el numero de digitos de RUC  es mayor a 6 
        private static bool TieneNumeroSecuencialLargo(string numeroSecuencial)
        {
            string sinCerosIniciales = numeroSecuencial.TrimStart('0');
            if (string.IsNullOrEmpty(sinCerosIniciales))
                sinCerosIniciales = "0";

            return sinCerosIniciales.Length > 6;
        }

        //public static bool ValidarCedula(string value)
        //{
        //    try
        //    {
        //        Init();
        //        if (value.Length < 10 || value.Length > 10)
        //            return false;
        //        /*^[0-9]{10,13}$*/
        //        Regex reg = new Regex("^[0-9]{10,13}$");
        //        if (!reg.IsMatch(value))
        //        {
        //            return false;
        //        }
        //        var numeroProvincia = Convert.ToInt32(value.Substring(0, 2));
        //        if ((numeroProvincia >= 1 && numeroProvincia <= (int)Atributos_identificacion["cedula"]["nroProvincias"]))
        //        {
        //            string ultimosdigitos = Atributos_identificacion["rucprivada"]["ultimosdigitos"].ToString();
        //            string newvalue = value;
        //            if (value.Length == 13)
        //            {
        //                newvalue = value.Substring(0, 10);
        //            }
        //            if (!isValidDigitoVerificador(newvalue, Atributos_identificacion["cedula"]["coeficiente"] as List<int>
        //                , (int)Atributos_identificacion["cedula"]["modulo"]
        //                , (bool)Atributos_identificacion["cedula"]["sumar_digitos"]))
        //            {
        //                return false;
        //            }
        //            else
        //                return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public static bool ValidarRUCPrivada(string value)
        //{
        //    try
        //    {
        //        Init();
        //        if (value.Length != 13)
        //            return false;
        //        Regex reg = new Regex("^[0-9]{10,13}$");
        //        if (!reg.IsMatch(value))
        //        {
        //            return false;
        //        }
        //        foreach (KeyValuePair<string, Dictionary<string, object>> o in Atributos_identificacion)
        //        {
        //            var newValue = value.Substring(0, value.Length - o.Value["ultimosdigitos"].ToString().Length);
        //            var es_valido = isValidDigitoVerificador(newValue, o.Value["coeficiente"] as List<int>, (int)o.Value["modulo"],
        //            (bool)o.Value["sumar_digitos"]);
        //            List<int> verificadores = (List<int>)o.Value["3digito"];
        //            int tercer_digito = int.Parse(value[2].ToString());
        //            int inicio = (value.Length - o.Value["ultimosdigitos"].ToString().Length);
        //            var m = value.Substring(inicio, o.Value["ultimosdigitos"].ToString().Length);
        //            var ultimos = o.Value["ultimosdigitos"].ToString();
        //            if (es_valido && verificadores.Contains(tercer_digito) && m == ultimos)
        //                return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}

        //public static bool ValidarRUCPublica(string value)
        //{
        //    try
        //    {
        //        Init();

        //        if (value.Length < 13 || value.Length > 13)
        //            return false;
        //        Regex reg = new Regex("^[0-9]{10,13}$");
        //        if (!reg.IsMatch(value))
        //        {
        //            return false;
        //        }
        //        if (!((List<int>)Atributos_identificacion["rucpublica"]["3digito"]).Contains(int.Parse(value[2].ToString())))
        //            return false;
        //        string ultimosdigitos = Atributos_identificacion["rucprivada"]["ultimosdigitos"].ToString();
        //        string newvalue = value.Substring(0, 8);
        //        if (!isValidDigitoVerificador(newvalue, Atributos_identificacion["rucpublica"]["coeficiente"] as List<int>
        //            , (int)Atributos_identificacion["rucpublica"]["modulo"]
        //            , (bool)Atributos_identificacion["rucpublica"]["sumar_digitos"]))
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public static bool ValidarRUCNatural(string value)
        //{
        //    try
        //    {
        //        Init();

        //        long isNumeric;
        //        const int tamanoLongitudRuc = 13;
        //        string ultimosdigitos = Atributos_identificacion["rucprivada"]["ultimosdigitos"].ToString(); 
        //        if (long.TryParse(value, out isNumeric) && value.Length.Equals(tamanoLongitudRuc))
        //        {
        //            var numeroProvincia = Convert.ToInt32(value.Substring(0, 2));
        //            var personaNatural = Convert.ToInt32(value.Substring(2, 1));
        //            if ((numeroProvincia >= 1 && numeroProvincia <= (int)Atributos_identificacion["cedula"]["nroProvincias"]) 
        //                &&
        //                personaNatural >= 0 && personaNatural <= 6)
        //                //((List<int>)Atributos_identificacion["cedula"]["3digito"]).Contains(personaNatural))
        //            {
        //                return value.Substring(10, 3) == ultimosdigitos
        //                        &&
        //                        isValidDigitoVerificador(value.Substring(0, 10), Atributos_identificacion["cedula"]["coeficiente"] as List<int>
        //                                                , (int)Atributos_identificacion["cedula"]["modulo"]
        //                                                , (bool)Atributos_identificacion["cedula"]["sumar_digitos"]);
        //            }
        //            return false;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public static bool ValidarPasaporte(string value)
        {
            try
            {
                Init();

                var listaCaracteres = new List<string> {    "A", "P", "5",
                                                            "G", "0", "V",
                                                            "7", "X", "F",
                                                            "4", "9", "1",
                                                            "D", "Y",
                                                            "F",
                                                            "H", "B", "C",
                                                            "I", "E", "T"
                };

                var caracterVerificador = value.Substring(0, 1).ToUpper();

                if (listaCaracteres.Any(x => x.Equals(caracterVerificador)) && value.Length <= 11)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class Cedulas
    {
        /**
    * Numero de Provincias del Ecuador
    */
        private static int NUMERO_DE_PROVINCIAS = 24;//22;
        /**
     * Este método permite verificar si una cédula de identidad es verdadera
     * retorna true si es válida, caso contrario retorna false.
     * @param cedula Cédula de Identidad Ecuatoriana de 10 digitos.
     * @return Si es verdadera true, si es falsa false
     */

        public static bool esCedulaValida(string cedula)
        {
            Regex reg = new Regex("^[0-9]{10}$");
            //verifica que tenga 10 dígitos y que contenga solo valores numéricos
            if (!((cedula.Length >= 10) && reg.IsMatch(cedula.Substring(0, 10))))
            {
                return false;
            }

            //verifica que los dos primeros dígitos correspondan a un valor entre 1 y NUMERO_DE_PROVINCIAS
            int prov = int.Parse(cedula.Substring(0, 2));

            if (!((prov > 0) && (prov <= NUMERO_DE_PROVINCIAS)))
            {
                return false;
            }

            //verifica que el último dígito de la cédula sea válido
            int[] d = new int[10];

            //Asignamos el string a un array
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = int.Parse(cedula[i] + "");
            }

            int imp = 0;
            int par = 0;

            //sumamos los duplos de posición impar
            for (int i = 0; i < d.Length; i += 2)
            {
                d[i] = ((d[i] * 2) > 9) ? ((d[i] * 2) - 9) : (d[i] * 2);
                imp += d[i];
            }

            //sumamos los digitos de posición par
            for (int i = 1; i < (d.Length - 1); i += 2)
            {
                par += d[i];
            }

            //Sumamos los dos resultados
            int suma = imp + par;

            //Restamos de la decena superior
            int d10 = int.Parse((suma + 10).ToString().Substring(0, 1) +
                    "0") - suma;

            //Si es diez el décimo dígito es cero
            d10 = (d10 == 10) ? 0 : d10;

            //si el décimo dígito calculado es igual al digitado la cédula es correcta
            return d10 == d[9];
        }
    }

    public class ValidaRucSociedades
    {
        /**
         * @param args
         */
        private static int num_provincias = 24;

        //public static String rucPrueba = "1790011674001";
        private static int[] coeficientes = { 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        private static int constante = 11;

        public static Boolean validacionRUC(string ruc)
        {
            //verifica que los dos primeros dígitos correspondan a un valor entre 1 y NUMERO_DE_PROVINCIAS
            int prov = int.Parse(ruc.Substring(0, 2));

            if (!((prov > 0) && (prov <= num_provincias)))
            {
                return false;
            }

            //verifica que el último dígito de la cédula sea válido
            int[] d = new int[10];
            int suma = 0;

            //Asignamos el string a un array
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = int.Parse(ruc[i] + "");
            }

            for (int i = 0; i < d.Length - 1; i++)
            {
                d[i] = d[i] * coeficientes[i];
                suma += d[i];
            }

            int aux, resp;

            aux = suma % constante;
            resp = constante - aux;

            resp = (resp == 10) ? 0 : resp;

            if (resp == d[9])
            {
                return true;
            }
            else
                return false;
        }
    }

    public class ValidaRUCNatural
    {
        private static int num_provincias = 24;

        public static Boolean validacionCedula(string cedula)
        {
            //verifica que los dos primeros dígitos correspondan a un valor entre 1 y NUMERO_DE_PROVINCIAS
            int prov = int.Parse(cedula.Substring(0, 2));

            if (!((prov > 0) && (prov <= num_provincias)))
            {
                return false;
            }

            //verifica que el último dígito de la cédula sea válido
            int[] d = new int[10];
            //Asignamos el string a un array
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = int.Parse(cedula[i] + "");
            }

            int imp = 0;
            int par = 0;

            //sumamos los duplos de posición impar
            for (int i = 0; i < d.Length; i += 2)
            {
                if ((d[i] * 2) > 9)
                {
                    d[i] = (d[i] * 2) - 9;
                }
                else
                {
                    d[i] = (d[i] * 2);
                }
                imp += d[i];
            }

            //sumamos los digitos de posición par
            for (int i = 1; i < d.Length - 1; i += 2)
            {
                par += d[i];
            }

            //Sumamos los dos resultados
            int suma = imp + par;

            //Restamos de la decena superior
            int d10 = int.Parse((suma + 10).ToString().Substring(0, 1) + "0") - suma;

            //Si es diez el décimo dígito es cero
            d10 = (d10 == 10) ? 0 : d10;

            //si el décimo dígito calculado es igual al digitado la cédula es correcta
            if (d10 == d[9])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
