using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.Sql;

namespace CL_ADO
{
   public class GOperationSQL
    {

       static public bool Salvar_BD(string pRuta,GConexion pConexion) 
       {
           string sBackup = "BACKUP DATABASE " + pConexion.Base_Datos +" TO DISK = N'" + pRuta +
               "' WITH NOFORMAT, NOINIT, NAME =N'" + pConexion.Base_Datos + ",SKIP, STATS = 10 '";
                
                //" TO DISK = N'" + this.txtRuta.Text +                 
                // "' Full Database Backup ,SKIP, STATS = 10 '";
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = pConexion.Origen;
            csb.InitialCatalog = pConexion.Base_Datos;
            csb.Password = pConexion.Password;
            csb.UserID = pConexion.Usuario;

            using (SqlConnection con = new SqlConnection(csb.ConnectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmdBackUp = new SqlCommand(sBackup, con);
                    cmdBackUp.ExecuteNonQuery(); 
                    con.Close();
                    return true;
                }
                catch {return false; }
            }
       }

       static public bool Restaurar_BD(string pRuta,GConexion pConexion) 
       {
           string sBackup = "RESTORE DATABASE " + pConexion.Base_Datos +
                " FROM DISK = '" + pRuta + "'" +
                " WITH REPLACE";

           SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
           csb.DataSource = pConexion.Origen;
           // Es mejor abrir la conexión con la base Master
           csb.InitialCatalog = "master";
           csb.IntegratedSecurity = true;
           //csb.ConnectTimeout = 480; // el predeterminado es 15

           using (SqlConnection con = new SqlConnection(csb.ConnectionString))
           {
               try
               {
                   con.Open();

                   SqlCommand cmdBackUp = new SqlCommand(sBackup, con);
                   cmdBackUp.ExecuteNonQuery();
                   con.Close();
                   return true;
               }
               catch {return false;}
           }
       }
       /// <summary>
       /// Este método devuelve una lista con todos los Nombre de los Servidores
       /// disponibles en la RED actual;
       /// </summary>
       /// <returns>arreglo de string</returns>

       static public string[] Lista_ServidoresSQL() 
       {
           string[] Lista = null;
           int Cant=0;
           SqlDataSourceEnumerator instancia = SqlDataSourceEnumerator.Instance;
           DataTable tabla = instancia.GetDataSources();
           Lista=new string[tabla.Rows.Count];

           foreach (DataRow Registro in tabla.Rows) 
           {
               foreach (DataColumn Columna in tabla.Columns) 
               {
                   if (Columna.ColumnName == "ServerName")
                       Lista[Cant++] = Registro[Columna].ToString();
               }
           }
           return Lista;
       }

       static public string[] Lista_BDSQL(string pServidorSQL) 
       {
           // Las bases de datos propias de SQL Server
           string[] basesSys = { "master", "model", "msdb", "tempdb" };
           string[] bases=null;
           DataTable dt = new DataTable();
           // Usamos la seguridad integrada de Windows
           string sCnn = "Data Source=" + pServidorSQL + "; Initial Catalog=master; integrated security=yes";
           //"Data Source=infomov;Initial Catalog=Comercial;Persist Security Info=True;User ID=sa;Password=ism.37"
           // La orden T-SQL para recuperar las bases de master
           string sel = "SELECT name FROM sysdatabases";
           try
           {
               SqlDataAdapter da = new SqlDataAdapter(sel, sCnn);
               da.Fill(dt);
               bases = new string[dt.Rows.Count - 1];
               int k = -1;
               for (int i = 0; i < dt.Rows.Count; i++)
               {
                   string s = dt.Rows[i]["name"].ToString();
                   // Solo asignar las bases que no son del sistema
                   if (Array.IndexOf(basesSys, s) == -1)
                   {
                       k += 1;
                       bases[k] = s;
                   }
               }
               if (k == -1) return null;
               // ReDim Preserve
               {
                   int i1_RPbases = bases.Length;
                   string[] copiaDe_bases = new string[i1_RPbases];
                   Array.Copy(bases, copiaDe_bases, i1_RPbases);
                   bases = new string[(k + 1)];
                   Array.Copy(copiaDe_bases, bases, (k + 1));
               };
               return bases;
           }
           catch { return null; }           
       }
    }
}
