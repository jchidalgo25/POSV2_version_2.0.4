using System;
using System.Data.Entity;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Net; // Necesario para obtener el nombre de la PC

namespace POS.Models // <--- ¡VERIFICA QUE ESTE NAMESPACE SEA CORRECTO!
{
    public partial class POSEntities : DbContext
    {
        // Constructor vacío que interceptamos
        public POSEntities()
            : base(ObtenerCadenaDeConexionDinamica())
        {
        }

        // Constructor con cadena manual (por si acaso)
        public POSEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        // --- LÓGICA PRINCIPAL DE CONEXIÓN ---
        private static string ObtenerCadenaDeConexionDinamica()
        {
            try
            {
                // 1. Leemos la cadena "plantilla" del App.config
                string cadenaOriginal = ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString;

                // 2. Buscamos la IP correcta para esta máquina
                string ipReal = DescubrirIpServidor();

                // 3. Reemplazamos el comodín [ipserver]
                string cadenaConIp = cadenaOriginal.Replace("[ipserver]", ipReal);

                // 4. Forzamos las credenciales correctas (Evita errores de login)
                var entityBuilder = new EntityConnectionStringBuilder(cadenaConIp);
                var sqlBuilder = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString);

                sqlBuilder.DataSource = ipReal;
                sqlBuilder.UserID = "svc_sql_pos";
                sqlBuilder.Password = "a17472Ol0";
                sqlBuilder.IntegratedSecurity = false;

                entityBuilder.ProviderConnectionString = sqlBuilder.ToString();
                return entityBuilder.ToString();
            }
            catch (Exception)
            {
                return "name=POSEntities";
            }
        }

        // --- LÓGICA DE BÚSQUEDA DE IP (BLINDADA CONTRA ERRORES) ---
        private static string DescubrirIpServidor()
        {
            // IP de Respaldo (Si todo falla, intenta conectar a QA)
            string ipResultado = "192.168.127.162";
            string rutaArchivo = @"C:\Log\POSConexiones.txt";

            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string[] lineas = File.ReadAllLines(rutaArchivo);
                    string nombrePC = Dns.GetHostName().ToUpper(); // Ej: "CAJA01-ALB"

                    foreach (string lineaRaw in lineas)
                    {
                        if (string.IsNullOrWhiteSpace(lineaRaw)) continue;

                        string linea = lineaRaw.Trim();

                        // -------------------------------------------------------
                        // CASO 1: MODO QA (Tu prueba actual)
                        // Busca explícitamente la línea "QA TESTING IP"
                        // -------------------------------------------------------
                        if (linea.ToUpper().Contains("QA TESTING IP") && linea.Contains("|"))
                        {
                            string[] partes = linea.Split('|');
                            if (partes.Length > 1)
                            {
                                string ip = partes[1].Trim();
                                if (EsIpValida(ip)) return ip;
                            }
                        }

                        // -------------------------------------------------------
                        // CASO 2: MODO PRODUCCIÓN (Archivo Limpio - Recomendado)
                        // Si la línea NO tiene barra '|' y parece una IP, úsala.
                        // Esto sirve si en la tienda dejas un archivo solo con la IP "192.168.10.7"
                        // -------------------------------------------------------
                        if (!linea.Contains("|") && EsIpValida(linea))
                        {
                            return linea;
                        }

                        // -------------------------------------------------------
                        // CASO 3: MODO PRODUCCIÓN (Archivo Maestro - Opcional)
                        // Si copiaste la lista gigante, intentamos ver si el nombre de 
                        // esta computadora aparece en la línea.
                        // -------------------------------------------------------
                        /* Descomenta esto si tus PCs se llaman igual que en la lista (Ej: "ALBORADA")
                           if (linea.ToUpper().Contains(nombrePC) && linea.Contains("|"))
                           {
                               string[] partes = linea.Split('|');
                               if (partes.Length > 1 && EsIpValida(partes[1].Trim())) return partes[1].Trim();
                           }
                        */
                    }
                }
            }
            catch
            {
                // Ignorar errores de lectura y usar la de respaldo
            }

            return ipResultado;
        }

        // Función auxiliar para validar que no sea texto basura y evitar error 128
        private static bool EsIpValida(string ip)
        {
            // Una IP debe tener puntos, ser mayor a 6 caracteres y menor a 20
            return ip.Contains(".") && ip.Length > 6 && ip.Length < 20;
        }
    }
}