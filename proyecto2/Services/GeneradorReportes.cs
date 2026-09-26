using System.Text;
using proyecto2.Models;

namespace proyecto2.Services
{
    public class GeneradorReportes
    {
        public string GenerarCategorias(ArbolCategorias categorias)
        {
            StringBuilder dot = IniciarCategorias();
            Categoria? actual = categorias.PrimeraRaiz;

            while (actual != null)
            {
                AgregarRama(dot, actual);
                actual = actual.SiguienteHermano;
            }

            dot.AppendLine("}");
            return dot.ToString();
        }

        public string GenerarSubcategorias(Categoria categoria)
        {
            StringBuilder dot = IniciarCategorias();
            AgregarRama(dot, categoria);
            dot.AppendLine("}");
            return dot.ToString();
        }

        private StringBuilder IniciarCategorias()
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph Categorias {");
            dot.AppendLine("graph [ordering=out];");
            dot.AppendLine("node [shape=box];");
            return dot;
        }

        private void AgregarRama(StringBuilder dot, Categoria categoria)
        {
            dot.Append('"').Append(Escapar(categoria.Nombre)).AppendLine("\";");
            Categoria? hijo = categoria.PrimerHijo;

            while (hijo != null)
            {
                dot.Append('"').Append(Escapar(categoria.Nombre)).Append("\" -> \"");
                dot.Append(Escapar(hijo.Nombre)).AppendLine("\";");
                AgregarRama(dot, hijo);
                hijo = hijo.SiguienteHermano;
            }
        }

        public string GenerarLibros(ArbolLibros libros)
        {
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph Libros {");
            dot.AppendLine("rankdir=LR;");
            dot.AppendLine("node [shape=box];");
            NodoListaLibro? actual = libros.ListarPorISBN().Primero;

            while (actual != null)
            {
                Libro libro = actual.Libro;
                dot.Append('"').Append(libro.ISBN).Append("\" [label=\"");
                dot.Append(libro.ISBN).Append("\\n").Append(Escapar(libro.Titulo));
                dot.Append("\\n").Append(Escapar(libro.Autor));
                dot.Append("\\n").Append(Escapar(libro.Categoria.Nombre)).AppendLine("\"];");

                if (actual.Siguiente != null)
                {
                    dot.Append('"').Append(libro.ISBN).Append("\" -> \"");
                    dot.Append(actual.Siguiente.Libro.ISBN).AppendLine("\";");
                }

                actual = actual.Siguiente;
            }

            dot.AppendLine("}");
            return dot.ToString();
        }

        private string Escapar(string? texto)
        {
            return (texto ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
        }
    }
}
