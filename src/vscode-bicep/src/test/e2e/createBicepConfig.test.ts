// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg content modified

import { Uri, window } from "vscode";
import path from "path";
import fse from "fs-extra";
import os from "os";
import {
  executeAcceptSelectedSuggestion,
  executeCloseAllEditors,
  executeCreateConfigFileCommand,
  executeSelectNextSuggestion,
} from "./commands";
import {} from "fs";
import { normalizeLineEndings } from "../utils/normalizeLineEndings";
import { testScope } from "../utils/testScope";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should create valid config file and open it", async () => {
    const tempFolder = createUniqueTempFolder("createBicepConfigTest-");
    const fakeBicepPath = path.join(tempFolder, "main.bicep");
    try {
      let newConfigPath = await executeCreateConfigFileCommand(
        Uri.file(fakeBicepPath)
      );

      if (!newConfigPath) {
        throw new Error(
          `Language server returned ${String(
            newConfigPath
          )} for bicep.createConfigFile`
        );
      }
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      newConfigPath = newConfigPath!;

      expect(path.basename(newConfigPath)).toBe("bicepconfig.json");
      if (!fileExists(newConfigPath)) {
        throw new Error(
          `Expected file ${newConfigPath} to exist but it doesn't`
        );
      }

      expect(fileContains(newConfigPath, "rules")).toBeTruthy();
      expect(fileIsValidJson(newConfigPath)).toBeTruthy();

      // Since the test instance of vscode does not have any workspace folders, the new file should be opened
      //   in the same folder as the bicep file
      expect(path.dirname(newConfigPath).toLowerCase()).toBe(
        path.dirname(fakeBicepPath).toLowerCase()
      );

      // Make sure the new config file has been opened in an editor
      const editor = window.visibleTextEditors.find(
        (ed) =>
          ed.document.uri.fsPath.toLowerCase() === newConfigPath?.toLowerCase()
      );
      if (!editor) {
        throw new Error("New config file should be opened in a visible editor");
      }

      //asdfg synchronize with snippet test
      const expectedAfterInsertion = `{
        // See https://aka.ms/bicep/config for more information on Bicep configuration options
        // Press CTRL+SPACE/CMD+SPACE at any location to see Intellisense suggestions
        "analyzers": {
            "core": {
                "rules": {
                    "no-unused-params": {
                        "level": "warning"
                    }
                }
            }
        }
    }
    `;

      const textAfterInsertion = editor.document.getText();
      expect(normalizeLineEndings(textAfterInsertion)).toBe(
        normalizeLineEndings(expectedAfterInsertion)
      );

      await testScope("Verify inserted snippet", () => {
        const actualText = editor.document.getText();
        expect(normalizeLineEndings(actualText)).toBe(
          normalizeLineEndings(expectedAfterInsertion)
        );
      });

      // Verify that vscode is in an "insertion" state with the dropdown for the first rule open to show the available diagnostic levels (the current one should be "warning").
      // Verify this by moving down to the next suggestion ("off") and selecting it`
      const expectedAfterSelectingOffInsteadOfWarning =
        expectedAfterInsertion.replace(/warning/, "off");
      await executeSelectNextSuggestion();
      await executeAcceptSelectedSuggestion();
      const textAfterSelectingOffInsteadOfWarningtext =
        editor.document.getText();
      expect(
        normalizeLineEndings(textAfterSelectingOffInsteadOfWarningtext)
      ).toBe(normalizeLineEndings(expectedAfterSelectingOffInsteadOfWarning));
    } finally {
      fse.rmdirSync(tempFolder, {
        recursive: true,
        maxRetries: 5,
        retryDelay: 1000,
      });
    }
  });

  function fileExists(path: string): boolean {
    return fse.existsSync(path);
  }

  function fileContains(path: string, pattern: RegExp | string): boolean {
    const contents: string = fse.readFileSync(path).toString();
    return !!contents.match(pattern);
  }

  function fileIsValidJson(path: string): boolean {
    fse.readJsonSync(path, { throws: true });
    return true;
  }
});

function createUniqueTempFolder(filenamePrefix: string): string {
  const tempFolder = os.tmpdir();
  if (!fse.existsSync(tempFolder)) {
    fse.mkdirSync(tempFolder, { recursive: true });
  }

  return fse.mkdtempSync(path.join(tempFolder, filenamePrefix));
}
