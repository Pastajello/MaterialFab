namespace MaterialFab.Sample.Pages;

public partial class ScrollPage : ContentPage
{
	const string Lorem =
		"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
		"Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
		"Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";

	public ScrollPage()
	{
		InitializeComponent();

		for (var i = 1; i <= 30; i++)
		{
			Paragraphs.Children.Add(new Label { Text = $"Paragraph {i}", FontAttributes = FontAttributes.Bold });
			Paragraphs.Children.Add(new Label { Text = Lorem });
		}
	}

	void OnThresholdChanged(object? sender, ValueChangedEventArgs e)
	{
		ScrollBehavior.Threshold = Math.Round(e.NewValue);
		ThresholdLabel.Text = $"Threshold: {ScrollBehavior.Threshold}";
	}

	async void OnEditClicked(object? sender, EventArgs e) =>
		await DisplayAlertAsync("Edit", "The FAB was tapped.", "OK");
}
