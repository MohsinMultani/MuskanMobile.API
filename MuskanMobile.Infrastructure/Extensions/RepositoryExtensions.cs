using Microsoft.EntityFrameworkCore;
using MuskanMobile.Application.Interfaces;
using System;
using System.Linq;

namespace MuskanMobile.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static IQueryable<T> GetQueryable<T>(this IRepository<T> repository) where T : class
        {
            // This requires adding this method to your IRepository interface
            // Or you can inject DbContext directly in services when needed
            throw new NotImplementedException("Add this method to IRepository<T>");
        }
    }
}