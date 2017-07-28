$(function () {
	$.fn.extend({
		enable: function () {
			return this.removeProp("disabled");
		},
		disable: function () {
			return this.prop("disabled", "disabled");
		},

		locate: function (x, y) {
			if (this.css("position") == "fixed") {
				y -= $(document).scrollTop();
			}
			return this.css({ left: x, top: y });
		},
		locateBeside: function (el, adjustX) {
			var p = $(el).offset(),
				w1 = $(el).outerWidth(),
				w2 = this.outerWidth(),
				h2 = this.outerHeight(),
				x = p.left + w1 + 5 + (adjustX || 0),
				y = p.top;
			if ($(document).width() < x + w2) {
				x = p.left - w2 - 5 - (adjustX || 0);
			}
			if ($(document).height() < y + h2) {
				y = p.top - (y + h2 + 15 - $(document).height());
			}
			return this.locate(x, y);
		},
		locateBelow: function (el, adjustY) {
			var p = $(el).offset();
			return this.locate(p.left, p.top + $(el).outerHeight() + 3 + (adjustY || 0));
		},
		locateCenter: function () {
			return this.locate(
				($(window).width() - this.width()) / 2,
				($(window).height() - this.height()) / 2 + $(document).scrollTop()
			);
		},

		loading: function (str) {
			return this.html("<span class='loading'><i class='fa fa-spin fa-spinner mr-1'></i>" + (str || $(this).attr("data-loading") || "Loading ...") + "</span>").show();
		}
	});
});