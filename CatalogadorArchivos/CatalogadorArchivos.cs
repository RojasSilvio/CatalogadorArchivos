/*
 * Creado por SharpDevelop.
 * Usuario: Alumno
 * Fecha: 27 jun. 2021
 * Hora: 03:41
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Data.OleDb;

namespace CatalogadorArchivos
{
	/// <summary>
	/// Description of CatalogadorArchivos.
	/// </summary>
	public class CatalogadorArchivos
	{
		public CatalogadorArchivos()
		{
		}
		
		public static void eliminar(int indiceEscaneo){
			OleDbConnection conexion = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Grupo6.accdb");
			String sqlQuery = "DELETE FROM ESCANEOS WHERE ID_ESCANEO = "+ Convert.ToString(indiceEscaneo) +";";
			OleDbCommand cmd = new OleDbCommand(sqlQuery, conexion);
			conexion.Open();
			cmd.ExecuteNonQuery();
			conexion.Close();
			
		}
	}
}
