﻿using System;
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
		public static HuskyInjector AddLocalFileBucket(this HuskyInjector husky, string fileBucketRootPath) {
			husky.Services.AddSingleton<IFileBucket>(new LocalFileBucket(fileBucketRootPath));
			return husky;
		}

		public static HuskyInjector AddAliyunOssBucket(this HuskyInjector husky, AliyunOssBucketOptions options) {
			husky.Services.AddSingleton<ICloudFileBucket>(new AliyunOssBucket(options));
			return husky;
		}

		public static HuskyInjector AddAliyunOssBucket(this HuskyInjector husky, Action<AliyunOssBucketOptions> setupAction) {
			var options = new AliyunOssBucketOptions();
			setupAction(options);
			husky.Services.AddSingleton<ICloudFileBucket>(new AliyunOssBucket(options));
			return husky;
		}

		public static HuskyInjector AddFileStoreLogger(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IFileStoreDbContext, FileStoreDbContext>(optionsAction);
			husky.Services.AddScoped<IFileStoreLogger, FileStoreLogger>();
			return husky;
		}

		public static HuskyInjector AddFileStoreLogger<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IFileStoreDbContext {
			husky.Services.AddDbContext<IFileStoreDbContext, TDbContext>();
			husky.Services.AddScoped<IFileStoreLogger, FileStoreLogger>();
			return husky;
		}
	}
}