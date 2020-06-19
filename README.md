# Akka.DI.DryIoc

**Actor Producer Extension** backed by the [DryIoc](https://github.com/dadhi/DryIoc) Dependency Injection Container for the [Akka.NET](https://github.com/akkadotnet/akka.net) framework.

## What is it?

**Akka.DI.DryIoc** is an **ActorSystem extension** for the Akka.NET framework that provides an alternative to the basic capabilities of [Props](https://getakka.net/api/Akka.Actor.Props.html) when you have Actors with multiple dependencies.  

If Autofac is your IoC container of choice and your actors have dependencies that make using the factory method provided by Props prohibitive  and code maintenance is an important concern then this is the extension for you.
