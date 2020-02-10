using System;

using Akka.DI.Core;
using Akka.DI.DryIoc;

using DryIoc;

namespace Akka.Actor
{
    public static class ActorSystemExtensions
    {
        public static ActorSystem UseDryIoC(this ActorSystem system, IContainer container)
            => system.UseDryIoC(container, out IDependencyResolver dependencyResolver);

        public static ActorSystem UseDryIoC(this ActorSystem system, IContainer container, out IDependencyResolver dependencyResolver)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            dependencyResolver = new DryIocDependencyResolver(container, system);

            return system;
        }
    }
}