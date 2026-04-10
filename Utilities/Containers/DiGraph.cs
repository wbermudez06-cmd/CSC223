using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Serialization;
using Xunit.Abstractions;

/// <summary>
/// Represents a generic directed graph (DiGraph) using an adjacency list.
/// Each vertex maps to a collection of its outgoing neighbors.
public class DiGraph<T> where T : notnull
{
    /// <summary>
    /// Internal adjacency list representation of the graph.
    /// Key = vertex, Value = list of neighboring vertices (outgoing edges).
    /// </summary>
    protected Dictionary<T, DLL<T>> _adjacencyList;

    /// <summary>
    /// Initializes a new empty directed graph.
    /// </summary>
    public DiGraph()
    {
        _adjacencyList = new Dictionary<T, DLL<T>>();
    }

    /// <summary>
    /// Adds a vertex to the graph if it does not already exist.
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    public bool AddVertex(T vertex)
    {
        // returns false if vertex already exists in the graph
        if (_adjacencyList.ContainsKey(vertex)) return false;

        // adds the vertex to the graph and returns true
        _adjacencyList[vertex] = new DLL<T>();
        return true;
    }

    /// <summary>
    /// Adds a directed edge from a source vertex to a destination vertex.
    /// </summary>
    /// <param name="source">The starting vertex.</param>
    /// <param name="destination">The ending vertex.</param>
    public bool AddEdge(T source, T destination)
    {
        // if one of the vertexes is not found in the graph, thrown an exception
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination))
            throw new ArgumentException("Both vertexes are not present in the graph.");

        // if it already has an edge to the corresponding destination, returns false
        if (_adjacencyList[source].Contains(destination)) return false;

        // add edge to the source vertex and returns true
        _adjacencyList[source].Add(destination);
        return true;
    }

    /// <summary>
    /// Removes vertex and all edges connected to it.
    /// </summary>
    /// <param name="vertex"></param>
    public bool RemoveVertex(T vertex)
    {
        // if the vertex does not exist in the dictionary return false
        if (!_adjacencyList.ContainsKey(vertex)) return false;

        // Remove the vertex from dictionary
        _adjacencyList.Remove(vertex);

        // removes edges connected to that vertex in other vertexes
        foreach (KeyValuePair<T, DLL<T>> checkVertex in _adjacencyList)
        {
            if (checkVertex.Value.Contains(vertex))
            {
                checkVertex.Value.Remove(vertex);
            }

        }
        // return true
        return true;
    }

    /// <summary>
    /// Removes a directed edge from source to destination.
    /// </summary>
    /// <param name="source">The starting vertex.</param>
    /// <param name="destination">The ending vertex.</param>
    public bool RemoveEdge(T source, T destination)
    {
        // throws and exception if one vertex does not exist in the dictionary
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination))
            throw new ArgumentException("One or both of indicated vertexes do not exist.");

        // returns false if edge does not exist
        if (!_adjacencyList[source].Contains(destination)) return false;

        // remove edge from source vertex and returns true
        _adjacencyList[source].Remove(destination);
        return true;
    }

    /// <summary>
    /// Determines whether a directed edge exists from source to destination.
    /// </summary>
    /// <param name="source">The starting vertex.</param>
    /// <param name="destination">The ending vertex.</param>
    public bool HasEdge(T source, T destination)
    {
        // returns false if either vertex does not exist in the dictionary
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination)) return false;

        // returns if an edge is found between vertexes
        return _adjacencyList[source].Contains(destination);
    }

    /// <summary>
    /// Gets all neighboring vertices (outgoing edges) for a given vertex.
    /// </summary>
    /// <param name="vertex">The vertex whose neighbors are requested.</param>
    public List<T> GetNeighbors(T vertex)
    {
        // throws and exeption if vertex is not found in the dictionary
        if (!_adjacencyList.ContainsKey(vertex))
            throw new ArgumentException("Vertex does not exist.");

        // initializes a list of results
        List<T> results = new List<T>();

        // loops through the edge list and adds neighbors to the list
        foreach (T item in _adjacencyList[vertex])
        {
            results.Add(item);
        }
        // return list
        return results;
    }

    /// <summary>
    /// Returns all vertices in the graph.
    /// </summary>
    public IEnumerable<T> GetVertices()
    {
        return _adjacencyList.Keys;
    }

    /// <summary>
    /// Gets the total number of vertices in the graph.
    /// </summary>
    public int VertexCount()
    {
        return _adjacencyList.Keys.Count();
    }

    /// <summary>
    /// Gets the total number of directed edges in the graph.
    /// </summary>
    public int EdgeCount()
    {
        int count = 0;
        // loops through dlls connected to vertex to find number
        foreach (T vertex in _adjacencyList.Keys)
        {
            count += _adjacencyList[vertex].Count();
        }
        // return final count
        return count;
    }

    /// <summary>
    /// Returns a string representation of the graph as adjacency lists.
    /// </summary>
    public override string ToString()
    {
        // initializes an empty list to store the strings
        List<string> strings = new List<string>();

        // loops through the pairs in the adjacency list
        foreach (KeyValuePair<T, DLL<T>> item in _adjacencyList)
        {
            // Add vertex key to list of string
            strings.Add($"{item.Key.ToString()} -->");
            foreach (T val in item.Value)
            {
                // adds the values indicated in the dll to the list
                strings.Add($"{val.ToString()},");
            }
            // adds a newline in between each vertex
            strings.Add("\n");
        }
        // returns the joined strings contained in the list
        return string.Join("", strings);

    }
}

