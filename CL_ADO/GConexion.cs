using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace CL_ADO
{
    /// <summary>
    /// Tipo que representa a los distintos proveedores de Base de Datos
    /// </summary>
    public enum TProveedor {SQL_Server,Access,Oracle}

    public class GConexion
    {
        private TProveedor aTProveedor=TProveedor.Access;       
        private string aOrigen="";
        private string aBaseDatos = "";
        private string aUsuario ="";
        private string aPassword ="";
        private bool aCifrada = false;
        private IDbConnection aConnection;
        private string aStringConnection;
        private bool aAssigned;

        public GConexion(bool pAssigned=true, TProveedor pProveedor=TProveedor.Access)
        {
            aTProveedor = pProveedor;
            aOrigen = "";
            aBaseDatos = "";
            aUsuario = "";
            aPassword = "";
            aConnection = null;
            aAssigned = pAssigned;
        }

        public GConexion(GConexion gConexion)
        {
            aTProveedor = gConexion.Tipo_Proveedor;
            aOrigen = gConexion.Origen;
            aBaseDatos = gConexion.Base_Datos;
            aUsuario = gConexion.Usuario;
            aPassword = gConexion.Password;
            aAssigned = gConexion.Asignada;
            this.AsignarStringConexion();
        }


        //Propiedades=====================================
        /// <summary>
        /// Objeto que contiene la conección a la base de datos según parámetros entrados 
        /// como es el caso del Proveedor, la Ruta de Conección, etc.
        /// </summary>
        public IDbConnection GetConnection
        {
          get
          {
            this.AsignarStringConexion();
            return aConnection;
          }            
        }

        /// <summary>
        /// Ruta de conección a la base de datos
        /// Ej: @"D:\Ejemplo.mdb" o "serversql"
        /// </summary>
        public string Origen
        {
            set{aOrigen = value; }
            get { return aOrigen; }
        }

        public string Base_Datos 
        {
            set { aBaseDatos = value; }
            get { return aBaseDatos; }
        }
        /// <summary>
        /// Verifica si hay conección con la base de datos
        /// </summary>
        /// <returns></returns>
        public bool Verificar()
        {
            try
            {
                this.AsignarStringConexion();
                aConnection.Open();               
            }
            catch
            {
                return false;
            }
            aConnection.Close();
            return true;
        }
       
        /// <summary>
        /// Establece el tipo de proveedor según valores predefinidos en TProveedor
        /// como son: Access,SQL Server,Oracle
        /// </summary>
        public TProveedor Tipo_Proveedor 
        {
            set { aTProveedor = value; }
            get { return aTProveedor; }
        }
        /// <summary>
        /// Usuario con Inicio de Sección en la bases de datos
        /// </summary>
        public string Usuario 
        {
            set 
            {
                if (value.IndexOf(";") >= 0)
                    throw new Exception("Usuario no valido");
                else 
                    aUsuario =value;
            }
            get { return aUsuario; }
        }
        /// <summary>
        /// Contraseña del Usuario que Inicia Sección en la base de datos
        /// </summary>
        public string Password 
        {
            set 
            {
                if (value.IndexOf(";") >= 0)
                    throw new Exception("Contraseña no valida");
                else
                    aPassword = value;
            }
            get { return aPassword; }
        }

        public bool Cifrada
        {
            set { aCifrada = value; }
            get { return aCifrada; }
        }

        public string StringConnection
        {
            set
            {
                if (value!=null)
                {
                  int ini = 0;
                  int fin = 0;
                  int cant = 0;

                  ini = 12 + value.IndexOf("Data Source=");
                  fin = value.IndexOf(';', ini);
                  cant = fin - ini;
                  aBaseDatos = value.Substring(ini, cant);
                }                
                aStringConnection = value;              
            }
            get
            {
                if(aAssigned)
                 return aStringConnection;
                else
                {
                    switch (aTProveedor)
                    {
                        case TProveedor.SQL_Server:
                            {
                                aStringConnection = "Data Source="+aOrigen + ";Initial Catalog=" + aBaseDatos + ";Persist Security Info=true; User ID=" + aUsuario + "; Password=" + aPassword;
                                break;
                            }
                        case TProveedor.Access:
                            {
                                if (aCifrada)
                                    aStringConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + aOrigen + ";Jet OLEDB:Database Password=" + aPassword + ";";
                                else
                                    aStringConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + aOrigen;
                                break;
                            }
                        case TProveedor.Oracle:
                            {
                                aStringConnection ="Provider = OraOLEDB.Oracle.1";
                                break;
                            }                        
                    }
                    return aStringConnection;
                }
            }
        }

        private void AsignarStringConexion()
        {
            if (aAssigned)
            {
                if (aStringConnection != "")
                {
                    switch (aTProveedor)
                    {
                        case TProveedor.SQL_Server:
                            {
                                //aConector = new SqlConnection();                               
                                break;
                            }
                        case TProveedor.Access:
                            {
                                aConnection = new OleDbConnection();                               
                                break;
                            }
                        case TProveedor.Oracle:
                            {
                                //aConector = new OleDbConnection();                               
                                break;
                            }
                        default: throw (new Exception("Tipo Invalido de Proveedor de Base de Datos"));
                    }                   
                }
                
                aConnection.ConnectionString = aStringConnection;  
            }
            else
            {
                switch (aTProveedor)
                {
                    case TProveedor.SQL_Server:
                        {
                            //aConector = new SqlConnection();
                            //aConector.ConnectionString = "Data Source="+aOrigen + ";Initial Catalog=" + aBaseDatos + ";Persist Security Info=true; User ID=" + aUsuario + "; Password=" + aPassword;
                            break;
                        }
                    case TProveedor.Access:
                        {
                            aConnection = new OleDbConnection();
                            if (aCifrada)
                                aConnection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + aOrigen + ";Jet OLEDB:Database Password=" + aPassword + ";";
                            else
                                aConnection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + aOrigen;
                            break;
                        }
                    case TProveedor.Oracle:
                        {
                            //aConector = new OleDbConnection();
                            //aConector.ConnectionString ="Provider = OraOLEDB.Oracle.1";
                            break;
                        }
                    default: throw (new Exception("Tipo Invalido de Proveedor de Base de Datos"));
                }
            }
        }

        public bool Asignada
        {
            get { return aAssigned; }
            set { aAssigned = value; }
        }
    }
}

