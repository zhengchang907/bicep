// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";

export class LintCommand implements Command {
  public readonly id = "bicep.lint";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to lint"
    );

    try {
      const lintOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "lint",
          arguments: [documentUri.fsPath],
        }
      );
      this.outputChannelManager.appendToOutputChannel(lintOutput);
    } catch (err) {
      this.client.error("Linter failed", parseError(err).message, true);
    }
  }
}
