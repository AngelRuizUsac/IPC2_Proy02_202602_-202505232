using System;

namespace proyecto2.Models
{
    public class ArbolCategorias
    {
        public Categoria? PrimeraRaiz { get; private set; }

        public ArbolCategorias()
        {
            PrimeraRaiz = null;
        }

        public Categoria? Buscar(string nombre)
        {
            Categoria? actual = PrimeraRaiz;

            while (actual != null)
            {
                if (string.Compare(actual.Nombre, nombre, true) == 0)
                {
                    return actual;
                }

                if (actual.PrimerHijo != null)
                {
                    actual = actual.PrimerHijo;
                }
                else
                {
                    while (actual != null && actual.SiguienteHermano == null){
                        actual = actual.Padre;
                    }

                    if (actual != null){
                        actual = actual.SiguienteHermano;
                    }
                }
            }

            return null;
        }

        public Categoria Agregar(string nombre,string? nombrePadre = null)
        {
            Categoria nueva = new Categoria(nombre);

            if (nombrePadre == null)
            {
                PrimeraRaiz = InsertarOrdenado(
                    PrimeraRaiz, nueva);
            }
            else
            {
                Categoria padre = Buscar(nombrePadre)!;

                nueva.Padre = padre;

                padre.PrimerHijo = InsertarOrdenado(
                    padre.PrimerHijo, nueva);
            }

            return nueva;
        }

        public ListaCategorias Listar()
        {
            ListaCategorias lista = new ListaCategorias();
            Categoria? actual = PrimeraRaiz;
            int nivel = 0;
            while (actual != null)
            {
                lista.Agregar(actual.Nombre, actual.Padre?.Nombre, nivel);
                if (actual.PrimerHijo != null)
                {
                    actual = actual.PrimerHijo;
                    nivel++;
                }
                else
                {
                    while (actual != null && actual.SiguienteHermano == null)
                    {
                        actual = actual.Padre;
                        nivel--;
                    }
                    if (actual != null) actual = actual.SiguienteHermano;
                }
            }
            return lista;
        }

        private Categoria InsertarOrdenado(Categoria? primero,Categoria nueva)
        {
            if (primero == null ||
                string.Compare(nueva.Nombre, primero.Nombre, true) < 0)
            {
                nueva.SiguienteHermano = primero;
                return nueva;
            }
            Categoria actual = primero;
            while (actual.SiguienteHermano != null &&
                   string.Compare(
                       actual.SiguienteHermano.Nombre,
                       nueva.Nombre,
                       true) < 0)
            {
                actual = actual.SiguienteHermano;
            }
            nueva.SiguienteHermano = actual.SiguienteHermano;
            actual.SiguienteHermano = nueva;
            return primero;
        }
    }
}