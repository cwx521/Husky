using System;

namespace Husky
{
	public class Pagination
	{
		private int _p = 1;

		public int PageIndex {
			get {
				if ( RecordCount > 0 ) {
					_p = Math.Min(_p, PageCount);
				}
				_p = Math.Max(_p, 1);
				return _p;
			}
			set => _p = value;
		}

		public int PageSize { get; set; } = 20;
		public int RecordCount { get; set; }

		public int SkipOffset => (PageIndex - 1) * PageSize;
		public int PageCount => RecordCount / PageSize + (RecordCount % PageSize == 0 ? 0 : 1);
	}
}
