import * as vscode from 'vscode';
import { RunbookViewProvider } from './runbookView';

export function activate(context: vscode.ExtensionContext) {
	console.log("RunbookStudio activated");

	const rootPath = (vscode.workspace.workspaceFolders && (vscode.workspace.workspaceFolders.length > 0))
		? vscode.workspace.workspaceFolders[0].uri.fsPath : "";
	
	const viewProvider = new RunbookViewProvider(rootPath, "");
	vscode.window.registerTreeDataProvider('runbookView', viewProvider);
	registerRunbookView(viewProvider, context);

	let disposable = vscode.commands.registerCommand('runbook-studio.helloWorld', () => {
		vscode.window.showInformationMessage('Hello World from runbook-studio!');
	});

	context.subscriptions.push(disposable);
}

export function deactivate() {
	console.log("RunbookStudio deactivated");
}

function registerRunbookView(provider: RunbookViewProvider, context: vscode.ExtensionContext) {
	const view = vscode.window.createTreeView('runbookView', { treeDataProvider: provider, showCollapseAll: true });
	context.subscriptions.push(view);
}
