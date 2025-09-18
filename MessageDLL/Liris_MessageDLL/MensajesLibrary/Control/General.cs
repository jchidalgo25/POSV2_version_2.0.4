using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Liris_MenssageDLL.Model;

namespace Liris_MenssageDLL.Control
{
    public class General
    {
        public static string DBIdCaja = string.Empty;
        public static string ID_Caja_POS = string.Empty;

        public General(string _DBIdCaja, string _ID_Caja_POS)
        {
            DBIdCaja = _DBIdCaja;
            ID_Caja_POS = _ID_Caja_POS;
        }


        public static void AgregaMensajesFaltantes(string rutaXML, List<Mensajes> baseRegistros)
        {
            XElement xmlFile = null;

            try
            {
                xmlFile = XElement.Load(rutaXML);

            }
            catch (Exception ex)
            {

            }



            var DeMensajesXml = xmlFile.Element("Mensajes")?.Elements("Mensaje")
              .Select(x => new Mensajes
              {
                  id = (int)x.Element("id"),
                  descripcion = (string)x.Element("descripcion"),
                  opcion = (string)x.Element("opcion"),
                  titulo_mensaje = (string)x.Element("titulo_mensaje"),
                  tipo_mensaje = (string)x.Element("tipo_mensaje"),
                  tiempo_espera = (int?)x.Element("tiempo_espera") ?? 0,
                  activo = (bool)x.Element("activo"),
                  fecha_creacion = (DateTime)x.Element("fecha_creacion"),
                  fecha_modificacion = (DateTime)x.Element("fecha_modificacion"),
              }).ToList() ?? new List<Mensajes>();


            var faltantes = baseRegistros
               .Where(b => !DeMensajesXml.Any(x => x.id == b.id))
               .ToList();

            if (DeMensajesXml.Count == 0)
            {
                Console.WriteLine("No hay registros faltantes para agregar.");
                return;
            }


            var articulosElement = xmlFile.Element("Mensajes");
            if (articulosElement == null)
            {
                articulosElement = new XElement("Mensajes");
                xmlFile.Add(articulosElement);
            }


            foreach (var faltante in faltantes)
            {
                XElement nuevoArticulo = new XElement("Mensaje",
                    new XElement("id", faltante.id),
                    new XElement("descripcion", faltante.descripcion),
                    new XElement("opcion", faltante.opcion),
                    new XElement("titulo_mensaje", faltante.titulo_mensaje),
                    new XElement("tipo_mensaje", faltante.tipo_mensaje),
                    new XElement("tiempo_espera", faltante.tiempo_espera),
                    new XElement("activo", faltante.activo),
                    new XElement("fecha_creacion", faltante.fecha_creacion),
                    new XElement("fecha_modificacion", faltante.fecha_modificacion)

                );
                articulosElement.Add(nuevoArticulo);
            }

            // Actualizar ContRegistro
            xmlFile.SetElementValue("ContRegistro", DeMensajesXml.Count + faltantes.Count);
            xmlFile.Save(rutaXML);

            Console.WriteLine($"Se agregaron {faltantes.Count} registros faltantes al XML.");


        }
        public static List<Mensajes> LeerXMLDetMensajes(string rutaXML, string establecimiento, string campoConsulta)
        {
            if (!File.Exists(rutaXML))
            {
                Console.WriteLine("El archivo XML no existe.");
                return new List<Mensajes>();
            }

            List<Mensajes> lista = new List<Mensajes>();

            XElement xmlFile = XElement.Load(rutaXML);
            var articulos = xmlFile.Element("Mensajes")?.Elements("Mensaje");

            if (articulos != null)
            {
                foreach (var elemento in articulos)
                {
                    Mensajes articulo = new Mensajes
                    {
                        id = (int)elemento.Element("id"),
                        descripcion = (string)elemento.Element("descripcion"),
                        opcion = (string)elemento.Element("descripcion"),
                        titulo_mensaje = (string)elemento.Element("titulo_mensaje"),
                        tipo_mensaje = (string)elemento.Element("tipo_mensaje"),
                        tiempo_espera = (int)elemento.Element("tiempo_espera"),
                        activo = (bool)elemento.Element("activo"),
                        fecha_creacion = (DateTime)elemento.Element("fecha_creacion"),
                        fecha_modificacion = (DateTime)elemento.Element("fecha_modificacion"),

                    };
                    lista.Add(articulo);
                }
            }


            return lista ?? new List<Mensajes>();
        }
        public static void AgregaDetalleMensajesTmpFile(List<Mensajes> ListMensajes)
        {
            string tipoDet = string.Empty;
            string rutaInsert = @"C:\Log\POSInserts\";

            tipoDet = "DetMensajes.txt";
            string textFile = DBIdCaja + ID_Caja_POS + "DetMensajes.txt";
            string rutaCompleta = string.Concat(rutaInsert, textFile);


            try
            {
                //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                //textFile = ConectividadSharedTmpFile(GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }

                var buffer = new StringBuilder();

                (from detMensaje in ListMensajes
                 select detMensaje).ToList().ForEach(deta =>
                        buffer.AppendLine(String.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}"
                        , deta.id, deta.descripcion, deta.titulo_mensaje, deta.tipo_mensaje, deta.tiempo_espera, deta.activo, deta.TipoMensaje)));
                File.WriteAllText(rutaCompleta, buffer.ToString());



            }
            catch (Exception ex)
            {
              
            }
        }


    }
}
