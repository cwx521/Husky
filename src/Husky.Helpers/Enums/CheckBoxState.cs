namespace Husky
{
	public enum CheckBoxState
	{
		[Label("", Description = "不选中")]
		Unchecked,

		[Label("checked", Description = "选中")]
		Checked,

		[Label("disabled", Description = "禁用")]
		Disabled,

		[Label("hidden", Description = "不显示")]
		NoDisplay
	}
}
