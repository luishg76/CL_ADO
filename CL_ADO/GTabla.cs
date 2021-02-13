using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;

namespace CL_ADO
{
    /// <summary>
    /// Esta clase es generica
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GTabla<T>:IDisposable
    {
        protected DataTable aTabla=null;
        protected IDataAdapter aAdaptador = null;
        protected DbCommandBuilder aComando;
        string aSQL="";
        protected GConexion aConexion;

        public GTabla(string pSQL,GConexion pConexion) 
        {
            aSQL = pSQL;
            aTabla = new DataTable();
            aConexion = pConexion; 

            switch (aConexion.Tipo_Proveedor)
            {
                case TProveedor.Access:
                    {
                        aAdaptador = new OleDbDataAdapter(aSQL, ((OleDbConnection)aConexion.GetConnection));
                        aComando = new OleDbCommandBuilder((OleDbDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        aComando.ConflictOption = ConflictOption.OverwriteChanges;
                        ((OleDbDataAdapter)aAdaptador).UpdateCommand = ((OleDbCommandBuilder)aComando).GetUpdateCommand();
                        ((OleDbDataAdapter)aAdaptador).InsertCommand = ((OleDbCommandBuilder)aComando).GetInsertCommand();
                        ((OleDbDataAdapter)aAdaptador).DeleteCommand = ((OleDbCommandBuilder)aComando).GetDeleteCommand();
                        break;
                    }
                case TProveedor.SQL_Server:
                    {
                        aAdaptador = new SqlDataAdapter(aSQL, (SqlConnection)aConexion.GetConnection);
                        aComando = new SqlCommandBuilder((SqlDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        aComando.ConflictOption = ConflictOption.OverwriteChanges;
                        ((SqlDataAdapter)aAdaptador).UpdateCommand = ((SqlCommandBuilder)aComando).GetUpdateCommand();
                        ((SqlDataAdapter)aAdaptador).InsertCommand = ((SqlCommandBuilder)aComando).GetInsertCommand();
                        ((SqlDataAdapter)aAdaptador).DeleteCommand = ((SqlCommandBuilder)aComando).GetDeleteCommand();
                        break;                        
                    }
                case TProveedor.Oracle:
                    {
                        aAdaptador = new OdbcDataAdapter(aSQL, (OdbcConnection)aConexion.GetConnection);
                        aComando = new OdbcCommandBuilder((OdbcDataAdapter)aAdaptador);
                        aComando.QuotePrefix = "[";
                        aComando.QuoteSuffix = "]";
                        aComando.ConflictOption = ConflictOption.OverwriteChanges;
                        ((OdbcDataAdapter)aAdaptador).UpdateCommand = ((OdbcCommandBuilder)aComando).GetUpdateCommand();
                        ((OdbcDataAdapter)aAdaptador).InsertCommand = ((OdbcCommandBuilder)aComando).GetInsertCommand();
                        ((OdbcDataAdapter)aAdaptador).DeleteCommand = ((OdbcCommandBuilder)aComando).GetDeleteCommand();
                        break;
                    }
            }           
        }

        public GTabla() 
        {
          aTabla=null;
          aAdaptador = null;
          aComando=null;
          aSQL="";
          aConexion=null;
        }

        //Propiedades==================================================
        public int Cant_Reg 
        {
            get { return aTabla.Rows.Count; }
        }

        public abstract T this[int Ind]
        {
            get;

        }       

        //Métodos===========================================================

        public abstract void Editar(T pElemento);

        public abstract void Agregar(T pElemento);       
        
        protected abstract void CrearTabla();

        public bool Eliminar(int pInd) 
        {
            if ((pInd >= 0) && (pInd <= (Cant_Reg - 1)))
            {
                aTabla.Rows[pInd].Delete();
                this.Guardar();
                return true;
            }
            return false;
        }

        public void Guardar()
        {
               switch (aConexion.Tipo_Proveedor)
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

        public void CargarDatos() 
         {
             try
              {
                aAdaptador.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                switch (aConexion.Tipo_Proveedor)
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
              }
              catch { throw (new Exception("No fué posible cargar los datos")); }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IDbConnection Connection 
        {
            get { return aConexion.GetConnection; }
        }
    }
}
