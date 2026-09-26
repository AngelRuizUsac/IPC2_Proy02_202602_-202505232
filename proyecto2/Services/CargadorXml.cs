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
            int duplicados = IncorporarCategorias(pendientes, catalogo);
            return duplicados + LeerLibros(contenido, catalogo);
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
            int duplicados = 0;
            // Con una jerarquia valida, cada pasada puede incorporar nuevas categorias.
            while (primero != null)
            {
                CategoriaPendiente? anterior = null;
                CategoriaPendiente? actual = primero;

                while (actual != null)
                {
                    if (catalogo.Categorias.Buscar(actual.Nombre) != null || actual.NombrePadre == null || catalogo.Categorias.Buscar(actual.NombrePadre) != null)
                    {
                        if (catalogo.AgregarCategoria(actual.Nombre, actual.NombrePadre) == null) duplicados++;

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
            }
            return duplicados;
        }

        private int LeerLibros(string contenido, Catalogo catalogo)
        {
            int duplicados = 0;
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
                                        if (!LeerLibro(libro, catalogo)) duplicados++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return duplicados;
        }

        private bool LeerLibro(XmlReader lector, Catalogo catalogo)
        {
            long isbn = 0;
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
                            isbn = long.Parse(valor);
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

            return catalogo.RegistrarLibro(isbn, titulo, autor, categoria) != null;
        }
    }
}
