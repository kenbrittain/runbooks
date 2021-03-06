---
title: Library
parent: Specifications
---

# Library

A library is a collection of packages that are published under a
single version. Each library is defined by a root directory. The
packages are published as child directories containing runbooks.

This `library.yaml` file format is described below.

## Fields

* __`version`__

	Version of the library format used. _This field is required._

* __`title`__

	Text to display when referencing this runbook. _This field is
    required._

* __`summary`__

	Brief description of the fuctionality provided by the runbook.
	_This field is optional._

* __`packages`__

	Sequence of package directories. Each directory must be a child
    directory that contains a valid `package.yaml` file. _This field
    is required._

* __`deprecated`__

	Sequence of package directories that are no longer supported. When
    runbooks reference a deprecated package a warning will be
    displayed by the compiler. _This field is optional._

## Examples

```yaml
version: 1

title: Standard Runbook Library
summary: |
  Runbooks common for all Runbook Compiler installations.

packages:
  - Examples
  - Runbook
  - System

deprecated:
  - Linux
  - Windows
```
