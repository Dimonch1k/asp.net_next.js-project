using Microsoft.Extensions.DependencyInjection;
using System;

public class LazyDependency<T> : Lazy<T>
{
  public LazyDependency( IServiceProvider serviceProvider )
      : base( serviceProvider.GetRequiredService<T> )
  {
  }
}
