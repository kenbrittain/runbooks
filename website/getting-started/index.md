---
title: Getting Started
nav_order: 2
---

# Getting Started

The runbook compiler is a command line interface for managing
collections of [YAML][yaml] defined runbooks. It is used for
publishing HTML and Markdown views of those runbook for off line and
on-line viewing.

## What is a Manifest?

ñThe runbook-compiler builds and maintains a file called the
manifest. This file is a YAML file and located in the root of your
source tree. It is called `manifest.yml` by default.

This file contains references to the runbook source files it uses. As
files are generated they are added to the manifest in different
sections. The manifest file using these top level categories.

```yaml
sources: []
views: []
scripts: []
```

The sources section contains a list of the the YAML runbooks included
when creating the manifest. The `export-runbook` command uses this
section as the list of files to generate output files.

```yaml
sources:
  - name: 002-runbook
    path: 002-runbook.yml
```

## Generate a Manifest

Use the `create-manifest` command to create a manifest file. This
command allows for including and including certain file patterns.

## Generate Markdown

Use the `export-runbook` command to generate [Markdown][md] files.

## Generate HTML

Use the `export-runbook` command to generate [HTML][html] files.



[md]:https://en.wikipedia.org/wiki/Markdown	"Markdown"
[html]:https://en.wikipedia.org/wiki/HTML
[yaml]:https://yaml.org
