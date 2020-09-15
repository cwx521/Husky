using System.Threading;
using System.Threading.Tasks;
using Husky.DataAudit.Data;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Husky.DataAudit
{
	public abstract class AuditEnabledDbContext : DbContext
	{
		protected AuditEnabledDbContext(DbContextOptions options) : base(options) {
			_auditDb = new AuditDbContext(options);
		}

		protected AuditEnabledDbContext(DbContextOptions options, AuditDbContext auditDb) : base(options) {
			_auditDb = auditDb;
		}

		private readonly AuditDbContext _auditDb;

		public override int SaveChanges(bool acceptAllChangesOnSuccess) {
			if ( _auditDb == null ) {
				return base.SaveChanges(acceptAllChangesOnSuccess);
			}

			var audit = new Audit();

			audit.PreSaveChanges(this);
			var affected = base.SaveChanges(acceptAllChangesOnSuccess);
			audit.PostSaveChanges();

			_auditDb.AuditEntries.AddRange(audit.Entries);
			_auditDb.SaveChanges();

			return affected;
		}

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
			if ( _auditDb == null ) {
				return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
			}

			var audit = new Audit();
			audit.PreSaveChanges(this);
			var affected = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
			audit.PostSaveChanges();

			_auditDb.AuditEntries.AddRange(audit.Entries);
			await _auditDb.SaveChangesAsync();

			return affected;
		}

		public override int SaveChanges() => SaveChanges(true);
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await SaveChangesAsync(true, cancellationToken);
	}
}
