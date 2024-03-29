﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace scorpioweb
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }


        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        //public static PagedData<T> PagedResult<T>(this List<T> list, int pageNumber, int pageSize) where T: class
        //{
        //    var result = new PagedData<T>();
        //    result.Data = list.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        //    result.TotalPages = Convert.ToInt32(Math.Ceiling((double)list.Count() / pageSize));
        //    result.CurrentPage = pageNumber;
        //    return result;
        //}
    }
}
