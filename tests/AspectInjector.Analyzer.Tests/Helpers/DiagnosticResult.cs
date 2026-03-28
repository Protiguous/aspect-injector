using System;
using Microsoft.CodeAnalysis;

namespace AspectInjector.Analyzer.Tests.Helpers
{

	/// <summary>
	///     Location where the diagnostic appears, as determined by path, line number, and column number.
	/// </summary>
	public struct DiagnosticResultLocation
	{

		public DiagnosticResultLocation(String path, Int32 line, Int32 column)
		{
			if (line < -1)
			{
				throw new ArgumentOutOfRangeException(nameof( line ), "line must be >= -1");
			}

			if (column < -1)
			{
				throw new ArgumentOutOfRangeException(nameof( column ), "column must be >= -1");
			}

			this.Path = path;
			this.Line = line;
			this.Column = column;
		}

		public String Path { get; }

		public Int32 Line { get; }

		public Int32 Column { get; }

	}

	/// <summary>
	///     Struct that stores information about a Diagnostic appearing in a source
	/// </summary>
	public struct DiagnosticResult
	{

		public static DiagnosticResult From(DiagnosticDescriptor diagnostic, Int32 line, Int32 column, String path = null)
		{
			return new DiagnosticResult
			{
				Id = diagnostic.Id,
				Message = null,
				Severity = diagnostic.DefaultSeverity,
				Locations =
					[new DiagnosticResultLocation("Test0.cs", line, column)]
			};
		}

		private DiagnosticResultLocation[] locations;

		public DiagnosticResultLocation[] Locations
		{
			get
			{
				if (this.locations == null)
				{
					this.locations = [];
				}

				return this.locations;
			}

			set => this.locations = value;
		}

		public DiagnosticSeverity Severity { get; set; }

		public String Id { get; set; }

		public String Message { get; set; }

		public String Path => this.Locations.Length > 0 ? this.Locations[0].Path : "";

		public Int32 Line => this.Locations.Length > 0 ? this.Locations[0].Line : -1;

		public Int32 Column => this.Locations.Length > 0 ? this.Locations[0].Column : -1;

	}

}