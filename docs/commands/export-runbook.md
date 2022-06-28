---
parent: Commands
---
# export-runbook

Write runbook contents in different formats.

## Syntax

```shell
runbook export-runbook <MANIFEST> [--include] [--exclude] [-t|--type] [-o|--output] [-v|--verbose] 
runbook export-runbook -?|--help
```

## Description

The `export-runbook` command writes the contents of a collection of runbooks into an external format. Only source files contained in the manifest are available for exporting.

## Arguments

* __`MANIFEST`__

  Specifies the name of the manifest file. If the manifest is not specified then the current directory is searched for a file named `manifest.yml` and uses that file.

## Options

* __`--exclude`__

  Pattern used to excludes source files from the export. This option can be included multiple times with different patterns.

* __`-?|--help`__

  Displays a description of how to use the command.

* __`--include`__

  Pattern used to include source files in the export command. This option can be included multiple times with different patterns.

* __`--index`__

  Write index file for the exported runbook.

* __`-o|--output`__

  Directory to write exported files. 

* __`-t|--type`__

  Specifies the type of view to export. Supported formats include: markdown and html.

* __`--verbose`__

  Displays more information. If not specified, the default is false.

