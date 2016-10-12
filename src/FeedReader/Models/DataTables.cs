using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    /// <summary>
    /// Object representation to bind to the request coming in from the front end from the Datatables plugin.
    /// Field names are lowercase for correct binding to occur.
    /// </summary>
    public class DTableRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public ICollection<Column> columns { get; set; }
        public ICollection<Order> order { get; set; }  
    }

    /// <summary>
    /// Object representation of the response to the Datatables request on the front end. A generic is used so that different datatypes can be used with the request
    /// </summary>
    /// <typeparam name="T">Object type of the data element in the response</typeparam>
    public class DTableResponse<T>
    {
        public DTableResponse(ICollection<T> data)
        {
            this.data = data;
        }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public ICollection<T> data { get; set; }
        public string error { get; set; }
    }

    /// <summary>
    /// DataTables internal search representation
    /// </summary>
    public class Search
    {
        public bool Regex { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// Datatables internal column representation
    /// </summary>
    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
        public bool searchable { get; set; }
    }
    /// <summary>
    /// Datatables internal Order representation
    /// </summary>
    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}