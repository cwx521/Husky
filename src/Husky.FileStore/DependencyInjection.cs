using System;
using Husky.FileStore;
using Husky.FileStore.AliyunOss;
using Husky.FileStore.Data;
using Husky.FileStore.LocalStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddLocalFileBucket(this HuskyServiceHub husky, string fileBucketRootPath) {
			husky.Services.AddSingleton<IFileBucket>(new LocalFileBucket(fileBucketRootPath));
			return husky;
		}

		public static HuskyServiceHub AddAliyunOssBucket(this HuskyServiceHub husky, AliyunOssBucketOptions options) {
			husky.Services.AddSingleton<ICloudFileBucket>(new AliyunOssBucket(options));
			return husky;
		}

		public static HuskyServiceHub AddAliyunOssBucket(this HuskyServiceHub husky, Action<AliyunOssBucketOptions> setupAction) {
			var options = new AliyunOssBucketOptions();
			setupAction(options);
			husky.Services.AddSingleton<ICloudFileBucket>(new AliyunOssBucket(options));
			return husky;
		}

		public static HuskyServiceHub AddFileStoreLogger(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IFileStoreDbContext, FileStoreDbContext>(optionsAction);
			husky.Services.AddScoped<IFileStoreLogger, FileStoreLogger>();
			return husky;
		}

		public static HuskyServiceHub AddFileStoreLogger<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, IFileStoreDbContext {
			husky.Services.AddDbContext<IFileStoreDbContext, TDbContext>();
			husky.Services.AddScoped<IFileStoreLogger, FileStoreLogger>();
			return husky;
		}
	}
}