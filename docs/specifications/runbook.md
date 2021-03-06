---
title: Runbook
parent: Specifications
---

# Runbook

A runbook file is a YAML document that describes the contents of an
executable runbook.

## Fields

* __`version`__

	Version of the runbook format used. _This field is required._

* __`title`__

	Text to display when referencing this runbook. _This field is required._

* __`summary`__

	Brief description of this runbook. _This field is optional._

* __`author`__

	Name of the author of this runbook. _This field is required._

* __`contact`__

	How to contact the author of this runbook. _This field is required._
	
* __`variables`__

	Sequence of `variable` objects defining values that are required
    by the runbook. _This field is optional._

* __`scripts`__

	Sequence of `script` objects. When this field is present to
    runbook is capable of being compiled into a script. _This field is
    optional._
	
* __`actions`__

	Sequence of `action` objects. _This field is optional._

## Example

```yaml
version: 1

title: Print Text
summary: |
  Call to display text on the console.

author: General Software Productions, LLC.
contact: ken.brittain@runbookcompiler.org

variables:
  - name: Text
    type: string
    summary: The text to display.
  
scripts:
  batch: |
    @ECHO <Text>
  bash: |
    echo <Text>
```
