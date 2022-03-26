// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function retryWhile<T>(
  func: () => Promise<T>,
  predicate: (result: T) => boolean,
  retryOptions?: Readonly<{
    interval?: number;
    timeLimitMs?: number;
  }>
): Promise<T> {
  let result = await func();

  const interval = retryOptions?.interval ?? 2000;
  let count = (retryOptions?.timeLimitMs ?? 10000) / interval;

  while (predicate(result)) {
    if (count-- <= 0) {
      throw new Error("Timeout");
    }
    result = await func();
    await sleep(interval);
  }

  return result;
}
