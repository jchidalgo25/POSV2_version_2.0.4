using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using POS.Models;

namespace POS.Control.XML
{
    public class FacturaXML
    {

       
        public static string generarFacturaXML(Factura factura)
        {
            var documento = new XmlDocument();
            XmlDeclaration declaracion = documento.CreateXmlDeclaration("1.0", "UTF-8", null);
            documento.AppendChild(declaracion);
            XmlElement documentoxml = documento.CreateElement("factura");
            documentoxml.SetAttribute("version", "2_00");
            documento.AppendChild(documentoxml);
            XmlElement cabeceraxml = documento.CreateElement("infoTributaria");
            documentoxml.AppendChild(cabeceraxml);
            XmlElement detallesxml = documento.CreateElement("detalles");
            documentoxml.AppendChild(detallesxml);
            XmlElement infoadicionalxml = documento.CreateElement("infoAdicional");
            documentoxml.AppendChild(infoadicionalxml);

            XmlElement razonsocial = documento.CreateElement("razonSocial");
            razonsocial.InnerText = "LIRIS S.A";
            cabeceraxml.AppendChild(razonsocial);
            XmlElement ruc = documento.CreateElement("ruc");
            ruc.InnerText = "0990865477001";
            cabeceraxml.AppendChild(ruc);
            XmlElement numAut = documento.CreateElement("numAut");
            numAut.InnerText = factura.Autorizacion;
            cabeceraxml.AppendChild(numAut);
            XmlElement codDoc = documento.CreateElement("codDoc");
            codDoc.InnerText = "1";
            cabeceraxml.AppendChild(codDoc);
            XmlElement estab = documento.CreateElement("estab");
            estab.InnerText = factura.Establecimiento;
            cabeceraxml.AppendChild(estab);
            XmlElement ptoEmi = documento.CreateElement("ptoEmi");
            ptoEmi.InnerText = factura.PtoEmision;
            cabeceraxml.AppendChild(ptoEmi);
            XmlElement secuencial = documento.CreateElement("secuencial");
            secuencial.InnerText = factura.Secuencia.ToString();
            cabeceraxml.AppendChild(secuencial);
            XmlElement fechaAutorizacion = documento.CreateElement("fechaAutorizacion");
            fechaAutorizacion.InnerText = factura.Fecha_inicio_autorizacion.ToString("dd/MM/yyyy");
            cabeceraxml.AppendChild(fechaAutorizacion);

            XmlElement caducidad = documento.CreateElement("caducidad");
            caducidad.InnerText = factura.Fecha_fin_autorizacion.ToString("dd/MM/yyyy");
            cabeceraxml.AppendChild(caducidad);
            XmlElement fechaEmision = documento.CreateElement("fechaEmision");
            fechaEmision.InnerText = factura.Fecha.ToString("dd/MM/yyyy");
            cabeceraxml.AppendChild(fechaEmision);
            XmlElement dirMatriz = documento.CreateElement("dirMatriz");
            dirMatriz.InnerText = "KM 5 1/2 VIA DURAN - BABAHOYO";
            cabeceraxml.AppendChild(dirMatriz);
            XmlElement razonSocialComprador = documento.CreateElement("razonSocialComprador");
            razonSocialComprador.InnerText = factura.Cliente_nombre;
            cabeceraxml.AppendChild(razonSocialComprador);

            if (factura.ClienteIdentificacion!="9999999999999")
            {
                XmlElement rucCedulaComprador = documento.CreateElement("rucCedulaComprador");
                rucCedulaComprador.InnerText = factura.ClienteIdentificacion;
                cabeceraxml.AppendChild(rucCedulaComprador);
            }

            XmlElement contribuyenteEspecial = documento.CreateElement("contribuyenteEspecial");
            contribuyenteEspecial.InnerText = "02239";
            cabeceraxml.AppendChild(contribuyenteEspecial);

            if (factura.ClienteIdentificacion != "9999999999999")
            {
                XmlElement totalSinImpuestos = documento.CreateElement("totalSinImpuestos");
                totalSinImpuestos.InnerText = factura.getSubTotal().ToString("N2");
                cabeceraxml.AppendChild(totalSinImpuestos);
                XmlElement baseIVA0 = documento.CreateElement("baseIVA0");
                baseIVA0.InnerText = factura.GetBase0().ToString("N2");
                cabeceraxml.AppendChild(baseIVA0);
                XmlElement baseIVA12 = documento.CreateElement("baseIVA12");
                baseIVA12.InnerText = factura.GetBase12().ToString("N2");
                cabeceraxml.AppendChild(baseIVA12);
                XmlElement IVA12 = documento.CreateElement("IVA12");
                IVA12.InnerText = factura.getIVA().ToString("N2");
                cabeceraxml.AppendChild(IVA12);
            }
            XmlElement totalConImpuestos = documento.CreateElement("totalConImpuestos");
            totalConImpuestos.InnerText = factura.GetTotal().ToString("N2");
            cabeceraxml.AppendChild(totalConImpuestos);
            foreach (var det in factura.Productos)
            {
                XmlElement detalle =documento.CreateElement("detalle");
                XmlElement concepto = documento.CreateElement("concepto");
                concepto.InnerText = det.Nombre;
                detalle.AppendChild(concepto);
                XmlElement cantidad = documento.CreateElement("cantidad");
                cantidad.InnerText = det.Cantidad.ToString();
                detalle.AppendChild(cantidad);
                XmlElement precioUnitario = documento.CreateElement("precioUnitario");
                precioUnitario.InnerText = det.Pvp.ToString("N2");
                detalle.AppendChild(precioUnitario);
                XmlElement descuentos = documento.CreateElement("descuentos");
                descuentos.InnerText = det.Descuento.ToString("N2");
                detalle.AppendChild(descuentos);
                XmlElement precioTotal = documento.CreateElement("precioTotal");
                precioTotal.InnerText = det.Total.ToString("N2");
                detalle.AppendChild(precioTotal);
                detallesxml.AppendChild(detalle);
            }

            XmlElement campoAdicional1 = documento.CreateElement("campoAdicional");
            campoAdicional1.SetAttribute("nombre", "Direccion Establecimiento");
            campoAdicional1.InnerText = factura.Direccion_sucursal;
            infoadicionalxml.AppendChild(campoAdicional1);
            XmlElement campoAdicional2 = documento.CreateElement("campoAdicional");
            campoAdicional2.SetAttribute("nombre", "NombreComercial");
            campoAdicional2.InnerText = factura.Nombre_sucursal;
            infoadicionalxml.AppendChild(campoAdicional2);

            //documento.Save("C:\Users\Minyie\Desktop\fact"+ factura.getNumeroFactura() + ".xml");
            return documento.OuterXml;
        }
    }
}
