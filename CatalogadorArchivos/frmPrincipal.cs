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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using System.Data.OleDb;

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
			ofrmEscaneoCarpeta.lblContenidoDispositivoValor.Text = String.Concat(files, " Archivos, ", dirs, " Carpetas");
			ofrmEscaneoCarpeta.progressBar1.Value = 100;
			ofrmEscaneoCarpeta.ShowDialog();
			OleDbConnection conexion = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Grupo6.accdb");
			
			string sqlQuery = "INSERT INTO ESCANEOS ([NOMBRE], [ARCHIVOS], CARPETAS, TAMANO, CAPACIDAD, ESPACIOLIBRE, SISTEMAARCHIVOS) values (?,?,?,?,?,?,?)";
			
			
			
			
			using (OleDbCommand cmd = new OleDbCommand(sqlQuery, conexion)) {
				conexion.Open();
				cmd.Parameters.AddWithValue("@NOMBRE", informacionDirectorio.Name);
				cmd.Parameters.AddWithValue("@ARCHIVOS", files);
				cmd.Parameters.AddWithValue("@CARPETAS", dirs);
				cmd.Parameters.AddWithValue("@TAMANO", GetDirectorySize(oSeleccionarCarpeta.SelectedPath.ToString()));
				cmd.Parameters.AddWithValue("@CAPACIDAD", informacionDispositivo.TotalSize);
				cmd.Parameters.AddWithValue("@ESPACIOLIBRE", informacionDispositivo.AvailableFreeSpace);
				cmd.Parameters.AddWithValue("@SISTEMAARCHIVOS", informacionDispositivo.DriveFormat);

				cmd.ExecuteNonQuery();

			}
 
//			OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM ESCANEOS", conexion);
// 
//			DataSet d = new DataSet();
//			adapter.Fill(d);
// 
//			foreach (DataRow row in d.Tables[0].Rows) {
//				Console.WriteLine("ID: " + row["Id"]);
//				Console.WriteLine("Nombre: " + row["nombre"]);
//				Console.WriteLine("Apellidos: " + row["apellidos"]);
//				Console.WriteLine("Edad: " + row["edad"]);
//				Console.WriteLine("");
//			}
// 
			conexion.Close();
			
			RefrescaEscaneos();
			
 
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
		
		private void RefrescaEscaneos(){
			listView1.Items.Clear();
			OleDbConnection conexion = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Grupo6.accdb");
			OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM ESCANEOS", conexion);
 
			DataSet d = new DataSet();
			adapter.Fill(d);
 
			foreach (DataRow row in d.Tables[0].Rows) {
				String[] fila = new String[8];
				ListViewItem itm;
				fila[0] = row["ID_ESCANEO"].ToString();
				fila[1] = row["NOMBRE"].ToString();
				fila[2] = row["ARCHIVOS"].ToString();
				fila[3] = row["CARPETAS"].ToString();
				fila[4] = FormatSize(Convert.ToInt64(row["TAMANO"]));
				fila[5] = FormatSize(Convert.ToInt64(row["CAPACIDAD"]));
				fila[6] = FormatSize(Convert.ToInt64(row["ESPACIOLIBRE"]));
				fila[7] = row["SISTEMAARCHIVOS"].ToString();
				itm = new ListViewItem(fila);
				listView1.Items.Add(itm);
				
			}
			
		}
		
		
	}
}
