Orleans.EventSourcing
=====================

Event sourcing support for MSR Orleans (http://orleans.codeplex.com) using EventStore (http://geteventstore.com)


Prerequisites
-------------

1. Orleans SDK. You can obtain it from here: http://aka.ms/orleans.
2. EventStore 3.0.0. You can obtain a prerelease version from  here: http://geteventstore.com/downloads.


Usage
-----

1. Compile the solution first. This will copy the assemblies to the local silo folder (post-build event).

2. Start EventStore. 

3. Configure EventStoreProvider in `DevTestServerConfiguration.xml`. Connection string is in `<hostname>:<port>` format. 

  ```
<OrleansConfiguration xmlns="urn:orleans">
  <Globals>
    <StorageProviders>
      <Provider Type="Orleans.EventSourcing.EventStoreStorage.EventStoreProvider" Name="EventStore" ConnectionString="localhost:1113" Username="admin" Password="changeit"  />
```

  *Username and Password can be omitted if you're using default values.*

4. Run the Test.Client project. This will start the local silo and execute sample code from Program.Main().


How does it work?
-----

The most important thing to understand is that it is the *state* that is event sourced, not the grain.

The second most important thing is that event sourcing is hidden in the Implementation project and not present in the Interfaces project - *the fact that a certain grain is event sourced is encapsulated in this grain and doesn't impact other grains*.


1. Define your state

2. Start EventStore. 

3. Configure EventStoreProvider in `DevTestServerConfiguration.xml`. Connection string is in `<hostname>:<port>` format. 

1. 

Derive from `IAggregateState` instead of `IState`.

```
public interface IPersonState : IAggregateState
{
    string FirstName { get; set; }
    string LastName { get; set; }
    GenderType Gender { get; set; }
    bool IsMarried { get; set; }
}
```

2. Define your events

Events are plain classes. By convention, add a `public void Apply(IYourAggregateState state)` method that will mutate the state by applying the current event.

```
public class PersonLastNameChanged
{
    public string LastName { get; set; }

    public void Apply(IPersonState state)
    {
        state.LastName = this.LastName;
    }
}
```

3. Raise events from the grain

Call `RaiseEvent` method passing your event in the grain. `RaiseEvent` will mutate the grain state so you don't have to do it yourself. By default the event will be persisted in event store. If you know that you will be raising multiple events and want to persist them in a single commit, you can pass `store: false` argument. In that case the state will be mutated but the event won't be persisted until `RaiseEvent(@event, store: true)` or `this.State.WriteStateAsync()` is called.

```
Task IPerson.Register(PersonalAttributes props)
{
    return this.RaiseEvent(new PersonRegistered
    {
        FirstName = props.FirstName,
        LastName = props.LastName,
        Gender = props.Gender
    });
}
```
