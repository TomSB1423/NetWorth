import React, { createContext, useContext, useMemo, useState } from "react";
import { useColorScheme } from "react-native";

export type ColorSchemeMode = "system" | "light" | "dark";
export type EffectiveColorScheme = "light" | "dark";

interface ColorSchemeContextValue {
  colorScheme: EffectiveColorScheme;
  mode: ColorSchemeMode;
  setMode: (mode: ColorSchemeMode) => void;
}

const ColorSchemeContext = createContext<ColorSchemeContextValue | undefined>(undefined);

export function ColorSchemeProvider({ children }: { children: React.ReactNode }) {
  const systemScheme = useColorScheme();
  const [mode, setMode] = useState<ColorSchemeMode>("system");

  const colorScheme: EffectiveColorScheme =
    mode === "system"
      ? systemScheme === "dark" ? "dark" : "light"
      : mode;

  const value = useMemo(() => ({ colorScheme, mode, setMode }), [colorScheme, mode]);

  return (
    <ColorSchemeContext.Provider value={value}>
      {children}
    </ColorSchemeContext.Provider>
  );
}

export function useColorSchemeContext() {
  const ctx = useContext(ColorSchemeContext);
  if (!ctx) throw new Error("useColorSchemeContext must be used within a ColorSchemeProvider");
  return ctx;
}
