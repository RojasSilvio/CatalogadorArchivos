/*
 * Creado por SharpDevelop.
 * Usuario: Alumno
 * Fecha: 5 jun. 2021
 * Hora: 22:48
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;

namespace CatalogadorArchivos
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
		{
			
			var oSeleccionarCarpeta = new FolderBrowserDialog();
			
			oSeleccionarCarpeta.ShowDialog();
			int files = Directory.GetFiles(oSeleccionarCarpeta.SelectedPath, "*.*", SearchOption.AllDirectories).Length;
			int dirs = Directory.GetDirectories(oSeleccionarCarpeta.SelectedPath, "*", SearchOption.AllDirectories).Length;
			DirectoryInfo informacionDirectorio = new DirectoryInfo(oSeleccionarCarpeta.SelectedPath);
			DriveInfo informacionDispositivo = new DriveInfo(oSeleccionarCarpeta.SelectedPath);
			var ofrmEscaneoCarpeta = new frmEscaneandoCarpeta();
			ofrmEscaneoCarpeta.txtUbicacion.Text = informacionDirectorio.Name;
			ofrmEscaneoCarpeta.lblTamanoValor.Text = FormatSize(GetDirectorySize(oSeleccionarCarpeta.SelectedPath.ToString()));
			ofrmEscaneoCarpeta.lblEtiquetaUnidad.Text = informacionDispositivo.VolumeLabel;
			ofrmEscaneoCarpeta.lblSistemaArchivosValor.Text = informacionDispositivo.DriveFormat;
			ofrmEscaneoCarpeta.lblTipoValor.Text = informacionDispositivo.DriveType.ToString();
			ofrmEscaneoCarpeta.lblNumeroSerieUnidad.Text = GetHDDSerial();
			ofrmEscaneoCarpeta.lblCapacidadDispositivoValor.Text = FormatSize(informacionDispositivo.TotalSize);
			ofrmEscaneoCarpeta.lblEspacioLibreDispositivoValor.Text = FormatSize(informacionDispositivo.AvailableFreeSpace);
			ofrmEscaneoCarpeta.lblContenidoDispositivoValor.Text = String.Concat(files," Archivos, ", dirs, " Carpetas");
			ofrmEscaneoCarpeta.progressBar1.Value = 100;
			ofrmEscaneoCarpeta.ShowDialog();
			
		}
		
		private static long GetDirectorySize(string p)
		{
			// 1.
			// Get array of all file names.
			string[] a = Directory.GetFiles(p, "*.*");

			// 2.
			// Calculate total bytes of all files in a loop.
			long b = 0;
			foreach (string name in a) {
				// 3.
				// Use FileInfo to get length of each file.
				FileInfo info = new FileInfo(name);
				b += info.Length;
			}
			// 4.
			// Return total size
			return b;
		}
		static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };  
		public static string FormatSize(Int64 bytes)
		{  
			int counter = 0;  
			decimal number = (decimal)bytes;  
			while (Math.Round(number / 1024) >= 1) {  
				number = number / 1024;  
				counter++;  
			}  
			return string.Format("{0:n1} {1} ({2:n1} bytes)", number, suffixes[counter], bytes);  
		
		
		}
	
		public string GetHDDSerial()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

			foreach (ManagementObject wmi_HD in searcher.Get()) {
				// get the hardware serial no.
				if (wmi_HD["SerialNumber"] != null)
					Debug.WriteLine(wmi_HD["SerialNumber"].ToString());
				return wmi_HD["SerialNumber"].ToString();
			}

			return string.Empty;
		}
		void Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
		void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
	
		}
	
	 
	}
}
