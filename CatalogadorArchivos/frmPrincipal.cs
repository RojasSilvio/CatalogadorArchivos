﻿/*
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
			String[] rutas = Directory.GetFiles(oSeleccionarCarpeta.SelectedPath, "*", SearchOption.AllDirectories);
			String[] directorios = Directory.GetDirectories(oSeleccionarCarpeta.SelectedPath, "*", SearchOption.AllDirectories);
			int dirs = Directory.GetDirectories(oSeleccionarCarpeta.SelectedPath, "*", SearchOption.AllDirectories).Length;
			DirectoryInfo informacionDirectorio = new DirectoryInfo(oSeleccionarCarpeta.SelectedPath);
			DriveInfo informacionDispositivo = new DriveInfo(oSeleccionarCarpeta.SelectedPath);
			var ofrmEscaneoCarpeta = new frmEscaneandoCarpeta();
			
			ofrmEscaneoCarpeta.txtUbicacion.Text = informacionDirectorio.Name;
			ofrmEscaneoCarpeta.lblTamanoValor.Text = FormatSizeExtended(GetDirectorySize(oSeleccionarCarpeta.SelectedPath.ToString()));
			ofrmEscaneoCarpeta.lblEtiquetaUnidad.Text = informacionDispositivo.VolumeLabel;
			ofrmEscaneoCarpeta.lblSistemaArchivosValor.Text = informacionDispositivo.DriveFormat;
			ofrmEscaneoCarpeta.lblTipoValor.Text = informacionDispositivo.DriveType.ToString();
			ofrmEscaneoCarpeta.lblNumeroSerieUnidad.Text = GetHDDSerial();
			ofrmEscaneoCarpeta.lblCapacidadDispositivoValor.Text = FormatSizeExtended(informacionDispositivo.TotalSize);
			ofrmEscaneoCarpeta.lblEspacioLibreDispositivoValor.Text = FormatSizeExtended(informacionDispositivo.AvailableFreeSpace);
			ofrmEscaneoCarpeta.lblContenidoDispositivoValor.Text = String.Concat(files, " Archivos, ", dirs, " Carpetas");
			ofrmEscaneoCarpeta.progressBar1.Value = 100;
			ofrmEscaneoCarpeta.ShowDialog();
			OleDbConnection conexion = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Grupo6.accdb");
			
			string sqlQuery = "INSERT INTO ESCANEOS ([NOMBRE]," +
													"[ARCHIVOS]," +
													"CARPETAS," +
													"TAMANO," +
													"CAPACIDAD," +
													"ESPACIOLIBRE," +
													"SISTEMAARCHIVOS," +
													"NUMEROSERIE, TIPO, ETIQUETA, ID_CATEGORIA, ID_LOCACION, PRESTAMO, FECHA_ESCANEO, LETRA_DISPOSITIVO, COMENTARIO) values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
			
			
			
			
			using (OleDbCommand cmd = new OleDbCommand(sqlQuery, conexion)) {
				conexion.Open();
				cmd.Parameters.AddWithValue("@NOMBRE", informacionDirectorio.Name);
				cmd.Parameters.AddWithValue("@ARCHIVOS", files);
				cmd.Parameters.AddWithValue("@CARPETAS", dirs);
				cmd.Parameters.AddWithValue("@TAMANO", GetDirectorySize(oSeleccionarCarpeta.SelectedPath.ToString()));
				cmd.Parameters.AddWithValue("@CAPACIDAD", informacionDispositivo.TotalSize);
				cmd.Parameters.AddWithValue("@ESPACIOLIBRE", informacionDispositivo.AvailableFreeSpace);
				cmd.Parameters.AddWithValue("@SISTEMAARCHIVOS", informacionDispositivo.DriveFormat);
				cmd.Parameters.AddWithValue("@NUMEROSERIE", GetHDDSerial().ToUpper());
				cmd.Parameters.AddWithValue("@TIPO", informacionDispositivo.DriveType.ToString());
				cmd.Parameters.AddWithValue("@ETIQUETA", oSeleccionarCarpeta.SelectedPath);
				cmd.Parameters.AddWithValue("@ID_CATEGORIA", 1);
				cmd.Parameters.AddWithValue("@ID_LOCACION", 1);
				cmd.Parameters.AddWithValue("@PRESTAMO", "");
				cmd.Parameters.AddWithValue("@FECHA_ESCANEO", OleDbType.Date).Value = DateTime.Now.ToString();
				cmd.Parameters.AddWithValue("@LETRA_DISPOSITIVO", informacionDirectorio.Root.Name);
				cmd.Parameters.AddWithValue("@COMENTARIO", ofrmEscaneoCarpeta.txtComentario.Text);
				
				cmd.ExecuteNonQuery();
			}
			
			foreach (var ruta in rutas) {
				Debug.WriteLine(ruta);
			}
			
			foreach (var directorio in directorios) {
				Debug.WriteLine(directorio);
			}
			treeView1.Nodes.Clear();
			treeView1.Nodes.Add(PopulateTreeNode2(directorios, "\\"));
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
			return string.Format("{0:n1} {1}", number, suffixes[counter]);  
		
		}
		
		public static string FormatSizeExtended(Int64 bytes)
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
				String[] fila = new String[17];
				ListViewItem itm;
				fila[0] = row["ID_ESCANEO"].ToString();
				fila[1] = row["NOMBRE"].ToString();
				fila[2] = row["ARCHIVOS"].ToString();
				fila[3] = row["CARPETAS"].ToString();
				fila[4] = FormatSize(Convert.ToInt64(row["TAMANO"]));
				fila[5] = FormatSize(Convert.ToInt64(row["CAPACIDAD"]));
				fila[6] = FormatSize(Convert.ToInt64(row["ESPACIOLIBRE"]));
				fila[7] = row["SISTEMAARCHIVOS"].ToString();
				fila[8] = row["NUMEROSERIE"].ToString();
				fila[9] = row["TIPO"].ToString();
				fila[10] = row["ETIQUETA"].ToString();
				fila[11] = row["ID_CATEGORIA"].ToString();
				fila[12] = row["ID_LOCACION"].ToString();
				fila[13] = row["PRESTAMO"].ToString();
				fila[14] = row["FECHA_ESCANEO"].ToString();
				fila[15] = row["LETRA_DISPOSITIVO"].ToString();
				fila[16] = row["COMENTARIO"].ToString();
				itm = new ListViewItem(fila);
				listView1.Items.Add(itm);
				
			}
			
		}
		
		private TreeNode PopulateTreeNode2(string[] paths, string pathSeparator)
		{
			if (paths == null)
				return null;

			TreeNode thisnode = new TreeNode();
			TreeNode currentnode;
			char[] cachedpathseparator = pathSeparator.ToCharArray();
			foreach (string path in paths) {
				currentnode = thisnode;
				foreach (string subPath in path.Split(cachedpathseparator)) {
					if (null == currentnode.Nodes[subPath])
						currentnode = currentnode.Nodes.Add(subPath, subPath);
					else
						currentnode = currentnode.Nodes[subPath];                   
				}
			}

			return thisnode;
		}
		
		void MainForm_Load(object sender, EventArgs e)
		{
			treeView1.ExpandAll();
			treeView2.ExpandAll();
			toolStrip1.ImageScalingSize = new Size(30, 30);
		}
		void listView1_ItemActivate(object sender, EventArgs e)
		{
//			path
//			treeView1.Nodes.Add()
		}
		void tsmiIconos_Click(object sender, EventArgs e)
		{
			listView2.View = View.Tile;
		}
		void tsmiDetalles_Click(object sender, EventArgs e)
		{
			listView2.View = View.Details;
		}
		void tssbBuscar_Click(object sender, EventArgs e)
		{
			frmBuscar oFrmBuscar = new frmBuscar();
			
			oFrmBuscar.ShowDialog();
		}
		
		
	}
}
