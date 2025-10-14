using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using IntegracionPedidos;

namespace WcfIntegDelivery
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public string IntegrationDelivery(string Xmlreq)
        {
            System.Xml.Linq.XDocument xDoc = System.Xml.Linq.XDocument.Parse(Xmlreq);
            string xml = "";
            var MetodoObj = (from d in xDoc.Descendants("Root")
                             select new
                             {
                                 Integ = d.Attribute("Integ").Value
                             }).FirstOrDefault();
            if (MetodoObj.Integ == "YA")
            {
                Integracion integracionYa = new Integracion();
                xml = integracionYa.Integration(Xmlreq);
            }
            return xml;
        }

    }
}
