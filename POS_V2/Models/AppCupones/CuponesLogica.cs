using System;
using System.Data;
using System.Collections.Generic;
using POS.Models.AppCupones; // Tu modelo de datos
using System.Data.SqlClient;

namespace POS.Models.AppCupones
{
    public class CuponesLogica
    {
        public static CuponRespuesta ValidarCuponEnBD(string codigo, string idCliente, string idAlmacen)
        {
            CuponRespuesta respuesta = new CuponRespuesta();

            string connectionMark = "";
            if (Control.Common.GlobalParameters.ConServerMarketing != "")
            {
                connectionMark = Control.Common.GlobalParameters.ConServerMarketing;
            }
            else
            {
                respuesta.EsValido = false;
                respuesta.Mensaje = "No se encontró conexión al servidor de Marketing.";
                return respuesta;
            }
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();

            try
            {
                // 2. Construcción del Query
                sQuery = string.Concat(sQuery, "exec sp_ValidarCuponPOS ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @CodigoLeido = '{codigo}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @IdCliente = '{idCliente}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @IdAlmacen = '{idAlmacen}' ", Environment.NewLine);

                // 3. Ejecución
                dtsConsulta = Control.Common.General.GetDataSet(sQuery, connectionMark);

                // 4. Procesar Resultados
                if (dtsConsulta != null && dtsConsulta.Tables.Count > 0)
                {
                    // --- TABLA 0: Cabecera y Validación ---
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = dtsConsulta.Tables[0].Rows[0];

                        // Convertimos 1/0 a booleano
                        respuesta.EsValido = (row["EsValido"].ToString() == "1" || row["EsValido"].ToString().ToLower() == "true");
                        respuesta.Mensaje = row["Mensaje"].ToString();

                        // ---- NUEVA VALIDACIÓN DE CÓDIGO DE MENSAJE ----
                        if (dtsConsulta.Tables[0].Columns.Contains("CodigoMensaje") && row["CodigoMensaje"] != DBNull.Value)
                        {
                            respuesta.CodigoMensaje = Convert.ToInt32(row["CodigoMensaje"]);
                        }
                        else if (!respuesta.EsValido)
                        {
                            // Si la columna no existe y el cupón es inválido, asignamos el código genérico.
                            respuesta.CodigoMensaje = 10008;
                        }
                        // ----------------------------------------------------

                        if (respuesta.EsValido)
                        {
                            respuesta.IdCupon = Convert.ToInt32(row["IdCupon"]);
                            respuesta.Descripcion = row["Descripcion"].ToString();
                            respuesta.TipoDescuento = row["TipoDescuento"].ToString();
                            respuesta.ValorDescuento = Convert.ToDecimal(row["ValorDescuento"]);

                            // =======================================================
                            // NUEVO: LEER SI PERMITE COMBINAR
                            // =======================================================
                            // Verificamos si la columna vino de la BD para evitar errores
                            if (dtsConsulta.Tables[0].Columns.Contains("PermiteCombinar") && row["PermiteCombinar"] != DBNull.Value)
                            {
                                respuesta.PermiteCombinar = Convert.ToBoolean(row["PermiteCombinar"]);
                            }
                            else
                            {
                                // Valor por defecto si la BD no responde nada (True = Amigable)
                                respuesta.PermiteCombinar = true;
                            }
                            // =======================================================
                        }
                    }

                    // --- TABLA 1: Detalle de Alcance ---
                    if (respuesta.EsValido && dtsConsulta.Tables.Count > 1)
                    {
                        foreach (DataRow rowDetalle in dtsConsulta.Tables[1].Rows)
                        {
                            AlcanceCupon alcance = new AlcanceCupon();
                            alcance.TipoAlcance = rowDetalle["TipoAlcance"].ToString();
                            alcance.ValorAlcance = rowDetalle["ValorAlcance"].ToString();

                            respuesta.Alcance.Add(alcance);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "CuponesLogica",
                    "ValidarCuponEnBD",
                    $"Error al validar cupón: {ex.Message} - Query: {sQuery}"
                );

                respuesta.EsValido = false;
                respuesta.Mensaje = "Ocurrió un error al consultar el cupón.";
            }

            return respuesta;
        }

        public static void RegistrarUsoCupon(int idCupon, string idCliente, string numeroFactura)
        {
            try
            {
                string connectionMark = "";
                if (Control.Common.GlobalParameters.ConServerMarketing != "")
                {
                    connectionMark = Control.Common.GlobalParameters.ConServerMarketing;
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CuponesLogica", "RegistrarUsoCupon", "No se encontró conexión al servidor central.");
                    return;
                }

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionMark))
                {
                    conn.Open();

                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("sp_RegistrarUsoCupon", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdCupon", idCupon);
                        cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        cmd.Parameters.AddWithValue("@IdTransaccion", numeroFactura);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CuponesLogica", "RegistrarUsoCupon", "Error: " + ex.Message);
            }
        }
    }
}