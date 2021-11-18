import { BaseLanguageClient, CloseAction, createConnection, Disposable, ErrorAction, MonacoLanguageClient, MonacoServices } from '@codingame/monaco-languageclient';
import { AbstractMessageReader, AbstractMessageWriter, createMessageConnection, DataCallback, Message, MessageReader, MessageWriter } from 'vscode-jsonrpc';
import { onLspData, sendLspData } from './lspInterop';
import * as monacoEditor from 'monaco-editor'

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

export async function createLanguageClient(): Promise<BaseLanguageClient> {
  monacoEditor.languages.register({
    id: 'bicep',
    extensions: ['.bicep'],
    aliases: ['bicep'],
  });

  MonacoServices.install(monacoEditor);

  const [reader, writer] = createStream();
  const messageConnection = createMessageConnection(reader, writer);

  const client = new MonacoLanguageClient({
    name: "Bicep Monaco Client",
    clientOptions: {
      documentSelector: [{ language: 'bicep' }],
      errorHandler: {
        error: () => ErrorAction.Continue,
        closed: () => CloseAction.DoNotRestart
      }
    },
    connectionProvider: {
      get: (errorHandler, closeHandler) => {
        return Promise.resolve(createConnection(messageConnection, errorHandler, closeHandler));
      }
    }
  });

  client.start();
  await client.onReady();

  return client;
}