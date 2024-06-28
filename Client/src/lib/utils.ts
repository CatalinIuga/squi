import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";
import { Result } from "./types";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function unrwapResult<T>(result: Result<T>): T {
  if (result.ok) {
    return result.data;
  }
  throw new Error(result.message);
}

export function errorToResult(error: unknown): Result<any> {
  if (error instanceof Error) {
    return { ok: false, message: error.message };
  }
  return { ok: false, message: "Unknown error" };
}

export function match<T, U extends unknown>(
  result: Result<T>,
  ok: (data: T) => U,
  err: (message: string) => U
): U {
  return result.ok ? ok(result.data) : err(result.message);
}

export function sleep(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
