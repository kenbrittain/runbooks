---
title: Manifest
parent: Specifications
---

# Manifest

The manifest file is used to store the state of the runbooks, their
exported views, and compiled scrippts between runs of commands.

This file is JSON and is serialized/deserialized by commands. It is
not intended to be edited manually..

## Fields

* __`version`__

	Version of the manifest format used. __This field is required.__

## Example

```json
{}
```
