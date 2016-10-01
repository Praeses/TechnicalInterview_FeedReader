using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class DTableRequest
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public ICollection<Column> columns { get; set; }
        public ICollection<Order> order { get; set; }  
    }

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

    public class Search
    {
        public bool regex { get; set; }
        public string value { get; set; }
    }
    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
        public bool searchable { get; set; }
    }
    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}