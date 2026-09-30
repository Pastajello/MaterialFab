namespace MaterialFab;

/// <summary>Registration of the MaterialFab handlers.</summary>
public static class AppHostBuilderExtensions
{
	/// <summary>Registers the <see cref="FloatingActionButton"/> and <see cref="ExtendedFloatingActionButton"/> handlers.</summary>
	public static MauiAppBuilder UseMaterialFab(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler<FloatingActionButton, FloatingActionButtonHandler>();
			handlers.AddHandler<ExtendedFloatingActionButton, ExtendedFloatingActionButtonHandler>();
		});

		return builder;
	}
}
