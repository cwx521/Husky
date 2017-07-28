$(function () {

	// sending dynamic pass code
	$(".dynamic-code-sender").click(function () {
		var $this = $(this);
		if ($this.data("sending")) {
			return;
		}

		var label = $this.html();
		var countDown = function () {
			var n = parseInt($this.html().replace("秒", ""));
			if (n-- > 0) {
				$this.data("sending", true).html(n.toString() + "秒");
				setTimeout(countDown, 1000);
			} else {
				$this.removeData("sending").html(label);
			}
		}

		$this.html("60秒");
		countDown();

		var to = $this.data("send-to");
		if (to) {
			$.post("/api/SendDynamicCode", to, function () {
				$("[data-valmsg-for=DynamicPassCode]")
					.removeClass("field-validation-error")
					.addClass("field-validation-valid")
					.html("已发送到 " + to + "，请注意查收。")
			});
		}
	});
	$(".dynamic-code-sender").click();
});