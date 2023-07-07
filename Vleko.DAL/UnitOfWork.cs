using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vleko.DAL.Interface;

namespace Vleko.DAL
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        public UnitOfWork(TDbContext context)
        {
            _context = context;
        }
        public IQueryable<TEntity> Entity<TEntity>() where TEntity : class, IEntity
        {
            return _context.Set<TEntity>();
        }
        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> Commit()
        {
            try
            {
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }
        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> Commit<T>(Func<T> f)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    f();
                    var save = await SaveChanges();
                    await transaction.CommitAsync();
                    return (true, "success", null, save.log);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, ex.Message, ex, null);
                }
            }
        }
        public void Dispose()
        {
            System.Threading.Thread.Sleep(1000);
            _context.Dispose();
        }

        #region Command

        #region Add
        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Add<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().AddRange(items);
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> AddSave<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> AddSave<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().AddRange(items);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }
        #endregion

        #region Update
        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Update<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().UpdateRange(items);
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> UpdateSave<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().Update(entity);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> UpdateSave<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().UpdateRange(items);
                await _context.SaveChangesAsync();
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }
        #endregion

        #region Delete
        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Delete<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            _context.Set<TEntity>().RemoveRange(items);
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> DeleteSave<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> DeleteSave<TEntity>(IEnumerable<TEntity> items) where TEntity : class, IEntity
        {
            try
            {
                _context.Set<TEntity>().RemoveRange(items);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }

        #endregion

        #region Query
        public void ExecuteQuery(string query)
        {
            _context.Database.ExecuteSqlRaw(query);
        }

        public async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> ExecuteQuerySave(string query)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(query);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }
        #endregion

        #endregion

        #region Query

        public async Task<List<TEntity>> List<TEntity>(IQueryable<TEntity> query) where TEntity : IEntity
        {
            return await query.ToListAsync();
        }
        public async Task<TEntity> Single<TEntity>(IQueryable<TEntity> query) where TEntity : IEntity
        {
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> Count<TEntity>(IQueryable<TEntity> query) where TEntity : IEntity
        {
            return await query.CountAsync();
        }
        public async Task<bool> Any<TEntity>(IQueryable<TEntity> query) where TEntity : IEntity
        {
            return await query.AnyAsync();
        }
        public async Task<(bool Success, string Message, List<T> Result, Exception? ex)> ListQuery<T>(string query) where T : class
        {
            try
            {
                var result = await _context.Database.SqlQueryRaw<T>(query).ToListAsync();
                return (true, "success", result, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null, ex);
            }
        }

        public async Task<(bool Success, string Message, T Result, Exception? ex)> SingleQuery<T>(string query) where T : class
        {
            try
            {
                var result = await _context.Database.SqlQueryRaw<T>(query).FirstOrDefaultAsync();
                return (true, "success", result, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null, ex);
            }
        }
        public async Task<(bool Success, string Message, List<Dictionary<string, string>> Result, Exception ex)> DynamicQuery(string query)
        {
            try
            {
                List<Dictionary<string, string>> listDictionary = new List<Dictionary<string, string>>();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                        columns = columns.Distinct().ToList();
                        while (reader.Read())
                        {
                            var dictionary = new Dictionary<string, string>();
                            foreach (var column in columns)
                            {
                                dictionary.Add(column, reader[column].ToString());
                            }
                            listDictionary.Add(dictionary);
                        }
                    }
                    await _context.Database.CloseConnectionAsync();
                }
#nullable disable
                return (true, "Success", listDictionary, null);
#nullable enable
            }
            catch (Exception ex)
            {
#nullable disable
                return (false, ex.Message, null, ex);
#nullable enable
            }
        }
        #endregion


        #region Save Changes
        private async Task<(bool Success, string Message, Exception? ex, List<ChangeLog>? log)> SaveChanges()
        {
            try
            {
                var modified = _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
                var add = _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
                var delete = _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).ToList();
                List<ChangeLog> changelog = new List<ChangeLog>();
                await _context.SaveChangesAsync();

                foreach (var entry in modified)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var primaryKey = GetPrimaryKeyValue(entry);
                    changelog.Add(new ChangeLog()
                    {
                        Entity = entityName,
                        PrimaryKey = primaryKey,
                        Type = ChangeLogType.EDIT,
                        Property = entry.OriginalValues.Properties.Where(d => (entry.CurrentValues[d]?.ToString() ?? "-") != (entry.OriginalValues[d]?.ToString() ?? "-"))
                                .Select(d => new ChangeLogProperties()
                                {
                                    NewValue = entry.CurrentValues[d]?.ToString() ?? "-",
                                    OldValue = entry.OriginalValues[d]?.ToString() ?? "-",
                                    Property = d.Name
                                }).ToList(),
                        DateChanged = DateTime.UtcNow
                    });
                }
                foreach (var entry in delete)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var primaryKey = GetPrimaryKeyValue(entry);
                    changelog.Add(new ChangeLog()
                    {
                        Entity = entityName,
                        PrimaryKey = primaryKey,
                        Type = ChangeLogType.DELETE,
                        DateChanged = DateTime.UtcNow
                    });
                }
                foreach (var entry in add)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var primaryKey = GetPrimaryKeyValue(entry);
                    changelog.Add(new ChangeLog()
                    {
                        Entity = entityName,
                        PrimaryKey = primaryKey,
                        Type = ChangeLogType.ADD,
                        Property = entry.OriginalValues.Properties.Select(d => new ChangeLogProperties()
                        {
                            NewValue = entry.CurrentValues[d]?.ToString() ?? "-",
                            Property = d.Name
                        }).ToList(),
                        DateChanged = DateTime.UtcNow
                    });
                }
                return (true, "success", null, changelog);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, ex, null);
            }
        }
        string GetPrimaryKeyValue(EntityEntry entity)
        {
            string? result = entity.Metadata.FindPrimaryKey()?.Properties.Select(p => entity.Property(p.Name).CurrentValue)?.FirstOrDefault()?.ToString();
            return result ?? "-";
        }
        #endregion

    }
}
