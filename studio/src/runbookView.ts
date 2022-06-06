import * as vscode from 'vscode';
import * as fs from 'fs';
import * as path from 'path';
import * as YAML from 'yaml';

export class RunbookViewProvider implements vscode.TreeDataProvider<RunbookAction> {

  constructor(private workspaceRoot: string, private runbookPath: string) { }

  getTreeItem(element: RunbookAction): vscode.TreeItem {
    return element;
  }

  getChildren(element?: RunbookAction): Thenable<RunbookAction[]> {
    if (!this.runbookPath) {
      vscode.window.showInformationMessage('No actions in a missing runbook');
      return Promise.resolve([]);
    }

    if (element) {
      return Promise.resolve(
        this.getActions(
          path.join(this.workspaceRoot, this.runbookPath)
        )
      );
    } else {
      const fullPath = path.join(this.workspaceRoot, this.runbookPath);
      if (this.pathExists(fullPath)) {
        return Promise.resolve(this.getActions(fullPath));
      } else {
        vscode.window.showInformationMessage('Workspace has no package.json');
        return Promise.resolve([]);
      }
    }
  }

  /**
   * Given the path to package.json, read all its dependencies and devDependencies.
   */
  private getActions(runbookPath: string): RunbookAction[] {
    if (this.pathExists(runbookPath)) {
      const yamlModel = YAML.parse(fs.readFileSync(runbookPath, 'utf-8'));

      const toAction = (moduleName: string, version: string): RunbookAction => {
        if (this.pathExists(path.join(this.workspaceRoot, 'node_modules', moduleName))) {
          return new RunbookAction(
            moduleName,
            version,
            vscode.TreeItemCollapsibleState.Collapsed
          );
        } else {
          return new RunbookAction(moduleName, version, vscode.TreeItemCollapsibleState.None);
        }
      };

      const deps = yamlModel.dependencies ? Object.keys(yamlModel.dependencies).map(dep =>
        toAction(dep, yamlModel.dependencies[dep])
      ) : [];
      const devDeps = yamlModel.devDependencies ? Object.keys(yamlModel.devDependencies).map(dep =>
        toAction(dep, yamlModel.devDependencies[dep])
      ) : [];

      return deps.concat(devDeps);
    } else {
      return [];
    }
  }

  private pathExists(p: string): boolean {
    try {
      fs.accessSync(p);
    } catch (err) {
      return false;
    }
    return true;
  }
}

class RunbookAction extends vscode.TreeItem {
  constructor(
    public readonly label: string,
    private version: string,
    public readonly collapsibleState: vscode.TreeItemCollapsibleState
  ) {
    super(label, collapsibleState);
    this.tooltip = `${this.label}-${this.version}`;
    this.description = this.version;
  }

  iconPath = {
    light: path.join(__filename, '..', '..', 'resources', 'light', 'dependency.svg'),
    dark: path.join(__filename, '..', '..', 'resources', 'dark', 'dependency.svg')
  };
}
