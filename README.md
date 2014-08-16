Orleans.EventSourcing
=====================

Event sourcing support for MSR Orleans (http://orleans.codeplex.com).


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
