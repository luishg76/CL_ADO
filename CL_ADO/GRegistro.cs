using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace CL_ADO
{
    public abstract class GRegistro
    {
        protected DataTable aTabla;
        protected DataRow aRegistro;
        protected IDataAdapter aAdaptador;
        protected DbCommandBuilder aComando;
        string aSQL;
        protected GConexion aConnection;

        public GRegistro(string pSQL,GConexion pConeccion) 
        {
            aSQL = pSQL;
            aTabla = new DataTable();
            aConnection = pConeccion;

            switch (aConnection.Tipo_Proveedor)
            {
                case TProveedor.Access:
                    {
                        aAdaptador = new OleDbDataAdapter(aSQL, (OleDbConnection)aConnection.GetConnection);
                        aComando = new OleDbCommandBuilder((OleDbDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        ((OleDbDataAdapter)aAdaptador).UpdateCommand = ((OleDbCommandBuilder)aComando).GetUpdateCommand();
                        ((OleDbDataAdapter)aAdaptador).InsertCommand = ((OleDbCommandBuilder)aComando).GetInsertCommand();
                        ((OleDbDataAdapter)aAdaptador).DeleteCommand = ((OleDbCommandBuilder)aComando).GetDeleteCommand();
                        break;
                    }
                case TProveedor.SQL_Server:
                    {
                        aAdaptador = new SqlDataAdapter(aSQL, (SqlConnection)aConnection.GetConnection);
                        aComando = new SqlCommandBuilder((SqlDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        ((SqlDataAdapter)aAdaptador).UpdateCommand = ((SqlCommandBuilder)aComando).GetUpdateCommand();
                        ((SqlDataAdapter)aAdaptador).InsertCommand = ((SqlCommandBuilder)aComando).GetInsertCommand();
                        ((SqlDataAdapter)aAdaptador).DeleteCommand = ((SqlCommandBuilder)aComando).GetDeleteCommand();
                        break;
                    }
                case TProveedor.Oracle:
                    {
                        aAdaptador = new OdbcDataAdapter(aSQL, (OdbcConnection)aConnection.GetConnection);
                        aComando = new OdbcCommandBuilder((OdbcDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        ((OdbcDataAdapter)aAdaptador).UpdateCommand = ((OdbcCommandBuilder)aComando).GetUpdateCommand();
                        ((OdbcDataAdapter)aAdaptador).InsertCommand = ((OdbcCommandBuilder)aComando).GetInsertCommand();
                        ((OdbcDataAdapter)aAdaptador).DeleteCommand = ((OdbcCommandBuilder)aComando).GetDeleteCommand();
                        break;
                    }
            }            
        }

        public GRegistro() 
        {
           aTabla=null;
           aRegistro=null;
           aAdaptador = null;
           aComando=null;
           aSQL="";
           aConnection=null;
        }
        
        //Métodos===========================================================

        protected abstract void CrearTabla();

        public void Guardar()
        {
               switch (aConnection.Tipo_Proveedor)
                {
                    case TProveedor.Access:
                        ((OleDbDataAdapter)aAdaptador).Update(aTabla);
                        break;
                    case TProveedor.SQL_Server:
                        ((SqlDataAdapter)aAdaptador).Update(aTabla);
                        break;
                    case TProveedor.Oracle:
                        ((OdbcDataAdapter)aAdaptador).Update(aTabla);
                        break;                               
                 }           
            aTabla.AcceptChanges();
        }

        protected void CargarDatos() 
         {
             try
              {
                aAdaptador.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                switch (aConnection.Tipo_Proveedor)
                {
                    case TProveedor.Access:
                        ((OleDbDataAdapter)aAdaptador).Fill(aTabla);                         
                        break;
                    case TProveedor.SQL_Server:
                        ((SqlDataAdapter)aAdaptador).Fill(aTabla);                        
                        break;
                    case TProveedor.Oracle:
                        ((OdbcDataAdapter)aAdaptador).Fill(aTabla);
                        break;
                }
                //aRegistro = aTabla.Rows[0];
              }
              catch { throw (new Exception("No fué posible cargar los datos")); }
        }

        public GConexion Conección 
        {
            get { return aConnection; }
        }

    }
}
