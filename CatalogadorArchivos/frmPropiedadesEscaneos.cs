/*
 * Creado por SharpDevelop.
 * Usuario: Alumno
 * Fecha: 22 jun. 2021
 * Hora: 21:52
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace CatalogadorArchivos
{
	/// <summary>
	/// Description of frmPropiedadesArchivos.
	/// </summary>
	public partial class frmPropiedadesEscaneos : Form
	{
		static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
		public frmPropiedadesEscaneos()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public void ShowDialog(int indiceEscaneo){
		
			OleDbConnection conexion = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Grupo6.accdb");
			OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM ESCANEOS WHERE ID_ESCANEO = "+ indiceEscaneo +";", conexion);
 
			DataSet d = new DataSet();
			adapter.Fill(d);
 
			foreach (DataRow row in d.Tables[0].Rows) {
				String[] fila = new String[17];
				lblNumeroSerieUnidad.Text = row["NUMEROSERIE"].ToString();
				lblIDEscaneo.Text = row["ID_ESCANEO"].ToString();
				txtUbicacion.Text = row["NOMBRE"].ToString();
				lblContenidoDispositivoValor.Text = row["ARCHIVOS"].ToString() + " Archivos, " + row["CARPETAS"].ToString() + " Carpetas";
				lblTamanoValor.Text = FormatSizeExtended(Convert.ToInt64(row["TAMANO"]));
				lblCapacidadDispositivoValor.Text = FormatSizeExtended(Convert.ToInt64(row["CAPACIDAD"]));
				lblEspacioLibreDispositivoValor.Text = FormatSizeExtended(Convert.ToInt64(row["ESPACIOLIBRE"]));
				lblSistemaArchivosValor.Text = row["SISTEMAARCHIVOS"].ToString();
				lblTipoValor.Text = row["TIPO"].ToString();
				lblEtiquetaUnidad.Text = row["ETIQUETA"].ToString();
				lblAccedidoValor.Text = String.Format("{0:dddd, dd MMMM yyyy, HH:mm:ss}", row["FECHA_ESCANEO"]);
				lblLetraUnidad.Text = row["LETRA_DISPOSITIVO"].ToString();
				txtComentario.Text = row["COMENTARIO"].ToString();
				lblNombreEquipo.Text = row["NOMBRE_PC"].ToString();
				this.ShowDialog();
			}
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
	}
}
