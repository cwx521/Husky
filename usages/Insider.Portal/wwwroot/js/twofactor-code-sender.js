$(function () {

	// sending two-factor pass code
	var triggerBtn = $(".twofactor-code-sender");

	triggerBtn.click(function () {
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
		var purpose = $this.data("purpose");
		if (to) {
			$.post("/api/SendTwoFactorCode", { to: to, purpose: purpose }, function (data) {
				if (data.ok) {
					$("[data-valmsg-for=TwoFactorCode]")
						.removeClass("field-validation-error")
						.addClass("field-validation-valid")
						.html("已发送到 " + to + "，请注意查收。")
				}
				else {
					$("[data-valmsg-for=TwoFactorCode]")
						.addClass("field-validation-error")
						.removeClass("field-validation-valid")
						.html("发送失败，请重试。")

					$this.removeData("sending").html(label);
				}
			});
		}
	});

	if (triggerBtn.data("auto") === "True") {
		triggerBtn.click();
	}
});