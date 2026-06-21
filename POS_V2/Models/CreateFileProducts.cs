using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using POS.Models;
using POS.Control;
using System.Xml.Linq;

namespace POS
{    
    class CreateFileProducts
    {        
        protected string establecimiento_inicio;

        public static List<LeerProductosArchivo> productoslista = new List<LeerProductosArchivo>();
        public static List<ProductoArticulo> productoslistaXML = new List<ProductoArticulo>();
        public CreateFileProducts(string establecimiento)
        {
            this.establecimiento_inicio = establecimiento;
        }


        private bool GuardarXML(List<ProductoArticulo> productoArticulos, string fileText)
        {


            // Validar parámetros
            if (productoArticulos == null)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GuardarXML", "La lista de productos es nula.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(fileText))
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GuardarXML", "La ruta del archivo es nula o vacía.");
                return false;
            }



            try
            {

                // Asegurar que el directorio exista
                string directorio = Path.GetDirectoryName(fileText);
                if (!Directory.Exists(directorio))
                {
                    try
                    {
                        Directory.CreateDirectory(directorio);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GuardarXML", $"Carpeta creada: {directorio}");
                    }
                    catch (Exception exDir)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "GuardarXML", $"No se pudo crear el directorio: {exDir.Message}");
                        return false;
                    }
                }

                // Crear el XML
                XElement xml = new XElement("ProductoArticulos",
                    new XElement("CodError", 0),
                    new XElement("MsjError", ""),
                    new XElement("ContRegistro", productoArticulos.Count),
                    new XElement("Articulos",
                        productoArticulos.Select(a => new XElement("Articulo",
                            new XElement("ARTICULO", a.ARTICULO),
                            new XElement("BARRAS", a.BARRAS),
                            new XElement("CATEGORIA", a.CATEGORIA),
                            new XElement("ESTABLECIMIENTO", a.ESTABLECIMIENTO),
                            new XElement("ORDEN", a.ORDEN),
                            new XElement("campoConsulta", a.campoConsulta)
                        ))
                    )
                );

                // Guardar el archivo
                xml.Save(fileText);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GuardarXML", $"Archivo XML guardado exitosamente: {fileText}");
                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CreaXml", "Error: " + ex.Message);
                return false;
            }
        }

       

        public void CrearArchivoProductosXML()
        {
            string linea;
            string fileName = string.Empty;
            string campoConsulta = string.Empty;
            List<ProductoArticulo> ProductoArticuloList = new List<ProductoArticulo>();

            fileName = "ListaProducto.xml";
           
            string fileText = POS.Control.Common.GlobalParameters.ListaProductos_Path + fileName;

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CrearArchivoProductosXML", "Se ejecuta Hilo para crear archivo");

            bool validaFechas = (!(File.GetCreationTime(fileText).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString()
               || File.GetLastWriteTime(fileText).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString()));


            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"ListaProductos_Path {Control.Common.GlobalParameters.ListaProductos_Path}");


                if (POS.Control.Common.GlobalParameters.ListaProductos_Path_Valida.ToUpper() == "TRUE")
                {
                    if (!(Directory.Exists(POS.Control.Common.GlobalParameters.ListaProductos_Path)))
                    {
                        Directory.CreateDirectory(POS.Control.Common.GlobalParameters.ListaProductos_Path);
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"");
                    if (!File.Exists(fileText))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"fileText {fileText}");

                        var objProducto = Control.Common.General.GetListConsultaProducto(establecimiento_inicio, "", "");


                        var listProducto = objProducto.ProductoArticuloList.ToList();
                        ProductoArticuloList = listProducto;

                        if (objProducto.CodError == 0)
                        {
                            var articulos = (from deta in listProducto
                                             select deta)
                                            .OrderBy(producto => producto.ORDEN)
                                            .ThenBy(prod => prod.ARTICULO).ToList();

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"Ejecuta GuardarXML");
                            var archivoCreado = GuardarXML(articulos, fileText);                          
                        }

                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"Ejecuta LeerXMLLista");
                        var ListaArticulosXML = LeerXMLLista(fileText, establecimiento_inicio, campoConsulta);

                        if (validaFechas)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"Valida fechas y ejecuta EliminarArchivoXML");

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"fileText {fileText}");
                            POS.Control.Common.General.EliminarArchivoXML(fileText);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"ejecuta GetListConsultaProducto");
                            var objProducto = Control.Common.General.GetListConsultaProducto(establecimiento_inicio, "", "");
                            var listProducto = objProducto.ProductoArticuloList.ToList();
                            ProductoArticuloList = objProducto.ProductoArticuloList.ToList();

                            if (objProducto.CodError == 0)
                            {
                                var articulos = (from deta in listProducto
                                                 select deta)
                                                .OrderBy(producto => producto.ORDEN)
                                                .ThenBy(prod => prod.ARTICULO).ToList();


                                var archivoCreado = GuardarXML(articulos, fileText);

                            }

                            Control.Common.GlobalParameters.ProductoArticuloList = objProducto.ProductoArticuloList.ToList();
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CrearArchivoProductosXML", $"Ejecuta AgregarRegistrosFaltantes");
                            AgregarRegistrosFaltantes(fileText, ListaArticulosXML);

                            
                        }
                    }

                    productoslistaXML = new List<ProductoArticulo>();
                    var productoArticulos = LeerXMLLista(fileText, establecimiento_inicio, campoConsulta);

                    productoslistaXML.AddRange(productoArticulos);
                }
                Control.Common.GlobalParameters.ProductoArticuloList = new List<ProductoArticulo>();
                Control.Common.GlobalParameters.ProductoArticuloList = productoslistaXML.ToList();
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CrearArchivoProductosXML", $"Error: {ex.Message}");

            }

        }

        public static List<ProductoArticulo> LeerXMLLista(string rutaXML, string establecimiento, string campoConsulta)
        {
            if (!File.Exists(rutaXML))
            {
                Console.WriteLine("El archivo XML no existe.");
                return new List<ProductoArticulo>();
            }

            List<ProductoArticulo> lista = new List<ProductoArticulo>();

            XElement xmlFile = XElement.Load(rutaXML);
            var articulos = xmlFile.Element("Articulos")?.Elements("Articulo");

            if (articulos != null)
            {
                foreach (var elemento in articulos)
                {
                    ProductoArticulo articulo = new ProductoArticulo
                    {
                        ARTICULO = (string)elemento.Element("ARTICULO"),
                        BARRAS = (string)elemento.Element("BARRAS"),
                        CATEGORIA = (string)elemento.Element("CATEGORIA"),
                        ESTABLECIMIENTO = (string)elemento.Element("ESTABLECIMIENTO"),
                        ORDEN = (int?)elemento.Element("ORDEN") ?? 0,
                        campoConsulta = (string)elemento.Element("campoConsulta")
                    };
                    lista.Add(articulo);
                }

            }
            

            return lista ?? new List<ProductoArticulo>();
        }


     
        public static void AgregarRegistrosFaltantes(string rutaXML, List<ProductoArticulo> baseRegistros)
        {
            XElement xmlFile = null;

            try
            {
                xmlFile = XElement.Load(rutaXML);
               
            }
            catch (Exception ex)
            {
                // Si no existe archivo XML, crear uno nuevo con los registros base
                ProductoArticulos nuevo = new ProductoArticulos
                {
                    CodError = 0,
                    MsjError = "Archivo creado nuevo",
                    ContRegistro = baseRegistros.Count,
                    ProductoArticuloList = baseRegistros
                };
                GuardarXML(rutaXML, nuevo);
                Console.WriteLine("Archivo XML creado con registros base.");
                return;
            }

            var articulosXml = xmlFile.Element("Articulos")?.Elements("Articulo")
              .Select(x => new ProductoArticulo
              {
                  ARTICULO = (string)x.Element("ARTICULO"),
                  BARRAS = (string)x.Element("BARRAS"),
                  CATEGORIA = (string)x.Element("CATEGORIA"),
                  ESTABLECIMIENTO = (string)x.Element("ESTABLECIMIENTO"),
                  ORDEN = (int?)x.Element("ORDEN") ?? 0,
                  campoConsulta = (string)x.Element("campoConsulta")
              }).ToList() ?? new List<ProductoArticulo>();


            var faltantes = baseRegistros
               .Where(b => !articulosXml.Any(x => x.ARTICULO == b.ARTICULO))
               .ToList();

            if (faltantes.Count == 0)
            {
                Console.WriteLine("No hay registros faltantes para agregar.");
                return;
            }


            var articulosElement = xmlFile.Element("Articulos");
            if (articulosElement == null)
            {
                articulosElement = new XElement("Articulos");
                xmlFile.Add(articulosElement);
            }

            foreach (var faltante in faltantes)
            {
                XElement nuevoArticulo = new XElement("Articulo",
                    new XElement("ARTICULO", faltante.ARTICULO),
                    new XElement("BARRAS", faltante.BARRAS),
                    new XElement("CATEGORIA", faltante.CATEGORIA),
                    new XElement("ESTABLECIMIENTO", faltante.ESTABLECIMIENTO),
                    new XElement("ORDEN", faltante.ORDEN),
                    new XElement("campoConsulta", faltante.campoConsulta)
                );
                articulosElement.Add(nuevoArticulo);
            }

            // Actualizar ContRegistro
            xmlFile.SetElementValue("ContRegistro", articulosXml.Count + faltantes.Count);

            xmlFile.Save(rutaXML);

            Console.WriteLine($"Se agregaron {faltantes.Count} registros faltantes al XML.");


        }

        // Método auxiliar para guardar un ProductoArticulos en XML
        public static void GuardarXML(string ruta, ProductoArticulos productoArticulos)
        {
            XElement xml = new XElement("ProductoArticulos",
                new XElement("CodError", productoArticulos.CodError),
                new XElement("MsjError", productoArticulos.MsjError),
                new XElement("ContRegistro", productoArticulos.ContRegistro),
                new XElement("Articulos",
                    new List<XElement>(
                        productoArticulos.ProductoArticuloList.ConvertAll(a => new XElement("Articulo",
                            new XElement("ARTICULO", a.ARTICULO),
                            new XElement("BARRAS", a.BARRAS),
                            new XElement("CATEGORIA", a.CATEGORIA),
                            new XElement("ESTABLECIMIENTO", a.ESTABLECIMIENTO),
                            new XElement("ORDEN", a.ORDEN),
                            new XElement("campoConsulta", a.campoConsulta)
                        ))
                    )
                )
            );

            xml.Save(ruta);
        }



        public static ProductoArticulos LeerXMLCompleto(string rutaArchivo)
        {
            ProductoArticulos resultado = new ProductoArticulos();

            try
            {
                XElement xmlFile = XElement.Load(rutaArchivo);

                resultado.CodError = (int?)xmlFile.Element("CodError") ?? 0;
                resultado.MsjError = (string)xmlFile.Element("MsjError") ?? "";
                resultado.ContRegistro = (int?)xmlFile.Element("ContRegistro") ?? 0;

                var articulos = xmlFile.Element("Articulos")?.Elements("Articulo")
                    .Select(x => new ProductoArticulo
                    {
                        ARTICULO = (string)x.Element("ARTICULO"),
                        BARRAS = (string)x.Element("BARRAS"),
                        CATEGORIA = (string)x.Element("CATEGORIA"),
                        ESTABLECIMIENTO = (string)x.Element("ESTABLECIMIENTO"),
                        ORDEN = (int?)x.Element("ORDEN") ?? 0,
                        campoConsulta = (string)x.Element("campoConsulta")
                    }).ToList();

                resultado.ProductoArticuloList = articulos ?? new List<ProductoArticulo>();
            }
            catch (Exception ex)
            {
                resultado.CodError = -1;
                resultado.MsjError = ex.Message;
                resultado.ProductoArticuloList = new List<ProductoArticulo>();
                resultado.ContRegistro = 0;
            }

            return resultado;
        }



        public void CrearArchivoProductos()
        {
            string linea;
            string fileName = string.Empty;
            fileName = POS.Control.Common.GlobalParameters.ListaProductos_File;
            string fileText = POS.Control.Common.GlobalParameters.ListaProductos_Path + fileName;

            try
            {
                if (POS.Control.Common.GlobalParameters.ListaProductos_Path_Valida.ToUpper() == "TRUE")
                {
                    if (!(Directory.Exists(POS.Control.Common.GlobalParameters.ListaProductos_Path)))
                    {
                        Directory.CreateDirectory(POS.Control.Common.GlobalParameters.ListaProductos_Path);
                    }

                    if (!File.Exists(fileText))
                    {
                        POSEntities db = new POSEntities();
                        var buffer = new StringBuilder();

                        (from tran in db.VW_SEARCHPRODUCT // .VW_SEARCHRODUCT
                         where tran.ESTABLECIMIENTO == establecimiento_inicio
                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).ToList().ForEach(product =>
                         buffer.AppendLine(String.Format("{0}|{1}|{2}|{3}|{4}", product.ARTICULO.Replace("\r\n", ""), product.BARRAS.Replace("\r\n", "")
                         , product.CATEGORIA.Replace("\r\n", ""), product.ESTABLECIMIENTO.Replace("\r\n", ""), product.ORDEN)));
                        File.WriteAllText(fileText, buffer.ToString());
                    }
                    else if (File.Exists(fileText))
                    {
                       bool validaFechas = (!(File.GetCreationTime(fileText).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString() 
                       || File.GetLastWriteTime(fileText).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString()));

                        if (validaFechas)
                        {
                            POSEntities db = new POSEntities();
                            var buffer = new StringBuilder();

                            (from tran in db.VW_SEARCHPRODUCT // .VW_SEARCHRODUCT
                             where tran.ESTABLECIMIENTO == establecimiento_inicio
                             select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).ToList().ForEach(product =>
                             buffer.AppendLine(String.Format("{0}|{1}|{2}|{3}|{4}", product.ARTICULO.Replace("\r\n", ""), product.BARRAS.Replace("\r\n", "")
                             , product.CATEGORIA.Replace("\r\n", ""), product.ESTABLECIMIENTO.Replace("\r\n", ""), product.ORDEN)));
                            File.WriteAllText(fileText, buffer.ToString());
                        }
                        else
                        {
                            int countLineFile = 0;
                            int countDB = 0;
                            using (StreamReader sr = new StreamReader(fileText))
                            {
                                while ((linea = sr.ReadLine()) != null)
                                {
                                  countLineFile++;
                                }
                            }

                            POSEntities db = new POSEntities();
                            countDB = (from tran in db.VW_SEARCHPRODUCT // .VW_SEARCHRODUCT
                                         where tran.ESTABLECIMIENTO == establecimiento_inicio
                                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Count();

                            if (!(countLineFile == countDB))
                            {
                                var buffer = new StringBuilder();

                                (from tran in db.VW_SEARCHPRODUCT // .VW_SEARCHRODUCT
                                 where tran.ESTABLECIMIENTO == establecimiento_inicio
                                 select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).ToList().ForEach(product =>
                                 buffer.AppendLine(String.Format("{0}|{1}|{2}|{3}|{4}", product.ARTICULO.Replace("\r\n", ""), product.BARRAS.Replace("\r\n", "")
                                 , product.CATEGORIA.Replace("\r\n", ""), product.ESTABLECIMIENTO.Replace("\r\n", ""), product.ORDEN)));
                                File.WriteAllText(fileText, buffer.ToString());
                            }
                        }
                        
                    }

                    using (StreamReader ReaderObject = new StreamReader(fileText))
                    {
                        while ((linea = ReaderObject.ReadLine()) != null)
                        {
                            string[] leer = linea.Split('|');

                            productoslista.Add(new LeerProductosArchivo()
                            {
                                ARTICULO = leer[0],
                                BARRAS = leer[1],
                                CATEGORIA = leer[2],
                                ESTABLECIMIENTO = leer[3],
                                ORDEN = Convert.ToInt32(leer[4])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "insertaCabeceraFile", "No fue posible guardar los datos de cabecera de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //MessageBox.Show(this, ex.Message);
            }
        }
      
    }
}
