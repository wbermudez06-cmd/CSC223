public class DiGraph<T> where T : notnull
{
    //_adjacencylist is the entire graph, _adjacencylist[vertex] is the adjacency list of a vertex
    protected Dictionary<T, DLL<T>> _adjacencyList;

    /// <summary>
    /// Adds a vertex to the graph if it does not already exist.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public bool AddVertex(T vertex)
    {
        //Vertex already exists, so cannot add vertex. Return false.
        if (_adjacencyList.ContainsKey(vertex)) return false;

        //Vertex does not exist, so add vertex and return true.
        _adjacencyList[vertex] = new DLL<T>();
        return true;
    }

    /// <summary>
    /// Adds a directed edge from source to destination, if it does not already exist.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool AddEdge(T source, T destination)
    {
        //ArgumentException if source or destination vertex does not exist in graph.
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination)) throw new ArgumentException("Source or destination vertex does not exist in graph.");

        //Destination already exists within source's adjacency list, so edge already exists. 
        //Return false; cannot add edge
        if (_adjacencyList[source].Contains(destination)) return false;

        //Destination does not exist within source's adjacency list, so edge does not exist. 
        //Add edge and return true.
        _adjacencyList[source].Add(destination);
        return true;
    }

    /// <summary>
    /// Removes a vertex and all edges connected to it.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public bool RemoveVertex(T vertex)
    {
        //Vertex does not exist, so cannot remove vertex. Return false.
        if (!_adjacencyList.ContainsKey(vertex)) return false;
    
        //Vertex exists, so remove vertex and all edges connected to it. Return true.
        _adjacencyList.Remove(vertex);
        //Remove all edges connected to vertex by removing vertex from all adjacency lists.
        foreach (var adjList in _adjacencyList.Values)
        {
            adjList.Remove(vertex);
        }
        return true;
    }

    /// <summary>
    /// Removes a directed edge from source to destination.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool RemoveEdge(T source, T destination)
    {
        //ArgumentException if source or destination vertex does not exist in graph.
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination)) throw new ArgumentException("Source or destination vertex does not exist in graph.");

        //Edge does not exist, so cannot remove edge. Return false.
        if (!_adjacencyList[source].Contains(destination)) return false;

        //Edge exists, so remove edge and return true.
        _adjacencyList[source].Remove(destination);
        return true;
    }

    /// <summary>
    /// Checks if an edge exists from source to destination.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool HasEdge(T source, T destination)
    {
        return _adjacencyList[source].Contains(destination);
    }

    /// <summary>
    /// Returns all vertices adjacent to the specified vertex.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public List<T> GetNeighbors(T vertex)
    {
        //ArgumentException if vertex doesn't exist in graph
        if (!_adjacencyList.ContainsKey(vertex)) throw new ArgumentException("Vertex does not exist in graph.");

        return _adjacencyList[vertex].ToList();
    }

    /// <summary>
    /// Returns all vertices in the graph as an iterable container.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetVertices()
    {
        return _adjacencyList.Keys;
    }

    /// <summary>
    /// Returns the number of vertices in the graph.
    /// </summary>
    /// <returns></returns>
    public int VertexCount()
    {
        return _adjacencyList.Count;
    }

    /// <summary>
    /// Returns the number of edges in the graph.
    /// </summary>
    /// <returns></returns>
    public int EdgeCount()
    {
        int count = 0;
        //Look through each vertices' adjacency list and add the number of edges to "count"
        foreach (var adjList in _adjacencyList.Values)
        {
            count += adjList.Count;
        }
        return count;
    }

    /// <summary>
    /// Returns a string representation of the graph.
    /// </summary>
    /// <returns></returns>
    public string ToString()
    {
        
    }
}