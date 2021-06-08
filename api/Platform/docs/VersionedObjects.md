# Versioned Objects
Objects that will go through changes and allow for old versions to remain in place
will need to track versioning.  At present, this will consist of a Parent entity 
for the grouping of the versioned object, and a child object that will be the 
version container.  The Parent object will define the current version to use, 
unless otherwise specified by the object reference.

```
Object {
    Id: Guid
    CurrentVersion: Int
    Name: String
    Description: String
    ...
}

ObjectVersion {
    Id: Guid
    ObjectId: Guid
    Version: Int
    ...
}

VersionReference {
    Id: Guid
    Version?: Int
    VersionId: Guid
    Name: String
}
```