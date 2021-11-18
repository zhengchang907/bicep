import * as monacoEditor from 'monaco-editor';
import { BaseLanguageClient } from '@codingame/monaco-languageclient';
import React, { useState } from 'react';
import { MonacoEditor } from './MonacoEditor';

interface Props {
  client: BaseLanguageClient,
  initialValue: string,
  onBicepChange: (bicepContent: string) => void,
  onJsonChange: (jsonContent: string) => void,
}

function configureEditorForBicep(editor: monacoEditor.editor.IStandaloneCodeEditor) {
  // @ts-expect-error
  editor._themeService._theme.getTokenStyleMetadata = (type, modifiers) => {
    // see 'monaco-editor/esm/vs/editor/standalone/common/themes.js' to understand these indices
    switch (type) {
      case 'keyword':
        return { foreground: 12 };
      case 'comment':
        return { foreground: 7 };
      case 'parameter':
        return { foreground: 2 };
      case 'property':
        return { foreground: 3 };
      case 'type':
        return { foreground: 8 };
      case 'member':
        return { foreground: 6 };
      case 'string':
        return { foreground: 5 };
      case 'variable':
        return { foreground: 4 };
      case 'operator':
        return { foreground: 9 };
      case 'function':
        return { foreground: 13 };
      case 'number':
        return { foreground: 15 };
      case 'class':
      case 'enummember':
      case 'event':
      case 'modifier':
      case 'label':
      case 'typeParameter':
      case 'macro':
      case 'interface':
      case 'enum':
      case 'regexp':
      case 'struct':
      case 'namespace':
        return { foreground: 0 };
    }
  };
}

const modelUri = 'inmemory://main.bicep-test';
export const BicepEditor: React.FC<Props> = (props) => {
  const { client, initialValue } = props;
  const [model] = useState(() => monacoEditor.editor.createModel('', 'bicep-test', monacoEditor.Uri.parse(modelUri)));

  const editorOptions: monacoEditor.editor.IStandaloneEditorConstructionOptions = {
    scrollBeyondLastLine: false,
    automaticLayout: true,
    model: model,
    minimap: {
      enabled: false,
    },
    insertSpaces: true,
    tabSize: 2,
    suggestSelection: 'first',
    suggest: {
      snippetsPreventQuickSuggestions: false,
      showWords: false,
    }
  };

  const handleOnChange = (text: string) => {
    props.onBicepChange(text);
/*
    client.sendRequest('workspace/executeCommand', {
      command: 'build',
      arguments: [
        modelUri,
      ]
    });
*/
  };

  return <MonacoEditor
    language="bicep"
    theme="vs-dark"
    options={editorOptions}
    initialValue={initialValue}
    onChange={handleOnChange}
    onMount={configureEditorForBicep}
  />
};