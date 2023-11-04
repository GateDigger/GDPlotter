using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

using Microsoft.CSharp;

namespace GDPlotter
{
    public static class RuntimeCompiler
    {
        static Func<double, double> FindMethod(Assembly assembly)
        {
            return Delegate.CreateDelegate(typeof(Func<double, double>), assembly.DefinedTypes.First().DeclaredMethods.First()) as Func<double, double>;
        }

        const string COREASSEMBLY = "mscorlib.dll";
        static Assembly BuildAssembly(string source, string coreAssembly)
        {
            CompilerParameters parameters = new CompilerParameters()
            {
                CoreAssemblyFileName = coreAssembly,
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = false,
                CompilerOptions = "/optimize",
            };

            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerResults result = provider.CompileAssemblyFromSource(parameters, source);
                return result.Errors.Count == 0 ? result.CompiledAssembly : null;
            }
        }

        const string NAMESPACE = "GDRCNS",
            CLASS = "GDRCC",
            METHOD = "GDRCM";
        static string BuildFxCode(string namespaceName, string className, string methodName, string variableName, string methodBody)
        {
            StringBuilder builder = new StringBuilder(511);

            builder.Append("using System;");
            builder.Append(Environment.NewLine);

            builder.Append("namespace ");
            builder.Append(namespaceName);
            builder.Append(Environment.NewLine);
            builder.Append("{");
            builder.Append(Environment.NewLine);
            {
                builder.Append("public static class ");
                builder.Append(className);
                builder.Append("{");
                builder.Append(Environment.NewLine);
                {
                    builder.Append("public static double ");
                    builder.Append(methodName);
                    builder.Append("(double ");
                    builder.Append(variableName);
                    builder.Append(")");
                    builder.Append("{");
                    builder.Append(Environment.NewLine);
                    {
                        builder.Append(methodBody);
                    }
                    builder.Append(Environment.NewLine);
                    builder.Append("}");
                }
                builder.Append(Environment.NewLine);
                builder.Append("}");
            }
            builder.Append(Environment.NewLine);
            builder.Append("}");

            return builder.ToString();
        }

        public static Func<double, double> CompileFunction(string variableName, string body, string coreAssembly = COREASSEMBLY)
        {
            string sourceCode = BuildFxCode(NAMESPACE, CLASS, METHOD, variableName, body);
            Assembly resultAssembly = BuildAssembly(sourceCode, coreAssembly);

            if (resultAssembly != null)
                return FindMethod(resultAssembly);
            return null;
        }
    }
}