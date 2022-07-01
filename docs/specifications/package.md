---
title: Package
parent: Specifications
---

# Package

A package publishes runbooks from a directory. Each package directory
must contain a valid `package.yaml` file.

## Fields

* __`version`__

	Version of the package format used. __This field is required.__
	
* __`title`__

	Text to display when referencing this package. _This field is
    required.__
	
* __`summary`__

	Brief description of the package contents. _This field is
    optional._
	
* __`runbooks`__

	Sequence of runbooks to publish for this package. Each runbook
    must contain the explicit version number or a version
    pattern. _This field is required._
	
* __`ignored`__

	Sequence of runbook that are removed from the package. When a
    runbook is ignored it cannot be referenced or used from another
    runbook. _This field is optional._

## Example

```yaml
version 1

title: System Package
summary: |
  Runbooks used to interact with the operating system.

runbooks:
  - Execute@1
  - Execute@3
  - Print@1
  - Print@2
  - Print@3
  - Prompt@+

ignored:
  - Execute@0
  - Execute@2
  - Print@0
  - Prompt@0
```



