using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CZero.Syntactic.Test.IntegrationTest
{
    public class AstTraverse
    {
        public List<string> NodeLog { get; } = new List<string>();

        public void Traverse(Ast.Ast ast)
        {
            // Log
            var name = ast.GetType().Name;
            Debug.Assert(name.EndsWith("Ast"), name);
            name = name[..^3];// remove Ast
            NodeLog.Add(name);

            foreach (var property in ast.GetType().GetProperties())
            {
                if (typeof(Ast.Ast).IsAssignableFrom(property.PropertyType))
                {
                    // get node and traverse
                    var node = (Ast.Ast)property.GetValue(ast);
                    if (node != null)
                    {
                        Traverse(node);
                    }
                }
                else
                {
                    if (typeof(IEnumerable<Ast.Ast>).IsAssignableFrom(property.PropertyType))
                    {
                        // Traverse each node
                        var nodeCollection = (IReadOnlyCollection<Ast.Ast>)property.GetValue(ast);
                        foreach (var node in nodeCollection)
                        {
                            Traverse(node);
                        }
                    }
                }
            }
        }

        public List<(string Name, int Count)> GetStatistics()
        {
            return NodeLog.GroupBy(p => p).Select(g => (g.Key, g.Count())).ToList();
        }
    }
}
