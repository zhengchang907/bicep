// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode from "vscode";
import path from "path";
import fs from "fs";

import { resolveExamplePath } from "./examples";
import {
  executeShowSourceCommand,
  executeShowVisualizerCommand,
  executeShowVisualizerToSideCommand,
} from "./commands";
import { retryWhile, sleep } from "../utils/time";
import { expectDefined } from "../utils/assert";

const extensionLogPath = path.join(__dirname, "../../../bicep.log");

describe("visualizer", (): void => {
  afterEach(async () => {
    await vscode.commands.executeCommand("workbench.action.closeAllEditors");
  });

  it("should open visualizer webview", async () => {
    console.log(`asdfg20`);
    const examplePath = resolveExamplePath("101", "vm-simple-linux");
    const document = await vscode.workspace.openTextDocument(examplePath);
    const editor = await vscode.window.showTextDocument(document);

    // Give the language server sometime to finish compilation.
    console.log(`asdfg21`);
    await sleep(2000);
    console.log(`asdfg22`);

    await executeShowVisualizerCommand(document.uri);
    console.log(`asdfg23`);
    const viewColumn = await retryWhile(
      async (): Promise<void> => void 0,
      () => {
        console.log(`asdfg23.5`);
        return !visualizerIsReady(document.uri);
      },
      { interval: 100 }
    );
    console.log(`asdfg24`);

    expect(visualizerIsReady(document.uri)).toBeTruthy();
    console.log(`asdfg25`);
    expectDefined(viewColumn);
    console.log(`asdfg26`);
    expect(viewColumn).toBe(editor.viewColumn);
    console.log(`asdfg27`);
  });

  it("should open visualizer webview to side", async () => {
    console.log(`asdfg30`);
    const examplePath = resolveExamplePath("201", "sql");
    const document = await vscode.workspace.openTextDocument(examplePath);
    await vscode.window.showTextDocument(document);
    console.log(`asdfg31`);

    // Give the language server sometime to finish compilation.
    await sleep(2000);
    console.log(`asdfg32`);
    await executeShowVisualizerToSideCommand(document.uri);
    console.log(`asdfg33`);
    let b: boolean;
    const viewColumn = await retryWhile(
      async (): Promise<void> => void 0,
      () => {
        console.log(`asdfg34`);
        b = visualizerIsReady(document.uri);
        return !b;
      },
      { interval: 100 }
    );
    console.log(`asdfg35`);
    let a = visualizerIsReady(document.uri);
    a = !!a;
    console.log(a);
    expect(visualizerIsReady(document.uri)).toBeTruthy();
    console.log(`asdfg36`);
    expectDefined(viewColumn);
    expect(viewColumn).toBe(vscode.ViewColumn.Beside);
  });

  it("should open source", async () => {
    console.log(`asdfg40`);
    expect(vscode.window.activeTextEditor).toBeUndefined();

    const examplePath = resolveExamplePath("201", "sql");
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    await executeShowVisualizerCommand(textDocument.uri);
    const sourceEditor = await executeShowSourceCommand();

    expectDefined(sourceEditor);
    expect(sourceEditor).toBe(vscode.window.activeTextEditor);
  });

  function visualizerIsReady(documentUri: vscode.Uri): boolean {
    if (!fs.existsSync(extensionLogPath)) {
      return false;
    }

    const readyMessage = `Visualizer for ${documentUri.fsPath} is ready.`;
    return fs.readFileSync(extensionLogPath).indexOf(readyMessage) >= 0;
  }
});
