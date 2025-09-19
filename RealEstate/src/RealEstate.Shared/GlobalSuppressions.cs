using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Namespaces de Shared/Application usan nombres de dominio intencionales", Scope = "namespace", Target = "RealEstate.Shared")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Namespaces de Shared/Application usan nombres de dominio intencionales", Scope = "namespace", Target = "RealEstate.Shared.Responses")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Namespaces de Shared/Application usan nombres de dominio intencionales", Scope = "namespace", Target = "RealEstate.Shared.Exceptions")]
[assembly: SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory helpers para ergonomía en controladores", Scope = "type", Target = "RealEstate.Shared.Responses.ApiResponse`1")]


