var ajaxRequestNotificationTimer = null;
$(function () {
	function unobtrusiveAjaxRequest(e) {
		var el = $(this);
		if (el.data("ajax-oneoff") == null || el.data("ajax-complete") == null) {

			var url = el.attr("href") || el.data("href") || el.attr("action");
			var pane = el.data("ajax-target") || el.data("target") || "body";
			var loading = el.data("ajax-loading") || el.data("loading") || pane;
			var form = el.is("form") ? this : el.data("ajax-form");
			var delay = parseInt(el.data("ajax-delay") || 0);
			var update = el.data("update");

			var wrapMessage = function (resp) {
				return "<span class='text-" + (resp.ok ? "success" : "danger") + "'>" +
					"<i class='fa fa-" + (resp.ok ? "check" : "times") + "-circle mr-1'></i>" +
					(resp.message || (resp.ok ? "成功。" : "执行失败。")) +
					"</span>";
			};

			if (form) {
				var validationInfo = $(form).data("unobtrusiveValidation");
				if (validationInfo && validationInfo.validate && !validationInfo.validate()) {
					return false;
				}
				e.preventDefault();
			}

			if (loading != "disabled") {
				$(loading).loading(form ? "马上就好..." : "努力载入中...");
			}

			$.ajaxSetup({ cache: false });
			var promise = $.ajax({
				url: url,
				method: form ? "POST" : "GET",
				data: form ? $(form).serialize() : null,
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					$(pane).html(red(errorThrown)).show();
					if (loading != pane) {
						$(loading).hide();
					}
				},
				success: function (resp) {
					clearTimeout(ajaxRequestNotificationTimer);
					el.data("ajax-complete", "true");
					el.data("ajax-excuted", "true");

					var timer = setTimeout(function () {
						clearTimeout(timer);

						//if the loading status label is not the same one of the content container, then hide loading
						if (loading != pane) {
							$(loading).hide();
						}
						//if it trigs an update panel, then refresh it.
						if (update) {
							$(update + "[data-load]").load($(update).data("load"));
						}
						//if the responseData is JSON else is HTML
						if (resp.ok === "undefined" || resp.message === "undefined") {
							$(pane).html(resp).show();
						}
						else {
							$(pane).html(wrapMessage(resp)).show();
							if (resp.ok) {
								//if successful, hide the success message automatically after a while.
								ajaxRequestNotificationTimer = setTimeout(function () {
									clearTimeout(ajaxRequestNotificationTimer);
									$(pane).hide();
								}, 5000);
							}
						}
					}, delay);
				}
			});
			$(form).data("promise", promise);
		}
	};

	$(document).on("click", "a[data-ajax=weixing][href]", unobtrusiveAjaxRequest);
	$(document).on("click", "button[data-ajax=weixing][data-href]", unobtrusiveAjaxRequest);
	$(document).on("submit", "form[data-ajax=weixing][action]", unobtrusiveAjaxRequest);
});