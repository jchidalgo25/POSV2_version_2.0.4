using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.Common
{
    public static class StringHelper
    {
        public static string ToTitleCase(string s)
        {
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        public static string DevolverConPadding(string texto, int maxPadding, int lengthMaxEsperado = 8, bool isLeft = true)
        {
            var valorPadding = maxPadding;
            var addToPadding = lengthMaxEsperado - texto.Length - 1;

            valorPadding += addToPadding >= 0 ? addToPadding : 0;

            if (isLeft)
                return texto.PadLeft(valorPadding, ' ');
            else
                return texto.PadRight(valorPadding, ' ');

        }

        public static string ToLimitedLength(string text, int maxLength)
        {
            string response = string.Empty;
            text = text.Trim();

            if (maxLength <= 0) return text;

            //Separar las cadenas por saltos de linea si hubiere
            string[] textSplit = text.Split('\n');

            foreach (string cadena in textSplit)
            {
                if (response.Length > 0) response += Environment.NewLine;
                //Variable adicional xq no se puede editar directamente las cadenas del array estando en bucle
                string texto = cadena;
                while (texto.Length > 0)
                {
                    if (texto.Length > maxLength)
                    {
                        response += texto.Substring(0, maxLength) + Environment.NewLine;
                        texto = texto.Replace(texto.Substring(0, maxLength), "");
                    }
                    else
                    {
                        response += texto;
                        texto = string.Empty;
                    }
                }
            }

            return response;
        }

        public static string ToAlphaNumeric(string text, bool permiteEspecialesEspañol = false)
        {
            try
            {
                string response = text;

                if (!permiteEspecialesEspañol)
                {
                    //Quitar acentos
                    byte[] tempBytes;
                    tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(response);
                    response = System.Text.Encoding.UTF8.GetString(tempBytes);
                }

                //Quitar caracteres especiales
                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9- " + (permiteEspecialesEspañol ? "ñÑáéíóúü" : string.Empty) + "]");
                response = rgx.Replace(response, "");

                return response;
            }
            catch (Exception)
            {
                return text;
            }
        }

        public static string GetSplitContent(string text, int wantedIndex, char delimiter)
        {
            try
            {
                var splitContent = text.Split(delimiter);

                return splitContent[wantedIndex].Trim();
            }
            catch (Exception)
            {
                return text;
            }
        }


    }
}
