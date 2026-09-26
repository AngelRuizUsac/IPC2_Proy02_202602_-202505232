using System;

namespace proyecto2.Models
{
    public class ArbolLibros
    {
        public NodoLibro? Raiz { get; private set; }

        public ArbolLibros()
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

            if (libro.ISBN < nodo.Libro.ISBN)
            {
                nodo.Izquierdo = InsertarRecursivo(nodo.Izquierdo, libro);
            }
            else
            {
                nodo.Derecho = InsertarRecursivo(nodo.Derecho, libro);
            }

            return Balancear(nodo);
        }

        public Libro? Buscar(long isbn)
        {
            NodoLibro? actual = Raiz;

            while (actual != null)
            {
                if (isbn == actual.Libro.ISBN)
                {
                    return actual.Libro;
                }

                if (isbn < actual.Libro.ISBN)
                {
                    actual = actual.Izquierdo;
                }
                else
                {
                    actual = actual.Derecho;
                }
            }

            return null;
        }

        public void Eliminar(long isbn)
        {
            Raiz = EliminarRecursivo(Raiz, isbn);
        }

        private NodoLibro? EliminarRecursivo(NodoLibro? nodo, long isbn)
        {
            if (nodo == null)
            {
                return null;
            }

            if (isbn < nodo.Libro.ISBN)
            {
                nodo.Izquierdo = EliminarRecursivo(nodo.Izquierdo, isbn);
            }
            else if (isbn > nodo.Libro.ISBN)
            {
                nodo.Derecho = EliminarRecursivo(nodo.Derecho, isbn);
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
                NodoLibro? derechaRestante = EliminarRecursivo(nodo.Derecho, sucesor.Libro.ISBN);

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

        public Libro? ObtenerMenor()
        {
            if (Raiz == null)
            {
                return null;
            }

            return BuscarNodoMenor(Raiz).Libro;
        }

        public Libro? ObtenerMayor()
        {
            if (Raiz == null)
            {
                return null;
            }

            NodoLibro actual = Raiz;

            while (actual.Derecho != null)
            {
                actual = actual.Derecho;
            }

            return actual.Libro;
        }

        public ListaLibros ListarPorISBN()
        {
            ListaLibros resultado = new ListaLibros();
            ListarInorden(Raiz, resultado);
            return resultado;
        }

        private void ListarInorden(NodoLibro? nodo, ListaLibros resultado)
        {
            if (nodo == null)
            {
                return;
            }

            ListarInorden(nodo.Izquierdo, resultado);
            resultado.Agregar(nodo.Libro);
            ListarInorden(nodo.Derecho, resultado);
        }
    }
}