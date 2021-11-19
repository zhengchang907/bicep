import { Message } from "vscode-jsonrpc";

let interop: any;
const self = global as any;

export async function initializeInterop(): Promise<boolean> {
  return new Promise<boolean>((resolve, reject) => {
    self['LspInitialized'] = (newInterop: any) => {
      interop = newInterop;
      resolve(true);
    }

    const BicepWasm = require("../../../Bicep.Wasm/bin/Bicep.js");
    BicepWasm.boot();
  });
}

export async function sendLspData(message: Message) {
  const messageString = JSON.stringify(message);
  const lspData = `Content-Length: ${messageString.length}\r\n\r\n${messageString}`;

  console.log(messageString);
  return await interop.invokeMethodAsync('SendLspDataAsync', lspData);
}

export function onLspData(callback: (message: Message) => void) {
  self['ReceiveLspData'] = (lspData: string) => {
    const headerSplitIndex = lspData.indexOf('\r\n\r\n');
    const messageString = lspData.substring(headerSplitIndex + 4);
    const message = JSON.parse(messageString);
    console.log(messageString);

    callback(message);
  }
}

export function decompile(jsonContent: string): string {
  const { bicepFile, error } = interop.invokeMethod('Decompile', jsonContent);

  if (error) {
    throw error;
  }

  return bicepFile;
}