using System;

namespace proyecto2.Models
{
    public class ArbolTitulos
    {
        public NodoLibro? Raiz { get; private set; }

        public ArbolTitulos()
        {
            Raiz = null;
        }

        private int Altura(NodoLibro? nodo)
        {
            if (nodo == null)
            {
                return 0;
            }

            return nodo.Altura;
        }

        private int FactorBalance(NodoLibro nodo)
        {
            return Altura(nodo.Izquierdo) - Altura(nodo.Derecho);
        }

        private void ActualizarAltura(NodoLibro nodo)
        {
            nodo.Altura = 1 + Math.Max(Altura(nodo.Izquierdo), Altura(nodo.Derecho));
        }

        private NodoLibro RotarDerecha(NodoLibro nodo)
        {
            NodoLibro nuevaRaiz = nodo.Izquierdo!;
            NodoLibro? subarbol = nuevaRaiz.Derecho;
            nuevaRaiz.Derecho = nodo;
            nodo.Izquierdo = subarbol;
            ActualizarAltura(nodo);
            ActualizarAltura(nuevaRaiz);
            return nuevaRaiz;
        }

        private NodoLibro RotarIzquierda(NodoLibro nodo)
        {
            NodoLibro nuevaRaiz = nodo.Derecho!;
            NodoLibro? subarbol = nuevaRaiz.Izquierdo;
            nuevaRaiz.Izquierdo = nodo;
            nodo.Derecho = subarbol;
            ActualizarAltura(nodo);
            ActualizarAltura(nuevaRaiz);
            return nuevaRaiz;
        }

        private NodoLibro Balancear(NodoLibro nodo)
        {
            ActualizarAltura(nodo);
            int balance = FactorBalance(nodo);

            if (balance > 1)
            {
                if (FactorBalance(nodo.Izquierdo!) < 0)
                {
                    nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo!);
                }

                return RotarDerecha(nodo);
            }

            if (balance < -1)
            {
                if (FactorBalance(nodo.Derecho!) > 0)
                {
                    nodo.Derecho = RotarDerecha(nodo.Derecho!);
                }

                return RotarIzquierda(nodo);
            }

            return nodo;
        }

        public void Insertar(Libro libro)
        {
            Raiz = InsertarRecursivo(Raiz, libro);
        }

        private NodoLibro InsertarRecursivo(NodoLibro? nodo, Libro libro)
        {
            if (nodo == null)
            {
                return new NodoLibro(libro);
            }

            if (Comparar(libro, nodo.Libro) < 0)
            {
                nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, libro);
            }
            else
            {
                nodo.Derecho = InsertarRecursivo(nodo.Derecho, libro);
            }

            return Balancear(nodo);
        }

        private int Comparar(Libro primero, Libro segundo)
        {
            int orden = string.Compare(primero.Titulo, segundo.Titulo, true);
            return orden == 0 ? primero.ISBN.CompareTo(segundo.ISBN) : orden;
        }

        public ListaLibros Buscar(string titulo)
        {
            ListaLibros resultado = new ListaLibros();
            BuscarCoincidencias(Raiz, titulo, resultado);
            return resultado;
        }

        private void BuscarCoincidencias(NodoLibro? nodo, string titulo, ListaLibros resultado)
        {
            if (nodo == null) return;
            int orden = string.Compare(titulo, nodo.Libro.Titulo, true);
            if (orden <= 0) BuscarCoincidencias(nodo.Izquierdo, titulo, resultado);
            if (orden == 0) resultado.Agregar(nodo.Libro);
            if (orden >= 0) BuscarCoincidencias(nodo.Derecho, titulo, resultado);
        }

        public void Eliminar(Libro libro)
        {
            Raiz = EliminarRecursivo(Raiz, libro);
        }

        private NodoLibro? EliminarRecursivo(NodoLibro? nodo, Libro libro)
        {
            if (nodo == null)
            {
                return null;
            }

            if (Comparar(libro, nodo.Libro) < 0)
            {
                nodo.Izquierdo = EliminarRecursivo(nodo.Izquierdo, libro);
            }
            else if (Comparar(libro, nodo.Libro) > 0)
            {
                nodo.Derecho = EliminarRecursivo(nodo.Derecho, libro);
            }
            else
            {
                if (nodo.Izquierdo == null)
                {
                    return nodo.Derecho;
                }

                if (nodo.Derecho == null)
                {
                    return nodo.Izquierdo;
                }

                NodoLibro sucesor = BuscarNodoMenor(nodo.Derecho);
                NodoLibro? derechaRestante = EliminarRecursivo(nodo.Derecho, sucesor.Libro);
                sucesor.Izquierdo = nodo.Izquierdo;
                sucesor.Derecho = derechaRestante;
                nodo = sucesor;
            }

            return Balancear(nodo);
        }

        private NodoLibro BuscarNodoMenor(NodoLibro nodo)
        {
            while (nodo.Izquierdo != null)
            {
                nodo = nodo.Izquierdo;
            }

            return nodo;
        }

    }
}
