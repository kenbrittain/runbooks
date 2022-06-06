---
parent: Commands
---
# show-manifest

Display the manifest contents on the console.

## Syntax

```shell
runbook show-manifest [<MANIFEST>] [--verbose]
runbook show-manifest -h|--help
```

## Description

The `show-manifest` command displays the contents of the manifest file for review. The manifest file tracks all of the source files and generated view files. This file exists to allow additional commands to operate on previously the generated files. For example, publishing generated view pages.

## Arguments

* __`MANIFEST`__

  Specifies the name of the manifest file. If the manifest is not specified then the current directory is searched for a file named `manifest.yml` and uses that file.

## Options

* __`-?|--help`__

  Displays a description of how to use the command.

* __`--verbose`__

  Displays more information. If not specified, the default is false.
