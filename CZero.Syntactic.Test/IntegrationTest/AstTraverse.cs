using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Test.IntegrationTest
{
    public static class AstTraverse
    {
        public static void Traverse(Ast.Ast ast)
        {
            var type = ast.GetType();
            foreach (var property in type.GetProperties())
            {
                if (typeof(Ast.Ast).IsAssignableFrom(property.PropertyType))
                {
                    Console.WriteLine(property.Name);

                    // get obj and traverse
                    var node = (Ast.Ast)property.GetValue(ast);
                    if (node != null)
                    {
                        Traverse(node);
                    }
                }
                else
                {
                    bool isAstCollection = property.PropertyType.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>));

                    if (isAstCollection)
                    {
                        Console.WriteLine(property.Name);

                        // Traverse each node
                        var nodeCollection = (IReadOnlyCollection<Ast.Ast>)property.GetValue(ast);
                        foreach (var node in nodeCollection)
                        {
                            Traverse(node);
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}
