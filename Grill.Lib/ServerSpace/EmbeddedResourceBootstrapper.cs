using System;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.Session;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace Grill.Lib.ServerSpace
{
    /// <summary>
    /// Class that enabled using embedded resources from this library
    /// </summary>
    public class EmbeddedResourceBootstrapper : DefaultNancyBootstrapper
    {
        const string StaticDir = "static";
        private byte[] _favIcon;
        private static Assembly _assembly;

        protected override NancyInternalConfiguration InternalConfiguration => 
            NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder);

        private string _viewsNameSpace => typeof (GrillServer).Namespace + ".Views";

        /// <summary>
        /// Favicon as byte array
        /// </summary>
        protected override byte[] FavIcon => _favIcon ?? (_favIcon = LoadFavIcon());
        /// <summary>
        /// Assembly where this class is stored
        /// </summary>
        protected static Assembly Assembly => _assembly ?? (_assembly = LoadAssembly());


        
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            // Enable Session where data can be stored
            CookieBasedSessions.Enable(pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {

            var rootNamespace = _viewsNameSpace;
           base.ConfigureApplicationContainer(container);

            if (ResourceViewLocationProvider.RootNamespaces.Values.Contains(rootNamespace))
                return;

            ResourceViewLocationProvider.RootNamespaces.Add(Assembly, rootNamespace);
        }


        protected override void ConfigureConventions(NancyConventions conventions)
        {
            var staticNamespace = $"{_viewsNameSpace}.{StaticDir}";

            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.Add(AddStaticResourcePath($"/{StaticDir}", Assembly, staticNamespace));
        }

        static void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }



        /// <summary>
        /// Add redirections for all of the static content
        /// </summary>
        /// <param name="requestedPath"></param>
        /// <param name="assembly"></param>
        /// <param name="namespacePrefix"></param>
        /// <returns></returns>
        public static Func<NancyContext, string, Response> AddStaticResourcePath(string requestedPath, Assembly assembly, string namespacePrefix)
        {
            return (context, s) =>
            {
                var path = context.Request.Path;
                if (!path.StartsWith(requestedPath))
                {
                    return null;
                }
                if (path.Equals(requestedPath))
                    return string.Empty; // root "static" dir url

                string resourcePath;
                string name;

                var adjustedPath = path.Substring(requestedPath.Length + 1);
                if (adjustedPath.IndexOf('/') >= 0)
                {
                    name = Path.GetFileName(adjustedPath);
                    resourcePath = namespacePrefix + "." + adjustedPath.Substring(0, adjustedPath.Length - name.Length - 1).Replace('/', '.');
                }
                else
                {
                    name = adjustedPath;
                    resourcePath = namespacePrefix;
                }

                if (string.IsNullOrWhiteSpace(name))
                    return string.Empty; // directory path without filename defined
                return new EmbeddedFileResponse(assembly, resourcePath, name);
            };
        }

        /// <summary>
        /// Load web page favicon from embedded resource
        /// </summary>
        /// <returns></returns>
        private static byte[] LoadFavIcon()
        {
            var favIconImageStream = Assembly.GetManifestResourceStream("Grill.Lib.ServerSpace.Views.static.images.favicon.png");
            using (var ms = new MemoryStream())
            {
                
                favIconImageStream?.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Load assembly containing this class
        /// </summary>
        /// <returns></returns>
        private static Assembly LoadAssembly()
        {
            return Assembly.GetAssembly(typeof (GrillServer));
        }
    }
}
