using System;
using System.Collections.Generic;
using System.Text;
using ReceptionSdk;
using ReceptionSdk.Http;
using ReceptionSdk.Models;
using System.Collections;
using System.Dynamic;
//using System.Threading;
using ReceptionSdk.Exceptions;
using ReceptionSdk.Helpers;
using ReceptionSdk.Clients;
using System.Xml;
using System.Linq;
using ReceptionSdk.Utils;

namespace IntegracionPedidos
{
   
       
    public class Integracion
    {
        public static ApiClient api;

        //public void Main()
        //{
        //    Credentials credentials = new Credentials();

        //    credentials.ClientId = "";
        //    credentials.ClientSecret = "";
        //    credentials.Environment = Environments.DEVELOPMENT;
        //    api = new ApiClient(credentials);
        //}

        public string Integration(string XmlIntegracion)
        {
            string XmlRequest = string.Empty;
            Credentials credentials = new Credentials();

            credentials.ClientId = "integration_delportal";
            credentials.ClientSecret = "E!D1qKSYUL";
            credentials.Environment = Environments.DEVELOPMENT;
            api = new ApiClient(credentials);


            if (!string.IsNullOrEmpty(XmlIntegracion))
            {
               
                System.Xml.Linq.XDocument xDoc = System.Xml.Linq.XDocument.Parse(XmlIntegracion);

                var MetodoObj = (from d in xDoc.Descendants("Root")
                                    select new
                                    {
                                        Integ  = d.Attribute("Integ").Value,
                                        ProgId = d.Attribute("ProgId").Value                                       
                                    }).FirstOrDefault();

                var DatosObj = (from d in xDoc.Descendants("req")
                                 select new
                                 {
                                     DelportalId    = d.Attribute("DelportalId").Value,
                                     orderId        = d.Attribute("orderId").Value,
                                     sInvoice       = d.Attribute("sInvoice").Value,
                                     Id             = d.Attribute("Id").Value,
                                     Name           = d.Attribute("Name").Value,
                                     Description    = d.Attribute("Description").Value,
                                     VersionOs      = d.Attribute("VersionOs").Value,
                                     VersionPos     = d.Attribute("VersionPos").Value
                                 }).FirstOrDefault();

                if (MetodoObj.ProgId == "INYA")
                {
                    XmlRequest = Initialization(DatosObj.DelportalId,DatosObj.VersionOs,DatosObj.VersionPos);
                }
                //else if (MetodoObj.ProgId == "HBYA")
                //{
                //    Heartbeat(DatosObj.DelportalId);
                //}
                else if (MetodoObj.ProgId == "DTYA")
                {
                    XmlRequest = DeliveyTime();
                }
                else if (MetodoObj.ProgId == "RMYA")
                {
                    XmlRequest = RejectMessages();
                }
                else if (MetodoObj.ProgId == "REYA")
                {
                    XmlRequest = Reception(Convert.ToInt32(DatosObj.orderId),DatosObj.DelportalId);
                }
                else if (MetodoObj.ProgId == "ALYA")
                {
                    XmlRequest = Acknowledgement(Convert.ToInt32(DatosObj.orderId), DatosObj.DelportalId);
                }
                else if (MetodoObj.ProgId == "SCYA")
                {
                    XmlRequest = State_change(Convert.ToInt32(DatosObj.orderId), DatosObj.DelportalId);
                }
                else if (MetodoObj.ProgId == "DIYA")
                {
                    XmlRequest = Dispatch(Convert.ToInt32(DatosObj.orderId), DatosObj.DelportalId);
                }               
                else if (MetodoObj.ProgId == "BIYA")
                {
                    XmlRequest = Billing(Convert.ToInt32(DatosObj.orderId), DatosObj.sInvoice);
                }
                else if (MetodoObj.ProgId == "COYA")
                {
                    XmlRequest = ConfirmOrder(Convert.ToInt32(DatosObj.orderId), Convert.ToInt32(DatosObj.Id),DatosObj.Name,DatosObj.Description);
                }
                else if (MetodoObj.ProgId == "GRYA")
                {
                    XmlRequest = GetTiendas();
                }
            }


            return XmlRequest;
        }
        public static string DeliveyTime ()
        {
            string xmlresp = string.Empty;
            IList<DeliveryTime> deliveryTimes = api.Order.DeliveryTime.GetAll();
            xmlresp = "<root ProgId='DeliveyTime'>";
            for (int i = 0; i < deliveryTimes.Count+1; i++)
            {
                xmlresp = xmlresp + "<req Respuesta='True' Id='" + Convert.ToString(deliveryTimes[i].Id) + "' name='" + deliveryTimes[i].Name + "' description='" + deliveryTimes[i].Description + "></req>";

            }
            xmlresp = xmlresp + " </Root>";
            return xmlresp;
        }
        public static string RejectMessages()
        {
            string xmlresp = string.Empty;
            IList<RejectMessage> rejectMessages = api.Order.RejectMessage.GetAll();

            xmlresp = "<root ProgId='RejectMessages'>";
            for (int i = 0; i < rejectMessages.Count + 1; i++)
            {
                xmlresp = xmlresp + "<req Respuesta='True' Id='" + Convert.ToString(rejectMessages[i].Id) + "' name='" + rejectMessages[i].DescriptionES + "' description='" + rejectMessages[i].DescriptionPT + "' ForLogistics='" + rejectMessages[i].ForLogistics + "' ForPickup='" + rejectMessages[i].ForPickup + "></req>";

            }
            xmlresp = xmlresp + " </Root>";
            return xmlresp;
        }
        public static string Initialization(string DelportalId,string VersionOs,string versionPos)
        {
            dynamic version = new ExpandoObject();            
            version.os = VersionOs;
            version.app = versionPos;
            //long DelportalId = 1;
            //api.Event.Initialization(version, DelportalId);

            string xmlresp = "<root ProgId='Initialization' > <req Respuesta='True' ></req> </Root>";
            return xmlresp;          
        }
        //public static void Heartbeat( string DelportalId)
        //{
        //    Timer timer = new Timer(
        //    callback => {
        //        try
        //        {
        //            //long DelportalId = 1;
        //            api.Event.HeartBeat(DelportalId);
        //        }
        //        catch (ApiException ex)
        //        {
        //            // Something went wrong
        //        }
        //        catch (Exception ex)
        //        {
        //            // Something went wrong
        //        }
        //    }
        //    );
        //    timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(2));
        //}
        public static string Reception(long orderId, string DelportalId)
        {
            //long orderId = OrderReceivedId();
            //api.Event.Reception(orderId, DelportalId);
            string xmlresp = "<root ProgId='Reception'> <req Respuesta='True' ></req> </Root>";
            return xmlresp;

        }
        public static string Acknowledgement(long orderId, string DelportalId)
        {
          
            //api.Event.Acknowledgement(orderId, DelportalId);
            string xmlresp = "<root ProgId='Acknowledgement'> <req Respuesta='True' ></req> </Root>";
            return xmlresp;
        }

