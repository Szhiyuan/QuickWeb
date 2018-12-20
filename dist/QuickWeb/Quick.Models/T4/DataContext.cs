﻿ 
/* ==============================================================================
* 命名空间：Quick.Models.Application
* 类 名 称：DataContext
* 创 建 者：Qing
* 创建时间：2018-11-29 20:27:11
* CLR 版本：4.0.30319.42000
* 保存的文件名：DataContext
* 文件版本：V1.0.0.0
*
* 功能描述：N/A
*
* 修改历史：
*
*
* ==============================================================================
*         CopyRight @ 班纳工作室 2018. All rights reserved
* ==============================================================================*/

using EFSecondLevelCache;
using Masuit.Tools.Logging;
using $safeprojectname$.Entity.Table;
using $safeprojectname$.Migrations;
using System.Data.Entity;
using System.Linq;
using static System.Data.Entity.Core.Objects.ObjectContext;
using $safeprojectname$.Validation;

namespace $safeprojectname$.Application
{
    public class DataContext : DbContext
    {
        public DataContext() :
            base(DbProvider.GetDataBaseProvider())
        {
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
#if DEBUG
            Database.Log = s =>
            {
                LogManager.Debug(typeof(Database), s);
            };
#endif
        }
		#region DbSet
		/// <summary>
        /// LoginRecord
        /// </summary>
        public virtual DbSet<LoginRecord> LoginRecord { get; set; }

		/// <summary>
        /// SystemSetting
        /// </summary>
        public virtual DbSet<SystemSetting> SystemSetting { get; set; }

		/// <summary>
        /// UserInfo
        /// </summary>
        public virtual DbSet<UserInfo> UserInfo { get; set; }

		#endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
			//base.OnModelCreating(modelBuilder);
            //设置的表的名称是一个多元化的实体类型名称版本
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			// 设置EntityFramework中decimal类型数据精度
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
			modelBuilder.Entity<UserInfo>().HasMany(e => e.LoginRecord).WithRequired(e => e.UserInfo).WillCascadeOnDelete(true);
        }

		//重写 SaveChanges
        public int SaveChanges(bool invalidateCacheDependencies = true)
        {
            return SaveAllChanges(invalidateCacheDependencies);
        }

        public int SaveAllChanges(bool invalidateCacheDependencies = true)
        {
            var changedEntityNames = GetChangedEntityNames();
            var result = base.SaveChanges();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }

        //修改、删除、添加数据时缓存失效
        private string[] GetChangedEntityNames()
        {
            return ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).Select(x => GetObjectType(x.Entity.GetType()).FullName).Distinct().ToArray();
        }
    }
}
