using System.IO;
using System.Xml;
using proyecto2.Models;

namespace proyecto2.Services
{
    public class CargadorXml
    {
        public int Cargar(string contenido, Catalogo catalogo)
        {
            CategoriaPendiente? pendientes = LeerCategorias(contenido);
            int rechazados = IncorporarCategorias(pendientes, catalogo);
            return rechazados + LeerLibros(contenido, catalogo);
        }

        private CategoriaPendiente? LeerCategorias(string contenido)
        {
            CategoriaPendiente? primero = null;
            CategoriaPendiente? ultimo = null;

            using (StringReader texto = new StringReader(contenido))
            using (XmlReader lector = XmlReader.Create(texto))
            {
                lector.Read();

                while (!lector.EOF)
                {
                    if (lector.NodeType == XmlNodeType.Element && lector.Name == "listaCategorias" && lector.Depth == 1)
                    {
                        using (XmlReader seccion = lector.ReadSubtree())
                        {
                            seccion.Read();

                            while (!seccion.EOF)
                            {
                                if (seccion.NodeType == XmlNodeType.Element && seccion.Name == "categoria" && seccion.Depth == 1)
                                {
                                    string? padre = seccion.GetAttribute("padre");
                                    string nombre = seccion.ReadElementContentAsString();
                                    CategoriaPendiente nueva = new CategoriaPendiente(nombre, padre);

                                    if (primero == null)
                                    {
                                        primero = nueva;
                                    }
                                    else
                                    {
                                        ultimo!.Siguiente = nueva;
                                    }

                                    ultimo = nueva;
                                }
                                else
                                {
                                    seccion.Read();
                                }
                            }
                        }
                    }

                    lector.Read();
                }
            }

            return primero;
        }

        private int IncorporarCategorias(CategoriaPendiente? primero, Catalogo catalogo)
        {
            int rechazados = 0;
            // Con una jerarquia valida, cada pasada puede incorporar nuevas categorias.
            while (primero != null)
            {
                bool avanzo = false;
                CategoriaPendiente? anterior = null;
                CategoriaPendiente? actual = primero;

                while (actual != null)
                {
                    if (string.IsNullOrWhiteSpace(actual.Nombre) || catalogo.Categorias.Buscar(actual.Nombre) != null || actual.NombrePadre == null || catalogo.Categorias.Buscar(actual.NombrePadre) != null)
                    {
                        avanzo = true;
                        if (catalogo.AgregarCategoria(actual.Nombre, actual.NombrePadre) == null)
                        {
                            rechazados++;
                        }

                        if (anterior == null)
                        {
                            primero = actual.Siguiente;
                        }
                        else
                        {
                            anterior.Siguiente = actual.Siguiente;
                        }
                    }
                    else
                    {
                        anterior = actual;
                    }

                    actual = actual.Siguiente;
                }
                if (!avanzo)
                {
                    // Los pendientes restantes tienen padres inexistentes o ciclos.
                    while (primero != null)
                    {
                        rechazados++;
                        primero = primero.Siguiente;
                    }
                }
            }
            return rechazados;
        }

        private int LeerLibros(string contenido, Catalogo catalogo)
        {
            int rechazados = 0;
            using (StringReader texto = new StringReader(contenido))
            using (XmlReader lector = XmlReader.Create(texto))
            {
                while (lector.Read())
                {
                    if (lector.NodeType == XmlNodeType.Element && lector.Name == "listaLibros" && lector.Depth == 1)
                    {
                        using (XmlReader seccion = lector.ReadSubtree())
                        {
                            while (seccion.Read())
                            {
                                if (seccion.NodeType == XmlNodeType.Element && seccion.Name == "libro" && seccion.Depth == 1)
                                {
                                    using (XmlReader libro = seccion.ReadSubtree())
                                    {
                                        if (!LeerLibro(libro, catalogo))
                                        {
                                            rechazados++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return rechazados;
        }

        private bool LeerLibro(XmlReader lector, Catalogo catalogo)
        {
            long isbn = 0;
            bool isbnValido = false;
            string titulo = "";
            string autor = "";
            string categoria = "";
            lector.Read();

            while (!lector.EOF)
            {
                if (lector.NodeType == XmlNodeType.Element && lector.Depth == 1)
                {
                    string campo = lector.Name;
                    string valor = lector.ReadElementContentAsString();

                    switch (campo)
                    {
                        case "ISBN":
                            isbnValido = long.TryParse(valor, out isbn);
                            break;
                        case "titulo":
                            titulo = valor;
                            break;
                        case "autor":
                            autor = valor;
                            break;
                        case "categoria":
                            categoria = valor;
                            break;
                    }
                }
                else
                {
                    lector.Read();
                }
            }

            return isbnValido && catalogo.RegistrarLibro(isbn, titulo, autor, categoria) != null;
        }
    }
}
