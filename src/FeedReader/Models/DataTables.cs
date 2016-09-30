using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class DTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public ICollection<Column> Columns { get; set; }
        public ICollection<Order> Order { get; set; }  
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
        public bool Regex { get; set; }
        public string Value { get; set; }
    }
    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
        public bool Searchable { get; set; }
    }
    public class Order
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}