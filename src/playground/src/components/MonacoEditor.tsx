import * as monacoEditor from 'monaco-editor';
import React, { useEffect, useRef, useState } from 'react';

interface Props {
  language: string,
  theme: string,
  initialValue?: string,
  options: monacoEditor.editor.IStandaloneEditorConstructionOptions,
  onChange?: (content: string) => void,
  onMount?: (editor: monacoEditor.editor.IStandaloneCodeEditor, monaco: typeof monacoEditor) => void,
}

export const MonacoEditor: React.FC<Props> = props => {
  const [initialValue, setInitialValue] = useState('');
  const [editor, setEditor] = useState<monacoEditor.editor.IStandaloneCodeEditor>();

  const monacoRef = useRef<HTMLDivElement>();
  const { onChange, onMount } = props;

  if (editor && initialValue != props.initialValue) {
    setInitialValue(props.initialValue);
    editor.getModel().setValue(props.initialValue);
  }

  useEffect(() => {
    if (monacoRef) {
      const { language, theme, initialValue, options } = props;
      const editor = monacoEditor.editor.create(
        monacoRef.current,
        {
          value: initialValue,
          language,
          ...options,
          theme,
        });
      
      setEditor(editor);

      if (onChange) {
        editor.onDidChangeModelContent((event) => {
          onChange(editor.getValue());
        });
      }

      if (onMount) {
        onMount(editor, monacoEditor);
      }
    }
  }, []);

  return <div ref={monacoRef} style={{ width: '100%', height: '100%', }} />
};