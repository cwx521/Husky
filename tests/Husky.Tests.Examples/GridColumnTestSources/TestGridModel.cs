using System;
using System.Collections.Generic;
using Husky.GridQuery;
using Husky.GridQuery.GridModeling.Annotations;

namespace Husky.Tests.Examples
{
	public class TestGridModel
	{
		public int Id { get; set; }

		[EnableRowNumber]
		public int RowNumber { get; set; }

		[Appearance(Title = "Change To Another Title")]
		public string? SomeText { get; set; }

		[Category("Numbers")]
		[EnableAggregates(GridColumnAggregates.Sum)]
		[EnableLink("/", LinkTarget = GridColumLinkTarget.Self)]
		[Appearance(Width = 200, Format = "{0:0.00 CNY}", TextAlign = TextAlign.End)]
		public decimal DecimalWithFormatAndLink { get; set; }

		[Category("Numbers")]
		[EnableLink(LinkUrl = "/", LinkTarget = GridColumLinkTarget.NewWindow)]
		[Appearance(Width = 200, KnownTemplate = GridColumnTemplate.ZeroAsEmpty, Format = "{0.00}", TextAlign = TextAlign.Center)]
		public int ZeroAsEmpty { get; set; }

		[GridColumn(Width = 240, KnownTemplate = GridColumnTemplate.TimeAgo, LinkUrl = "/", LinkTarget = GridColumLinkTarget.ModalLG, CssClass = "text-end")]
		public DateTime DateTimeWithKnownFormatAndModalLink { get; set; }


		public static List<TestGridModel> BuildTestDataSource() => new() {
			new TestGridModel {
				Id = 1,
				SomeText = "Hello",
				DecimalWithFormatAndLink = 1.23m,
				ZeroAsEmpty = 0,
				DateTimeWithKnownFormatAndModalLink = DateTime.Now.AddMinutes(-123),
			},
			new TestGridModel {
				Id = 2,
				SomeText = "To",
				DecimalWithFormatAndLink = 425.1m,
				ZeroAsEmpty = 20,
				DateTimeWithKnownFormatAndModalLink = DateTime.Now.AddMinutes(-3),
			},
			new TestGridModel {
				Id = 3,
				SomeText = "You",
				DecimalWithFormatAndLink = 19.40m,
				ZeroAsEmpty = 2023,
				DateTimeWithKnownFormatAndModalLink = DateTime.Now.AddMinutes(2023),
			},
		};
	}
}
