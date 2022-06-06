---
title: Overview
nav_order: 1
---

# Overview

The runbook compiler is a command line interface for managing collections of [YAML][yaml] defined [Runbooks][wiki]. It is used for publishing Html and Markdown views.

## Commands

The following commands are available when the `runbook` command is installed.

### Manifest Commands

* [create-manifest](commands/create-manifest.md)
* [show-manifest](commands/show-manifest.md)

### Runbook Commands

* [export-runbook](commands/export-runbook.md)

## Usage

The `runbook` command is the program that executes the commands. Commands are specified on the command line using the name listed.

```shell
runbook <COMMAND> [OPTIONS]
```

### Arguments

The arguments passed on the command line are used by the command invoked. They are noted in the documentation as either required or optional using the following syntax:

* Required arguments are identified by enclosing the name of the argument in angle brackets: \<ARGUMENT\>

* Optional arguments are identified by enclosing the name of the argument in square brackets: [OPTIONAL]

### Options

The options passed on the command line are also used by the invoked command. Arguments are passed as a fixed position to the command. Options use a prefix character for identifying the option name. The options prefixes are:

* __`-`__ single dash for short option names. These options are identified by a single character.
* __`--`__ double dash for long option names. These options are identified by a name or names.



[yaml]: https://yaml.org
[wiki]:https://en.wikipedia.org/wiki/Runbook

