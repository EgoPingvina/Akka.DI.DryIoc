using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;

using Akka.Actor;
using Akka.DI.Core;

using DryIoc;

namespace Akka.DI.DryIoc
{
    public sealed class DryIocDependencyResolver : IDependencyResolver, INoSerializationVerificationNeeded
    {
        private readonly IContainer container;
        private readonly ConcurrentDictionary<string, Type> typeCache;
        private readonly ActorSystem system;
        private readonly ConditionalWeakTable<ActorBase, IResolverContext> references;

        public DryIocDependencyResolver(IContainer container, ActorSystem system)
        {
            this.container  = container
                ?? throw new ArgumentNullException(nameof(container));

            this.system     = system
                ?? throw new ArgumentNullException(nameof(system));

            this.typeCache  = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            this.references = new ConditionalWeakTable<ActorBase, IResolverContext>();

            this.system.AddDependencyResolver(this);
        }

        public Func<ActorBase> CreateActorFactory(Type actorType)
            => () =>
                {
                    var context = this.container.OpenScope();
                    var key     = (ActorBase)context.Resolve(actorType);
                    this.references.Add(key, context);
                    return key;
                };

        public Type GetType(string actorName)
        {
            var type = actorName.GetTypeValue();
            if (type is null)
                type = this.container.GetServiceRegistrations()
                                     .Where(registration =>
                                        registration.ImplementationType.Name.Equals(actorName, StringComparison.OrdinalIgnoreCase))
                                     .Select(registration =>
                                        registration.ImplementationType)
                                     .FirstOrDefault();

            this.typeCache.TryAdd(actorName, type);

            return this.typeCache[actorName];
        }

        public Props Create<TActor>()
            where TActor : ActorBase
            => this.Create(typeof(TActor));

        public Props Create(Type actorType)
            => this.system.GetExtension<DIExt>().Props(actorType);

        public void Release(ActorBase actor)
        {
            if (!this.references.TryGetValue(actor, out IResolverContext lifetimeScope))
                return;

            lifetimeScope.Dispose();
            this.references.Remove(actor);
        }
    }
}