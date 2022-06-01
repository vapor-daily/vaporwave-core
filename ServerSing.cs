using Agile.AServer;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace vaporwave_core
{
    internal class ServerSing
    {

        private static Server? serverSing;

        private static readonly object locker = new();

        private ServerSing()
        {

        }

        private static Server GetInstance()
        {
            lock (locker) if (serverSing == null) serverSing = new Server();
            return serverSing;
        }

        public static void AddHandler(HttpHandler head)
        {
            GetInstance().AddHandler(head);
        }

        public static void Run()
        {
            GetInstance().SetIP("0.0.0.0");
            var jsonPath = Path.Combine(Environment.CurrentDirectory, @"app.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    Newtonsoft.Json.Linq.JObject? app = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(File.ReadAllText(jsonPath, Encoding.UTF8));
                    if (app != null)
                    {
                        int? port = (int?)app["port"];
                        if (port != null)
                        {
                            GetInstance().SetPort((int)port).Run();
                        }
                        else
                        {
                            GetInstance().SetPort(10086).Run();
                        }
                    }
                    else
                    {
                        LogSing.LogError("app.json 序列化異常");
                    }
                }
                catch (Exception)
                {
                    LogSing.LogError("app.json 转换异常");
                }
            }
            else
            {
                LogSing.LogError(jsonPath + "app.json 找不到");
            }
            Console.ReadLine();
        }

        public static void RouteRegister(string pkgName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                MethodBase? methodBase = MethodBase.GetCurrentMethod();
                Type? declaringType = methodBase?.DeclaringType;
                string? @namespace = declaringType?.Namespace;
                string? name = assembly.GetName().Name;
                if (methodBase != null && declaringType != null && @namespace != null && @namespace.Replace("_", "").Replace("-", "").Equals(assembly.GetName().Name.Replace("_", "").Replace("-", "")))
                {
                    var m = assembly.GetTypes();
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (t.FullName != null && t.FullName.StartsWith($"{@namespace}.{pkgName}") && !CompilerGen(t) && t.FullName.IndexOf("<>c") == -1)
                        {
                            object? r = Activator.CreateInstance(t);
                            if (r != null)
                            {
                                Type type = r.GetType();
                                LogSing.LogInfo($"API.{type.Name}>>>    registration success");
                            }
                        }
                    }
                    break;
                }
            }
        }

        private static bool CompilerGen(Type t) => Attribute.GetCustomAttribute(t, typeof(CompilerGeneratedAttribute)) != null;

    }
}
