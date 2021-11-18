import * as monacoEditor from 'monaco-editor';
import React, { useState } from 'react';
import { MonacoEditor } from './MonacoEditor';

interface JsonEditorProps {
  content: string;
}

export const JsonEditor : React.FC<JsonEditorProps> = props=> {
  const [editor, setEditor] = useState<monacoEditor.editor.IStandaloneCodeEditor>();

  const options: monacoEditor.editor.IStandaloneEditorConstructionOptions = {
    scrollBeyondLastLine: false,
    automaticLayout: true,
    minimap: {
      enabled: false,
    },
    readOnly: true,
  };
  
  const handleOnMount = (editor: monacoEditor.editor.IStandaloneCodeEditor) => {
    setEditor(editor);
  }

  const handleOnChange = (content: string) => {
    if (editor) {
      // clear the selection after rendering completes
      editor.setSelection({startColumn: 1, startLineNumber: 1, endColumn: 1, endLineNumber: 1});
    }
  }

  return <MonacoEditor
    language="json"
    theme="vs-dark"
    initialValue={props.content}
    options={options}
    onMount={handleOnMount}
    onChange={handleOnChange}
  />
};