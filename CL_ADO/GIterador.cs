using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CL_ADO
{
    public class GIterador<T>:IEnumerator<T>
    {
        GTabla<T> aTabla=null;
        int pos=0;

        public GIterador(GTabla<T> pTabla) 
        {
            aTabla = pTabla;
        }

        public bool MoveAfter() 
        {
            if (pos > 0)
            {
                pos--;
                return true;
            }
            else
                return false;
        }       

        public void Last() 
        {
            pos = aTabla.Cant_Reg - 1;
        }

        bool IEnumerator.MoveNext()
        {
            if (pos < aTabla.Cant_Reg - 1)
            {
                pos++;
                return true;
            }
            else
                return false;
        }

        void IEnumerator.Reset()
        {
            pos = 0;
        }

        public int Pos 
        {
            get { return pos; }
        }

       T IEnumerator<T>.Current
        {
            get
            {
                if ((pos >= 0) || (pos <= aTabla.Cant_Reg - 1))
                    return aTabla[pos];
                return default(T);
            }
        }

       object IEnumerator.Current => throw new NotImplementedException();

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: elimine el estado administrado (objetos administrados).
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~GIterador() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        void IDisposable.Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
