using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        private Dictionary<String, LinkedList<String>> graph;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            graph = new Dictionary<String, LinkedList<String>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                    int size = 0;
                    // loop through each list in the dictionary and record its size
                    foreach(LinkedList<String> list in graph.Values)
                    {
                        size += list.Count;
                    }
                    return size;
               }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                int count = 0;
                // loop through each list in the dictionary to check for s
                foreach (LinkedList<String> list in graph.Values)
                {
                    if(list.Contains(s)) // If s is found, increment count.
                        count += 1;
                }
                return count;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            // If s was ever added as a key, it must have a dependent
            if (graph.ContainsKey(s))
                return true;
            else return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            // loop through each list in the dictionary to check for s
            foreach (LinkedList<String> list in graph.Values)
            {
                if (list.Contains(s)) // If s is found, return true
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            LinkedList<String> dependents;
            // If the key s is found, poplate dependents with its associated list
            if (graph.TryGetValue(s, out dependents))
                return dependents;
         
            else return new LinkedList<string>();

        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            LinkedList<String> dependees = new LinkedList<String>();
            // loop through each list in the dictionary to check for s
            for (int i = 0; i < graph.Count; i++)
            {
                KeyValuePair<String, LinkedList<String>> pair = graph.ElementAt(i);
                if (pair.Value.Contains(s)) // If s is found in the value list
                    dependees.AddLast(pair.Key);
            }
            return dependees;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   s depends on t
        ///
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first.  S depends on T</param>
        public void AddDependency(string s, string t)
        {
            if(!graph.ContainsKey(s)) // s has not been added to the keys
            {
                // List used to store dependents
                LinkedList<String> newList = new LinkedList<String>();
                newList.AddLast(t);
                // Add new dependency to the graph
                graph.Add(s, newList);
            }
            else // s exists as a key
            {
                // Pull the list assoiated with key s
                LinkedList<String> dependents = graph[s];
                // If t is not in the list, add it
                if (!dependents.Contains(t))
                    dependents.AddLast(t);
                else return; // The ordered pair (s,t) already exists
            }
            return;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (graph.ContainsKey(s))
            {
                // Pull the list assoiated with key s
                LinkedList<String> dependents = graph[s];
                // If t is in the list, remove it
                if (dependents.Contains(t))
                {
                    if(dependents.Count <= 1) // Only one value correlates to the key
                    {
                        graph.Remove(s); // Remove key value pair
                    }
                    else
                    {
                        dependents.Remove(t);
                    }
                    return;
                }
                else return; // t was not one of s's dependents
            }
            else return; // No key s was found so there is no ordered value (s,t)
            
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            LinkedList<String> newList = new LinkedList<String>();
            // Populate a new list with the new dependents
            foreach (String dependent in newDependents)
                newList.AddLast(dependent);
            if (graph.ContainsKey(s))
            {
                graph[s] = newList; // Replace current list with new list
            }
            else return; // There was no key s
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // Remove all ordered pairs with s as a dependee
            foreach (LinkedList<String> list in graph.Values)
                list.Remove(s);
            
            // Add evrey dependee in newDependees
            foreach (String dependee in newDependees)
                AddDependency(dependee, s);
        }

    }




}
