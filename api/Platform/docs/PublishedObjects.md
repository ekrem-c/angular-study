# Publishing / Active States
For objects that need to track state for use, we are following a basic publishing 
approach.  We use "Draft" as a starting point, "Published" as an active state, and 
"Archived"" for deprecated or inactive.  If something was published (and subsequently
used), then it can not be hard deleted.  If it was only in draft, then it may be 
deleted.  If there is a need for additional states, then the team should discuss 
and consider documenting which states are used elsewhere for consistent patterns.  
IE: a "Testing" state will allow something to be used in a private fashion before 
publishing.  Object state flows should be well documented for every project.

In many cases, a published or archived item should not be modifiable.  Only items 
in a draft state should be modifiable.  In the case of a published item having 
child items that are also tracking state (versioned objects for example), then, 
publishing the parent will publish the 'active' child (current version in versioned 
objects) if it is not already published.  Archiving a parent will archive all 
children.  Archiving the last published child on a published parent will archive 
the parent.

_Example State Flow Map_
Draft => (delete)
Draft => Published
Published => Archived
Archived => Published
