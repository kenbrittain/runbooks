---
parent: Commands
---
# create-manifest

Creates a new manifest file.

## Syntax

```shell
runbook create-manifest <MANIFEST> [--exclude] [-f|--force] [--include] [--verbose]
runbook create-manifest -?|--help
```

## Description

This command writes the YAML manifest to the specific location. If a file exists then default is to produce an error. You can override this behavior by using the `--force` flag.

## Arguments

* __`MANIFEST`__

  Specifies the name of the manifest file. If the manifest is not specified then the current directory is searched for a file named `manifest.yml` and uses that file.

## Options

* __`--exclude`__
  
  File pattern to exclude. This option can be included multiple times with different patterns.

* __`-f|--force`__

  Overwrites an existing manifest if it exists.

* __`-?|--help`__

  Displays a description of how to use the command.

* __`--include`__
  
  File pattern to include. If not specified, the command used `**/*.yml` to locate all YAML files.

* __`--verbose`__

  Displays more information. If not specified, the default is false.

## Examples

```shell

```
