using System;
using System.Collections.Generic;
using System.Linq;
using DataDriven;

namespace DataDriven
{
    public class ClassDependencyResolver {
        private const string UNMARKED = "unmarked";
        private const string TEMP_MARKED = "temporary";
        private const string PERM_MARKED = "permanent";

        // takes a list of classes and sorts them so that all dependencies will be processed
        // first. Any cycles will cause an exception to be thrown
        public static List<DynamicClass> SortDependencyOrder(List<DynamicClass> dynamicClasses)
        {
            // Tarjan's algorithm for topological sorting to make sure dynamic classes
            // that depend on other dynamic classes are initialized first
            List<DynamicClass> sortedDynamicClasses = new List<DynamicClass>();
            Dictionary<string, string> nodeStatus = new Dictionary<string, string>();
            foreach (DynamicClass dynamicClass in dynamicClasses)
            {
                nodeStatus[dynamicClass.ObjectName] = UNMARKED;
            }

            while (nodeStatus.Values.Any(k => k == UNMARKED))
            {
                DynamicClass currentDynamicClass = dynamicClasses.First(dc => dc.ObjectName == nodeStatus.First(s => s.Value == UNMARKED).Key);
                Visit(currentDynamicClass, nodeStatus, dynamicClasses, sortedDynamicClasses);
            }

            return sortedDynamicClasses;
        }

        // Tarjan's algorithm for topological sorting
        private static void Visit(DynamicClass node, Dictionary<string, string> nodeStatus, List<DynamicClass> allDynamicClasses, List<DynamicClass> sortedDynamicClasses)
        {
            // if a node has already been temporarily marked, then a cycle exists
            if (nodeStatus[node.ObjectName] == TEMP_MARKED)
            {
                throw new InvalidOperationException("Cycle detected in rule set");
            }

            // temporarily mark the node as visited
            if (nodeStatus[node.ObjectName] == UNMARKED)
            {
                nodeStatus[node.ObjectName] = TEMP_MARKED;

                // find all nodes that have a dependency on the current node and recursively visit them
                foreach (DynamicClass dependentDynamicClass in allDynamicClasses.Where(dc => dc.Fields != null && dc.Fields.Any(f => f.DynamicRuntimeType != null && f.DynamicRuntimeType == node.ObjectName)))
                {
                    Visit(dependentDynamicClass, nodeStatus, allDynamicClasses, sortedDynamicClasses);
                }

                // after processing all dependent nodes, permanently mark the current node as visited
                nodeStatus[node.ObjectName] = PERM_MARKED;

                // insert the node at the front of the rule list
                sortedDynamicClasses.Insert(0, node);
            }
        }
    }
}