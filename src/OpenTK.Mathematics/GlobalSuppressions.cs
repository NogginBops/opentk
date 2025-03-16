// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1305:Field names should not use Hungarian notation",
    Justification = "There are a lot of short variable names (especially for matrix elements) in the Mathematics library, so instead of changing 500-1000 variable names, we just suppress this message instead."
)]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "In physics temperature is denoted with capital T.", Scope = "member", Target = "~M:OpenTK.Mathematics.RGBColorSpace.DIlluminantTemperatureToxy(System.Single)~OpenTK.Mathematics.Vector2")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "In physics temperature is denoted with capital T.", Scope = "member", Target = "~M:OpenTK.Mathematics.RGBColorSpace.DIlluminantTemperatureToxy(System.Single)~OpenTK.Mathematics.Vector2")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "In physics temperature is denoted with capital T.", Scope = "member", Target = "~M:OpenTK.Mathematics.RGBColorSpace.TemperatureToxy(System.Single)~OpenTK.Mathematics.Vector2")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "In physics temperature is denoted with capital T.", Scope = "member", Target = "~M:OpenTK.Mathematics.RGBColorSpace.TemperatureToxy(System.Single)~OpenTK.Mathematics.Vector2")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "A lot of color formats use lower and capital case for color channels.", Scope = "namespaceanddescendants", Target = "~N:OpenTK.Mathematics.Colors")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "A lot of color formats use lower and capital case for color channels.", Scope = "namespaceanddescendants", Target = "~N:OpenTK.Mathematics.Colors")]
