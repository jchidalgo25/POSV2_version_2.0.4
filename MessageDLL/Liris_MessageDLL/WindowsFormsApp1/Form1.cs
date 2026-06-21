using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MensajesLibrary;
using Microsoft.PointOfService;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        LineDisplay lineDisplay;
        PosExplorer posExplorer;
        PosPrinter selectedPrinter;

        string Titulo = string.Empty;
        string MsjBox = string.Empty;
        int Timer = 0; 
        public Form1()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a PosExplorer instance
                PosExplorer posExplorer = new PosExplorer();

                // Get a list of all printer devices
                DeviceCollection printers = posExplorer.GetDevices(DeviceType.PosPrinter);

                if (printers.Count > 0)
                {
                    Console.WriteLine("Available POS Printers:");
                    foreach (DeviceInfo printer in printers)
                    {
                        Console.WriteLine($"Name: {printer.ServiceObjectName}, Type: {printer.LogicalNames}");
                    }
                }
                else
                {
                    Console.WriteLine("No POS printers found.");
                }
            }
            catch (PosException ex)
            {
                Console.WriteLine($"POS Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            try
            {
                PosExplorer posExplorer = new PosExplorer();

                // Get all POS printers
                DeviceCollection printers = posExplorer.GetDevices(DeviceType.PosPrinter);

                if (printers.Count > 0)
                {
                    PosPrinter printer = (PosPrinter)posExplorer.CreateInstance(
                       // posExplorer.GetDevice("PosPrinter", "YOUR_PRINTER_NAME")
                       // posExplorer.GetDevice("PosPrinter", "YOUR_PRINTER_NAME")
                       // posExplorer.GetDevice("PosPrinter", "YOUR_PRINTER_NAME")
                       // posExplorer.GetDevice("PosPrinter", "YOUR_PRINTER_NAME")
                       // posExplorer.GetDevice("PosPrinter", "YOUR_PRINTER_NAME")
                       posExplorer.GetDevice("PosPrinter", "POS-80C (copy 4)")
                    );

                    //// Select a specific printer by LogicalName (update accordingly)
                    //string selectedPrinter = "EPSON_TM_T20"; // Replace with your printer name

                    //PosPrinter printerDevice = (PosPrinter)posExplorer.CreateInstance(
                    //    posExplorer.GetDevice("PosPrinter", selectedPrinter)
                    //);

                    printer.Open();
                    printer.Claim(1000);
                    printer.DeviceEnabled = true;
                    printer.PrintNormal(PrinterStation.Receipt, "Test Print\n");
                    printer.Release();
                    printer.Close();

                    Console.WriteLine("Print successful.");
                }
                else
                {
                    Console.WriteLine("No POS printers found.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private async  void button3_Click(object sender, EventArgs e)
        {

            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            // MsgBoxCtrl.MessageBoxResult result = await msgBoxCtrl.ShowMessageAsync( "info", "Mensaje Texto", "Titulo");
            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage("info", "Mensaje Texto", "Titulo");


        }

        private void button4_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage("info", "tiempo de espera", "Titulo", 10, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage("question", "tiempo de espera", "Titulo", 10, true);

            // MsgBoxCtrl.MessageBoxResult.Yes

        }




        private void btnInfo_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageType messageType;

            Titulo = this.txtTitulo.Text;
            MsjBox = this.txtMsjBox.Text;
            if (this.txtTimer.Text != "") { Timer = Int32.Parse(this.txtTimer.Text);  }


            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, MsjBox, Titulo, Timer, false);

        }

        private void btnWarning_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageType messageType;

            Titulo = this.txtTitulo.Text;
            MsjBox = this.txtMsjBox.Text;
            if (this.txtTimer.Text != "") { Timer = Int32.Parse(this.txtTimer.Text); }

            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Warning, MsjBox, Titulo, Timer, false);
            



        }

        private void btnQuestion_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageType messageType;
            Titulo = this.txtTitulo.Text;
            MsjBox = this.txtMsjBox.Text;
            if (this.txtTimer.Text != "") { Timer = Int32.Parse(this.txtTimer.Text); }



            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Question, MsjBox, Titulo, Timer, false);

            //MsgBoxCtrl.MessageBoxResult.Yes;
                // result
        }
       
        private void btnStop_Click(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageType messageType;

            Titulo = this.txtTitulo.Text;
            MsjBox = this.txtMsjBox.Text;
            if (this.txtTimer.Text != "") { Timer = Int32.Parse(this.txtTimer.Text); }

            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Stop, MsjBox, Titulo, Timer, false);
            

        }

        private void btnError_Click_1(object sender, EventArgs e)
        {
            MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
            MsgBoxCtrl.MessageType messageType;

            Titulo = this.txtTitulo.Text;
            MsjBox = this.txtMsjBox.Text;
            if (this.txtTimer.Text != "") { Timer = Int32.Parse(this.txtTimer.Text); }


            MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjBox, Titulo);
            //MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjBox, Titulo, Timer, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
