import { useState } from "react";
import { useColorScheme } from "react-native";
import { useColorSchemeContext } from "./ColorSchemeContext";

/**
 * Custom hook to manage and toggle color scheme (system, light, dark).
 * Returns [effectiveColorScheme, mode, setMode]
 * - effectiveColorScheme: 'light' | 'dark' (for UI)
 * - mode: 'light' | 'dark' | 'system' (for settings UI)
 * - setMode: (mode: 'system' | 'light' | 'dark') => void
 */
export function useColorSchemeToggle(): ["light" | "dark", "light" | "dark" | "system", (mode: "system" | "light" | "dark") => void] {
  // ALWAYS call these hooks first - they must be called in the same order every render
  const systemScheme = useColorScheme();
  const [localMode, setLocalMode] = useState<"light" | "dark" | "system">("system");
  
  // Always call the context hook - it will either return the context or throw
  // This ensures hooks are called in the same order every time
  let hasContext = false;
  let ctx: ReturnType<typeof useColorSchemeContext> | null = null;
  
  try {
    ctx = useColorSchemeContext();
    hasContext = true;
  } catch {
    // Context not available, will use local state
    hasContext = false;
  }
  
  // Use context if available, otherwise use local state
  const effectiveColorScheme: "light" | "dark" = hasContext && ctx
    ? ctx.colorScheme
    : localMode === "system"
      ? systemScheme === "dark" ? "dark" : "light"
      : localMode;
  
  const mode = hasContext && ctx ? ctx.mode : localMode;
  const setMode = hasContext && ctx ? ctx.setMode : setLocalMode;
  
  return [effectiveColorScheme, mode, setMode];
}
