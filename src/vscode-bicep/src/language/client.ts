// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import * as lsp from "vscode-languageclient/node";
import { getLogger } from "../utils/logger";
import {
  callWithTelemetryAndErrorHandlingSync,
  IActionContext,
  parseError,
} from "vscode-azureextensionui";
import { ErrorAction, Message, CloseAction, AbstractMessageReader, MessageReader, MessageWriter, AbstractMessageWriter, DataCallback, Disposable } from "vscode-languageclient/node";
import { initializeInterop, onLspData, sendLspData } from "./lspInterop";

export async function launchLanguageServiceWithProgressReport(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Promise<lsp.LanguageClient> {
  return await vscode.window.withProgress(
    {
      title: "Launching Bicep language service...",
      location: vscode.ProgressLocation.Notification,
    },
    async () => await launchLanguageService(context, outputChannel)
  );
}

async function launchLanguageService(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Promise<lsp.LanguageClient> {
  getLogger().info("Launching Bicep language service...");

  await initializeInterop();

  const serverOptions: lsp.ServerOptions = async () => {
    const [reader, writer] = createStream();

    return {
      reader,
      writer
    };
  }

  const clientOptions: lsp.LanguageClientOptions = {
    documentSelector: [{ language: "bicep" }],
    initializationOptions: {
      // this tells the server that this client can handle additional DocumentUri schemes
      enableRegistryContent: true,
    },
    progressOnInitialization: true,
    outputChannel,
    middleware: {
      provideDocumentFormattingEdits: (document, options, token, next) =>
        next(
          document,
          {
            ...options,
            insertFinalNewline:
              vscode.workspace
                .getConfiguration("files")
                .get("insertFinalNewline") ?? false,
          },
          token
        ),
    },
    synchronize: {
      // These file watcher globs should be kept in-sync with those defined in BicepDidChangeWatchedFilesHander.cs
      fileEvents: [
        vscode.workspace.createFileSystemWatcher("**/"), // folder changes
        vscode.workspace.createFileSystemWatcher("**/*.bicep"), // .bicep file changes
        vscode.workspace.createFileSystemWatcher("**/*.{json,jsonc,arm}"), // ARM template file changes
      ],
    },
  };

  const client = new lsp.LanguageClient(
    "bicep",
    "Bicep",
    serverOptions,
    clientOptions
  );

  client.registerProposedFeatures();

  configureTelemetry(client);

  // To enable language server tracing, you MUST have a package setting named 'bicep.trace.server'; I was unable to find a way to enable it through code.
  // See https://github.com/microsoft/vscode-languageserver-node/blob/77c3a10a051ac619e4e3ef62a3865717702b64a3/client/src/common/client.ts#L3268

  context.subscriptions.push(client.start());

  getLogger().info("Bicep language service started.");

  await client.onReady();

  getLogger().info("Bicep language service ready.");

  return client;
}

function configureTelemetry(client: lsp.LanguageClient) {
  const startTime = Date.now();
  const defaultErrorHandler = client.createDefaultErrorHandler();

  client.onTelemetry(
    (telemetryData: {
      eventName: string;
      properties: { [key: string]: string | undefined };
    }) => {
      callWithTelemetryAndErrorHandlingSync(
        telemetryData.eventName,
        (telemetryActionContext) => {
          telemetryActionContext.errorHandling.suppressDisplay = true;
          telemetryActionContext.telemetry.properties =
            telemetryData.properties;
        }
      );
    }
  );

  client.clientOptions.errorHandler = {
    error(
      error: Error,
      message: Message | undefined,
      count: number | undefined
    ): ErrorAction {
      callWithTelemetryAndErrorHandlingSync(
        "bicep.lsp-error",
        (context: IActionContext) => {
          context.telemetry.properties.jsonrpcMessage = message
            ? message.jsonrpc
            : "";
          context.telemetry.measurements.secondsSinceStart =
            (Date.now() - startTime) / 1000;

          throw new Error(`Error: ${parseError(error).message}`);
        }
      );
      return defaultErrorHandler.error(error, message, count);
    },
    closed(): CloseAction {
      callWithTelemetryAndErrorHandlingSync(
        "bicep.lsp-error",
        (context: IActionContext) => {
          context.telemetry.measurements.secondsSinceStart =
            (Date.now() - startTime) / 1000;

          throw new Error(`Connection closed`);
        }
      );
      return defaultErrorHandler.closed();
    },
  };
}


class LspMessageReader extends AbstractMessageReader implements MessageReader {
  listen(callback: DataCallback): Disposable {
    onLspData(data => callback(data));
    return Disposable.create(() => {});
  }
}

class LspMessageWriter extends AbstractMessageWriter implements MessageWriter {
  async write(msg: Message): Promise<void> {
    sendLspData(msg);
  }
  end(): void { }
}

function createStream() {
  const reader = new LspMessageReader();
  const writer = new LspMessageWriter();

  return [reader, writer] as const;
}