        public static string State_change(long orderId, string DelportalId)
        {
            //api.Event.StateChange(orderId, OrderState.CONFIRMED, DelportalId);
            string xmlresp = "<root ProgId='State_change'> <req Respuesta='True' ></req> </Root>";
            return xmlresp;
        }
        public static string Dispatch(long orderId, string DelportalId)
        {
            string xmlresp = string.Empty;
            try
            {
                //Order order = OrderToDispatch(orderId);

                bool result = true;// api.Order.Dispatch(order);
                if (result == true)
                {
                    xmlresp = "<root ProgId='Dispatch'> <req Respuesta='True' ></req> </Root>";
                    //DispatchOrderLocally(order);
                }
            }
            catch (ApiException ex)
            {
                // Something went wrong
                if (ex.Code == ErrorCode.INTERNAL_SERVER_ERROR)
                {
                    // Wait 5 seconds and retry again
                    // Don't retry more than three times
                }
                else
                {
                    throw ex;
                }
            }
            return xmlresp;

        }
        private static Order OrderToDispatch(long orderId)
        {
            Order order = api.Order.Get(orderId);
            return order;
        }
        public static string Billing(long orderId,string sInvoice)
        {
            string xmlresp = string.Empty;
            try
            {
                //Order order = OrderToBill(orderId);
                //string Invoice = sInvoice;
                //order.Payment.Invoice = "the new invoice number";
                //order.Payment.Total = 1000; // Edit the total is optional
                bool result = true;// api.Order.Bill(orderId, Invoice);
                if (result == true)
                {
                    xmlresp = "<root ProgId='Dispatch'> <req Billing='True' ></req> </Root>";
                    //BillOrderLocally(order);
                }
            }
            catch (ApiException ex)
            {
                // Something went wrong
                if (ex.Code == ErrorCode.INTERNAL_SERVER_ERROR)
                {
                    // Wait 5 seconds and retry again
                    // Don't retry more than three times
                }
                else
                {
                    throw ex;
                }
            }
            return xmlresp;
        }
        private static Order OrderToBill(long orderId)
        {
            Order order = api.Order.Get(orderId);
            return order;
        }
        public static string ConfirmOrder(long orderId,long Id,string name,string description)
        {
            string xmlresp = string.Empty;
            try
            {
                DeliveryTime deliveryTime = RestaurantSelectedTime(Id,name,description);
                Order order = OrderToConfirm(orderId);
                bool result = false;
                if (order.PreOrder == true || order.Logistics == true)
                {
                    result = api.Order.Confirm(order);
                }
                else
                {
                    result = api.Order.Confirm(order, deliveryTime);
                }
                if (result == true)
                {
                    xmlresp = "<root ProgId='ConfirmOrder'> <req Billing='True' ></req> </Root>";
                    //ConfirmOrderLocally(order);
                }
            }
            catch (ApiException ex)
            {
                // Something went wrong
                if (ex.Code == ErrorCode.INTERNAL_SERVER_ERROR)
                {
                    // Wait 5 seconds and retry again
                    // Don't retry more than three times
                }
                else
                {
                    throw ex;
                }
            }
            return xmlresp;
        }

        private static DeliveryTime RestaurantSelectedTime(long IdDelportal,string NameDelportal, string DescriptionDelportal)
        {
            var deliveryTime = new DeliveryTime();
            deliveryTime.Id = IdDelportal;
            deliveryTime.Name = NameDelportal;
            deliveryTime.Description = DescriptionDelportal;

            return deliveryTime;
        }

        private static Order OrderToConfirm(long orderId)
        {
            Order order = api.Order.Get(orderId);
            return order;
        }

        public  static string GetTiendas()
        {
            IList<Restaurant> restaurants = api.Restaurant.GetAll(PaginationOptions.Create());
            return "";
        }
        //private DeliveryTime RestaurantSelectedTime()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
