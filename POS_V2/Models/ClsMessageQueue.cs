using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace POS.Models
{
    public static class ClsMessageQueue
    {
        public enum TipoQueueExcepcion
        {
            NINGUNO = 0,
            QUEUENOENCONTRADO = 1,
            MSQNOINSTALADO = 5,
            TIMEOUT = 2,
            NODISPONIBLE = 3,
            OTROS = 4
        }

        [Serializable]
        public class ClsFacturaMQ
        {
            public string Establecimiento;
            public string PuntoEmision;
            public string Secuencia;
            public string TipoDocumento;
        }

        public class ClsFacturaMQExcept: ClsFacturaMQ
        {
            public Exception _mqExc;
            public TipoQueueExcepcion tipoException;
            public string QueueName;

            public ClsFacturaMQExcept()
            {
            }
            public ClsFacturaMQExcept(ClsFacturaMQ base1)
            {
                base.Establecimiento = base1.Establecimiento;
                base.PuntoEmision = base1.PuntoEmision;
                base.Secuencia = base1.Secuencia;                
            }
        }

        public static bool setMessageQueue(string establecimiento, string puntoemision, string secuencia, string tipoDoc)
        {
            MessageQueue msgQ = null;

            try
            {
                // Arma identificadores
                string colaId = tipoDoc + establecimiento + puntoemision;
                string queueName = $"SecuenciaPOS{colaId}";
                string path = $@".\Private$\{queueName}";
                string description = queueName;

                // Construye el objeto del mensaje
                ClsFacturaMQ clsfactmq = new ClsFacturaMQ
                {
                    Establecimiento = establecimiento,
                    PuntoEmision = puntoemision,
                    Secuencia = secuencia,
                    TipoDocumento = tipoDoc
                };

                // Verifica si la cola existe y créala si no
                if (!MessageQueue.Exists(path))
                {
                    MessageQueue.Create(path, true); // true = transaccional
                }

                // Enviar mensaje a la cola
                msgQ = new MessageQueue(path)
                {
                    Label = description
                };

                System.Messaging.Message msg = new System.Messaging.Message
                {
                    Recoverable = true,
                    Body = clsfactmq,
                    Label = description
                };

                msgQ.Send(msg);
                return true;
            }
            catch (MessageQueueException mqe)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "setMessageQueue", $"MSMQ Error: {mqe.Message}");
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "setMessageQueue", $"General Error: {ex.Message}");
            }
            finally
            {
                if (msgQ != null)
                    msgQ.Dispose();
            }

            return false;
        }

        public static ClsFacturaMQExcept getMessageQueue(string tipodoc, string establecimiento, string ptoemision)
        {
            string MessageQueueName = string.Empty;
            try
            {
                
                MessageQueueName = @".\Private$\SecuenciaPOS"+ tipodoc+ establecimiento+ ptoemision; 
                MessageQueue msgQ = new MessageQueue(MessageQueueName);
                ClsFacturaMQ clsfactmq = new ClsFacturaMQ();
                ClsFacturaMQExcept clsfactmq1 = new ClsFacturaMQExcept();
                Object o = new Object();
                System.Type[] arrTypes = new System.Type[2];
                arrTypes[0] = clsfactmq.GetType();
                arrTypes[1] = o.GetType();
                msgQ.Formatter = new XmlMessageFormatter(arrTypes);

                System.Messaging.Message[] messages = msgQ.GetAllMessages();

                // Peek and format the message. 
                //Message myMessage = msgQ.Peek();
                //clsfactmq = ((ClsFacturaMQ)myMessage.Body);

                foreach (Message msg in messages)
                {
                    clsfactmq = ((ClsFacturaMQ)msg.Body);
                }

                /* if (messages.Count() > 0)
                 {
                     //clsfactmq = ((ClsFacturaMQ)msgQ.Receive().Body);
                     clsfactmq = ((ClsFacturaMQ)msgQ.GetMessageEnumerator2().Current.Body);
                 }*/

                clsfactmq1 = new ClsFacturaMQExcept(clsfactmq);
                clsfactmq1.tipoException = TipoQueueExcepcion.NINGUNO;
                clsfactmq1.QueueName = MessageQueueName;

                return clsfactmq1;
            }
            catch (Exception ex1)
            {
                ClsFacturaMQExcept clsfactmqErr = new ClsFacturaMQExcept();
                clsfactmqErr.tipoException = TipoQueueExcepcion.OTROS;

                if (ex1 is ArgumentException)
                {   //the provided queue name is wrong.                    
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "El nombre de la cola no existe o es incorrecto: " +MessageQueueName);
                }
                else if (ex1 is MessageQueueException)
                {   //if message queue exception occurs either the queue is avialable but without entries (check for peek timeout) or the queue does not exist or you don't have access.                    
                    if(((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.TIMEOUT;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "timeout para obtener la cola de mensaje: " + MessageQueueName);
                    }
                    if (((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                    {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.QUEUENOENCONTRADO;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "La cola de mensaje no existe: '" + MessageQueueName+ "' Error: "+ ex1.Message);
                    }
                    if (((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.QueueNotAvailable)
                    {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.NODISPONIBLE;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "La cola de mensaje : '" + MessageQueueName+"' no se encuentra disponible. Error: "+ ex1.Message);
                    }
                    
                }
                if (ex1.Message.Equals("Message Queuing has not been installed on this computer."))
                {
                    clsfactmqErr.tipoException = TipoQueueExcepcion.MSQNOINSTALADO;
                }

                clsfactmqErr._mqExc = ex1;
                clsfactmqErr.QueueName = MessageQueueName;

                return clsfactmqErr;
            }
        }

        public static ClsFacturaMQExcept receiveMessageQueue(string tipodoc, string establecimiento, string ptoemision)
        {
            string MessageQueueName = string.Empty;
            try
            {

                MessageQueueName = @".\Private$\SecuenciaPOS" + tipodoc+ establecimiento+ ptoemision;
                MessageQueue msgQ = new MessageQueue(MessageQueueName);
                ClsFacturaMQ clsfactmq = new ClsFacturaMQ();
                ClsFacturaMQExcept clsfactmq1 = new ClsFacturaMQExcept();
                Object o = new Object();
                System.Type[] arrTypes = new System.Type[2];
                arrTypes[0] = clsfactmq.GetType();
                arrTypes[1] = o.GetType();
                msgQ.Formatter = new XmlMessageFormatter(arrTypes);

             

                System.Messaging.Message[] messages = msgQ.GetAllMessages();
                /* foreach (Message msg in messages)
                 {
                     clsfactmq = ((ClsFacturaMQ)msg.Body);
                 }
                 */

                if (messages.Count() > 0)
                {
                    messages[0].Recoverable = true;
                    clsfactmq = ((ClsFacturaMQ)msgQ.Receive().Body);                    
                }

                clsfactmq1 = new ClsFacturaMQExcept(clsfactmq);
                clsfactmq1.tipoException = TipoQueueExcepcion.NINGUNO;
                clsfactmq1.QueueName = MessageQueueName;

                return clsfactmq1;
            }
            catch (Exception ex1)
            {
                ClsFacturaMQExcept clsfactmqErr = new ClsFacturaMQExcept();
                clsfactmqErr.tipoException = TipoQueueExcepcion.OTROS;

                if (ex1 is ArgumentException)
                {   //the provided queue name is wrong.                    
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "El nombre de la cola no existe o es incorrecto: " + MessageQueueName);
                }
                else if (ex1 is MessageQueueException)
                {   //if message queue exception occurs either the queue is avialable but without entries (check for peek timeout) or the queue does not exist or you don't have access.                    
                    if (((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.TIMEOUT;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "timeout para obtener la cola de mensaje: " + MessageQueueName);
                    }
                    if (((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                    {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.QUEUENOENCONTRADO;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "La cola de mensaje no existe: '" + MessageQueueName + "' Error: " + ex1.Message);
                    }
                    if (((MessageQueueException)ex1).MessageQueueErrorCode == MessageQueueErrorCode.QueueNotAvailable)
                    {
                        clsfactmqErr.tipoException = TipoQueueExcepcion.NODISPONIBLE;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsMessageQueue", "getMessageQueue", "La cola de mensaje : '" + MessageQueueName + "' no se encuentra disponible. Error: " + ex1.Message);
                    }

                }
                if (ex1.Message.Equals("Message Queuing has not been installed on this computer."))
                {
                    clsfactmqErr.tipoException = TipoQueueExcepcion.MSQNOINSTALADO;
                }

                clsfactmqErr._mqExc = ex1;
                clsfactmqErr.QueueName = MessageQueueName;

                return clsfactmqErr;
            }
        }

    }
}
